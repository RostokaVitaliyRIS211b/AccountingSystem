package BdHandling

import (
	gf "ServerModule/generalFunctions"
	lg "ServerModule/logging"
	s "database/sql"
	"errors"
	"os"
	"path/filepath"

	gs "github.com/RostokaVitaliyRIS211b/gosql"
	"github.com/RostokaVitaliyRIS211b/gosql/sqlstrings"
	st "github.com/RostokaVitaliyRIS211b/gosql/sqlstrings"
	_ "github.com/jackc/pgx/v5/stdlib"
	"go.uber.org/zap"
)

var Db *gs.DB = nil

const (
	PrtName               = "Producers"
	ntName                = "NameItem"
	toftName              = "TypeOfItem"
	toutName              = "TypesOfUnit"
	grptName              = "GroupingProperties"
	UsersTbName           = "Users"
	rolesOfUsrTbName      = "RolesOfUsers"
	rolesTbName           = "Roles"
	permsTbName           = "Permissions"
	permsOfRolesTbName    = "PermissionsForRoles"
	itemsTableName        = "Items"
	objMetaDataTbName     = "ObjectMetaData"
	objTbName             = "Objects"
	typeOfItemTbName      = "TypeOfItem"
	typeOfMetaDataTbName  = "TypesOfMetaData"
	itemsMetaDataTbName   = "ItemMetaData"
	grPropsForItemsTbName = "GroupingPropertiesForItems"
	IdColumnName          = "Id"
	NameColumnName        = "Name"
	DescColumnName        = "Description"
	TagName               = "db"
	wr                    = "\""
)

var PgDumpPath = "pg_dump"
var BackupFilePath = "backups"
var DbUser = "default"
var DbPassword = "default"
var DbHost = "default"
var DbName = "default"
var DbPort = "default"

func OpenNewConnection(connection string) {
	if Db == nil {
		db, err := s.Open("pgx", connection)
		if err != nil {
			lg.Logger.Error("Ошибка при подключении к базе ->", zap.Error(err))
			db.Close()
			Db = nil
			os.Exit(1)
		}

		wd, er := os.Getwd()
		if er == nil {
			BackupFilePath = filepath.Join(wd, BackupFilePath)
		}

		_, err = os.Stat(BackupFilePath)
		if err != nil {
			os.Mkdir(BackupFilePath, 0755)
		}

		Db = gs.GetDb(db, "postgres")

	}
}

func stdCloseRows(rows *s.Rows, err error) {
	if err == nil {
		rows.Close()
	} else {
		lg.Logger.Error("Ошибка при чтении строк -> ", zap.Error(err))
	}
}

//region StdConfigs

const wrapper = "\""

var selectQC = st.QueryConfig{
	TableName:    "",
	NameWrapper:  wrapper,
	ColumnName:   "",
	TagName:      TagName,
	Item:         nil,
	ExcludedTags: []string{},
	QueryType:    sqlstrings.SELECT,
}

var whereIdQC = st.QueryConfig{
	TableName:    "",
	NameWrapper:  wrapper,
	ColumnName:   IdColumnName,
	TagName:      TagName,
	Item:         nil,
	ExcludedTags: []string{},
	QueryType:    sqlstrings.SELECT,
}

var insertQC = st.QueryConfig{
	TableName:    "",
	NameWrapper:  wrapper,
	ColumnName:   IdColumnName,
	TagName:      TagName,
	Item:         nil,
	ExcludedTags: []string{IdColumnName},
	QueryType:    sqlstrings.INSERT,
}

var updateQC = st.QueryConfig{
	TableName:    "",
	NameWrapper:  wrapper,
	ColumnName:   IdColumnName,
	TagName:      TagName,
	Item:         nil,
	ExcludedTags: []string{IdColumnName},
	QueryType:    sqlstrings.UPDATE,
}

var deleteQC = st.QueryConfig{
	TableName:    "",
	NameWrapper:  wrapper,
	ColumnName:   IdColumnName,
	TagName:      TagName,
	Item:         nil,
	ExcludedTags: []string{IdColumnName},
	QueryType:    sqlstrings.DELETE,
}

//endregion

//region UserMethods

func GetUser(id int) (*User, error) {
	defer gf.TryCatch()
	var err error
	u := &User{}
	err = Db.Get(whereIdQC.ChangeTable(UsersTbName, User{}), u, id)
	u.Roles = GetAllRolesOfUser(u.Id)
	return u, err
}

func UpdateUser(user User) error {
	if len(user.Name) == 0 {
		return errors.New("Пустое имя пользователя")
	}
	if len(user.Password) == 0 {
		_, err := Db.Update(updateQC.ChangeTable(UsersTbName, user).ChangeExcludedTags("Password"))
		return err
	}
	_, err := Db.Update(updateQC.ChangeTable(UsersTbName, user))
	return err
}

func AddUser(user User) (int, error) {
	defer gf.TryCatch()
	if len(user.Name) == 0 {
		return -1, errors.New("empty name")
	}
	return Db.Insert(insertQC.ChangeTable(UsersTbName, user))
}

func DeleteUser(id int32) error {
	defer gf.TryCatch()
	_, err := Db.Delete(deleteQC.ChangeTable(UsersTbName, User{}), id)
	return err
}

func GetAllUsers() ([]*User, error) {
	defer gf.TryCatch()
	users := []*User{}
	err := Db.Select(selectQC.ChangeTable(UsersTbName, User{}), &users)

	for _, val := range users {
		val.Roles = GetAllRolesOfUser(val.Id)
	}

	return users, err
}

func GetAllRolesOfUser(userId int) []int32 {
	var roles []RolesOfUser
	err := Db.Select(selectQC.ChangeTable(rolesOfUsrTbName, RolesOfUser{}).ChangeColumnName("UserId"), &roles, userId)
	if err != nil {
		panic(err)
	}
	return gf.Select(gf.From(roles), func(r RolesOfUser) int32 { return int32(r.RoleId) }).ToList()
}

func SetUserRoles(userId int, roles []int) error {
	_, err := Db.Delete(deleteQC.ChangeTable(rolesOfUsrTbName, RolesOfUser{}).ChangeColumnName("UserId"), userId)
	if err != nil {
		return err
	}
	for _, value := range roles {
		_, err = Db.Insert(insertQC.ChangeTable(rolesOfUsrTbName, RolesOfUser{RoleId: value, UserId: userId}))
		if err != nil {
			lg.Logger.Error("Ошибка при задании роли пользователю -> ", zap.Error(err))
		}
	}
	return nil
}

var jpuq = ""

func GetAllPermissionsOfUser(userId int) ([]Permission, error) {
	result := []Permission{}
	// query := "SELECT \"Permissions\".\"Id\", \"Permissions\".\"Name\" FROM \"Permissions\" " +
	// 	"JOIN \"PermissionsForRoles\" ON \"Permissions\".\"Id\" = \"PermissionsForRoles\".\"PermId\" " +
	// 	"JOIN \"Roles\" ON \"PermissionsForRoles\".\"RoleId\" = \"Roles\".\"Id\" " +
	// 	"JOIN \"RolesOfUsers\" ON \"Roles\".\"Id\" = \"RolesOfUsers\".\"RoleId\" " +
	// 	"JOIN \"Users\" ON \"RolesOfUsers\".\"UserId\" = \"Users\".\"Id\" " +
	// 	"WHERE \"Users\".\"Id\" = $1"

	if len(jpuq) == 0 {
		column1, column2 := sqlstrings.TCC(permsTbName, IdColumnName), sqlstrings.TCC(permsTbName, NameColumnName)
		jpuq = selectQC.StartJoin(permsTbName, column1, column2).Join(IdColumnName, sqlstrings.TCC(permsOfRolesTbName, "PermId")).Join("RoleId", sqlstrings.TCC(rolesTbName, IdColumnName)).
			Join(IdColumnName, sqlstrings.TCC(rolesOfUsrTbName, "RoleId")).Join("UserId", sqlstrings.TCC(UsersTbName, IdColumnName)).Result(sqlstrings.TCC(UsersTbName, IdColumnName))
	}

	err := Db.SelectQuery(jpuq, selectQC.ChangeTable(permsTbName, Permission{}), &result, userId)

	return result, err
}

//endregion UserMethods

//region RoleMethods

func GetRole(id int) (*Role, error) {
	var role Role = Role{}
	err := Db.Get(whereIdQC.ChangeTable(rolesTbName, role), &role, id)
	return &role, err
}

func GetAllRoles() ([]Role, error) {
	var roles []Role = []Role{}
	err := Db.Select(selectQC.ChangeTable(rolesTbName, Role{}), &roles)

	if err != nil {
		return nil, err
	}

	var permsForRoles = []DbPermForRole{}
	err = Db.Select(selectQC.ChangeTable(permsOfRolesTbName, DbPermForRole{}), &permsForRoles)

	if err != nil {
		return nil, err
	}

	for idx := range roles {
		roles[idx].Permissions = getPermsOfRole(roles[idx].Id, permsForRoles)
	}

	return roles, err
}

func getPermsOfRole(roleId int, permsOfRoles []DbPermForRole) []int32 {
	var pIds = []int32{}
	for _, val := range permsOfRoles {
		if val.RoleId == roleId {
			pIds = append(pIds, int32(val.PermId))
		}
	}
	return pIds
}

var jpq = ""

func GetRolePermissions(roleId int) []Permission {
	result := []Permission{}
	// query := "SELECT \"Permissions\".\"Id\", \"Permissions\".\"Name\" FROM \"Permissions\" " +
	// 	"JOIN \"PermissionsForRoles\" ON \"Permissions\".\"Id\" = \"PermissionsForRoles\".\"PermId\" " +
	// 	"JOIN  \"Roles\" ON  \"PermissionsForRoles\".\"RoleId\" = \"Roles\".\"Id\" " +
	// 	"WHERE \"Roles\".\"Id\" = $1"
	if len(jpq) == 0 {
		column1, column2 := sqlstrings.TCC(permsTbName, IdColumnName), sqlstrings.TCC(permsTbName, NameColumnName)
		jpq = selectQC.StartJoin(permsTbName, column1, column2).Join(IdColumnName, sqlstrings.TCC(permsOfRolesTbName, "PermId")).Join("RoleId", sqlstrings.TCC(rolesTbName, IdColumnName)).
			Result(sqlstrings.TCC(rolesTbName, IdColumnName))
	}
	Db.SelectQuery(jpq, selectQC.ChangeTable(permsTbName, Permission{}), &result, roleId)

	return result
}

func AddRole(role Role) (int, error) {
	if len(role.Name) == 0 {
		return -1, errors.New("empty name")
	}
	return Db.Insert(insertQC.ChangeTable(rolesTbName, role))
}

func UpdateRole(role *Role) error {
	if len(role.Name) == 0 {
		return errors.New("empty name")
	}
	_, err := Db.Update(insertQC.ChangeTable(rolesTbName, role))
	return err
}

func DeleteRole(id int32) error {
	_, err := Db.Delete(deleteQC.ChangeTable(rolesTbName, Role{}), id)
	return err
}

func GetAllPermissions() ([]Permission, error) {
	var res = []Permission{}
	err := Db.Select(selectQC.ChangeTable(permsTbName, Permission{}), &res)
	return res, err
}

func SetRolePermissions(roleId int32, perms []int32) error {
	_, err := Db.Delete(deleteQC.ChangeTable(permsOfRolesTbName, DbPermForRole{}).ChangeColumnName("RoleId"), roleId)
	if err != nil {
		return err
	}

	for _, val := range perms {
		Db.Insert(insertQC.ChangeTable(permsOfRolesTbName, DbPermForRole{RoleId: int(roleId), PermId: int(val)}))
	}

	return nil
}

//endregion RoleMethods

//region Items related methods

func GetAllItems() ([]DbItem, error) {
	items := []DbItem{}
	err := Db.Select(selectQC.ChangeTable(itemsTableName, DbItem{}), &items)
	return items, err
}

func GetAllObjects() ([]Object, error) {
	items := []Object{}
	err := Db.Select(selectQC.ChangeTable(objTbName, Object{}), &items)
	return items, err
}

func GetAllProducers() []Producer {
	return getTypicalNigga[Producer]()
}

func GetAllGroupingProps() []GroupingProp {
	return getTypicalNigga[GroupingProp]()
}

func GetAllItemNames() []Name {
	return getTypicalNigga[Name]()
}

func GetAllTypesOfUnits() []TypeOfUnit {
	return getTypicalNigga[TypeOfUnit]()
}

func GetAllTypesOfItems() []TypeOfItem {
	return getTypicalNigga[TypeOfItem]()
}

func getTypicalNigga[T SimpleDbType]() []T {
	res := []T{}
	var it2 any = T{}
	var mess string = ""
	tableName := ""
	switch it2.(type) {
	case GroupingProp:
		tableName = grptName
	case Producer:
		tableName = PrtName
	case Name:
		tableName = ntName
	case TypeOfUnit:
		tableName = toutName
	case TypeOfItem:
		tableName = toftName
	}

	err := Db.Select(selectQC.ChangeTable(tableName, T{}), &res)

	if err != nil {
		lg.Logger.Error(mess, zap.Error(err))
	}

	return res
}

func GetItemsByObject(objId int) ([]Item, error) {
	defer gf.TryCatch()

	var resDb = []DbItem{}
	err := Db.Select(selectQC.ChangeTable(itemsTableName, DbItem{}).ChangeColumnName("Objectid"), &resDb, objId)

	if err != nil {
		return nil, err
	}

	var res = []Item{}

	for _, value := range resDb {

		var it = Item{
			Id:               value.Id,
			PricePerUnit:     value.PricePerUnit,
			ExcpectedCost:    value.ExcpectedCost,
			Description:      value.Description,
			CountOfUsedUnits: value.CountOfUsedUnits,
			CountOfUnits:     value.CountOfUnits,
		}

		var obj = Object{}
		var name = Name{}
		var producer = Producer{}
		var typeOfUnit = TypeOfUnit{}
		var typeOfItem = TypeOfItem{}

		Db.Get(whereIdQC.ChangeTable(objTbName, obj), &obj, value.ObjectId)
		it.ObjItem = obj

		Db.Get(whereIdQC.ChangeTable(ntName, name), &name, value.NameId)
		it.NameItem = name

		Db.Get(whereIdQC.ChangeTable(PrtName, producer), &producer, value.ProducerId)
		it.ItemProducer = producer

		Db.Get(whereIdQC.ChangeTable(toutName, typeOfUnit), &typeOfUnit, value.TypeUnitId)
		it.UnitType = typeOfUnit

		Db.Get(whereIdQC.ChangeTable(typeOfItemTbName, typeOfItem), &typeOfItem, value.TypeOfItem)
		it.ItemType = typeOfItem

		res = append(res, it)

	}

	return res, nil
}

var jgprq = ""

func GetGroupingPropsByItem(itemId int) ([]GroupingProp, error) {
	defer gf.TryCatch()
	var all = []GroupingProp{}
	// query := "SELECT " + grptName + "." + IdColumnName + ", " + grptName + "." + NameColumnName + " FROM " + grptName +
	// 	"JOIN " + grPropsForItemsTbName + " ON " + grptName + "." + IdColumnName + " = " + grPropsForItemsTbName + "." + "\"PropId\"" +
	// 	"WHERE " + grPropsForItemsTbName + "." + "\"ItemId\" = $1"
	if len(jgprq) == 0 {
		jgprq = selectQC.StartJoin(grptName, sqlstrings.TCC(grptName, IdColumnName), sqlstrings.TCC(grptName, NameColumnName)).Join(IdColumnName, sqlstrings.TCC(grPropsForItemsTbName, "PropId")).
			Result(sqlstrings.TCC(grPropsForItemsTbName, "ItemId"))
	}
	err := Db.SelectQuery(jgprq, selectQC.ChangeTable(grptName, GroupingProp{}), &all, itemId)

	return all, err
}

func GetMetaDataOfItem(itemId int) ([]ItemMetaData, error) {
	var res = []ItemMetaData{}

	err := Db.Select(selectQC.ChangeTable(itemsMetaDataTbName, ItemMetaData{}).ChangeColumnName("ItemId"), &res, itemId)

	if err != nil {
		return nil, err
	}

	return res, nil
}

func AddItem(item Item) (int, error) {
	return Db.Insert(insertQC.ChangeTable(itemsTableName, item.ConvertToBd()))
}

func UpdateItem(item Item) error {
	_, err := Db.Update(updateQC.ChangeTable(itemsTableName, item.ConvertToBd()))
	return err
}

func RemoveItem(itemId int32) error {
	_, err := Db.Delete(deleteQC.ChangeTable(itemsTableName, DbItem{}), itemId)
	return err
}

func AddGroupingProperty(prop GroupingProp) (int, error) {
	return Db.Insert(insertQC.ChangeTable(grptName, prop))
}

func AddProducer(prod Producer) (int, error) {
	return Db.Insert(insertQC.ChangeTable(PrtName, prod))
}

func AddNameItem(name Name) (int, error) {
	return Db.Insert(insertQC.ChangeTable(ntName, name))
}

func AddTypeOfUnit(unit TypeOfUnit) (int, error) {
	return Db.Insert(insertQC.ChangeTable(toutName, unit))
}

func AddItemMetaData(md ItemMetaData) (int, error) {
	return Db.Insert(insertQC.ChangeTable(itemsMetaDataTbName, md))
}

func AddGroupingPropertyOfItem(itemId int, prop GroupingProp) error {
	_, err := Db.Insert(insertQC.ChangeTable(grPropsForItemsTbName, DbGroupPropForItem{ItemId: itemId, PropId: prop.Id}))
	return err
}

func RemoveGroupingPropertyOfItem(itemId int, prop GroupingProp) error {
	query := "DELETE FROM " + wr + grPropsForItemsTbName + wr + " WHERE \"ItemId\"=$1 AND \"PropId\"=$2"
	_, err := Db.Exec(query, itemId, prop.Id)
	return err
}

func RemoveItemMetaData(id int) error {
	_, err := Db.Delete(deleteQC.ChangeTable(itemsMetaDataTbName, ItemMetaData{}), id)
	return err
}

func SetGroupingPropertiesOfItem(itemId int, grProps []GroupingProp) error {
	_, err := Db.Delete(deleteQC.ChangeTable(grPropsForItemsTbName, DbGroupPropForItem{}).ChangeColumnName("ItemId"), itemId)
	if err != nil {
		return err
	}

	for _, val := range grProps {
		_, err1 := Db.Insert(insertQC.ChangeTable(grPropsForItemsTbName, DbGroupPropForItem{ItemId: itemId, PropId: val.Id}))
		err = errors.Join(err1)
	}

	return err
}

//endregion Items related methods

//region ObjMethods

func GetAllObjectMetaData(objId int32) ([]ObjectMetaData, error) {
	var res = []ObjectMetaData{}

	err := Db.Select(selectQC.ChangeTable(objMetaDataTbName, ObjectMetaData{}).ChangeColumnName("ObjectId"), &res, objId)

	if err != nil {
		return nil, err
	}

	return res, nil
}

func AddObject(obj *Object) (int, error) {
	return Db.Insert(insertQC.ChangeTable(objTbName, *obj))
}

func UpdateObject(obj *Object) error {
	_, err := Db.Update(updateQC.ChangeTable(objTbName, *obj))
	return err
}

func RemoveObject(objId int32) error {
	_, err := Db.Delete(deleteQC.ChangeTable(objTbName, Object{}), objId)
	return err
}

var objMtDQC = insertQC.ChangeTable(objMetaDataTbName, ObjectMetaData{})

func AddObjectMetaData(md ObjectMetaData) (int, error) {
	return Db.Insert(objMtDQC.ChangeItem(md))
}

func RemoveObjectMetadata(id int) error {
	_, err := Db.Delete(objMtDQC, id)
	return err
}

//endregion ObjMethods

//region ShareMethods

func GetAllMetaDataTypes() ([]MetaDataType, error) {
	var res = []MetaDataType{}
	err := Db.Select(selectQC.ChangeTable(typeOfMetaDataTbName, MetaDataType{}), &res)
	if err != nil {
		return nil, err
	}
	return res, nil
}

//endregion
