package BdHandling

import (
	gf "ServerModule/generalFunctions"
	lg "ServerModule/logging"
	"errors"
	"os"
	"path/filepath"
	"reflect"
	"slices"
	"strconv"
	"strings"

	_ "github.com/jackc/pgx/v5/stdlib"
	s "github.com/jmoiron/sqlx"
	"go.uber.org/zap"
)

var Db *s.DB = nil

const (
	PrtName               = "\"Producers\""
	ntName                = "\"NameItem\""
	toftName              = "\"TypeOfItem\""
	toutName              = "\"TypesOfUnit\""
	grptName              = "\"GroupingProperties\""
	UsersTbName           = "\"Users\""
	rolesOfUsrTbName      = "\"RolesOfUsers\""
	rolesTbName           = "\"Roles\""
	permsTbName           = "\"Permissions\""
	permsOfRolesTbName    = "\"PermissionsForRoles\""
	itemsTableName        = "\"Items\""
	objMetaDataTbName     = "\"ObjectMetaData\""
	objTbName             = "\"Objects\""
	typeOfItemTbName      = "\"TypeOfItem\""
	typeOfMetaDataTbName  = "\"TypesOfMetaData\""
	itemsMetaDataTbName   = "\"ItemMetaData\""
	grPropsForItemsTbName = "\"GroupingPropertiesForItems\""
	IdColumnName          = "\"Id\""
	NameColumnName        = "\"Name\""
	DescColumnName        = "\"Description\""
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
		var err error
		Db, err = s.Connect("pgx", connection)
		if err != nil {
			lg.Logger.Error("Ошибка при подключении к базе ->", zap.Error(err))
			Db.Close()
			Db = nil
			os.Exit(1)
		}

		wd, er := os.Getwd()
		if er == nil {
			BackupFilePath = filepath.Join(wd, BackupFilePath)
		}

		_, err = os.Stat(BackupFilePath)
		if err != nil {
			os.Mkdir(BackupFilePath, 0777)
		}

	}
}

func stdCloseRows(rows *s.Rows, err error) {
	if err == nil {
		rows.Close()
	} else {
		lg.Logger.Error("Ошибка при чтении строк -> ", zap.Error(err))
	}
}

//region GeneralMethods

func GetSelectString(tableName string) string {
	return "SELECT * FROM " + tableName
}

func GetWhereSelectString(tableName string, columnName string) string {
	return "SELECT * FROM " + tableName + " WHERE " + columnName + " = $1"
}

// Id должен быть первым аргументом он делается $1 а столбцы в переданном порядке с $2
func GetUpdateSignleString(tableName string, idColumnName string, columnNames ...string) string {
	var builder strings.Builder
	additionalSymbols := 11
	totalSymbols := len(tableName) + len(idColumnName) + additionalSymbols

	for _, w := range columnNames {
		totalSymbols += len(w) + 4
	}

	builder.Grow(totalSymbols)

	builder.WriteString("UPDATE ")
	builder.WriteString(tableName)
	builder.WriteString(" SET ")

	for idx, w := range columnNames {
		argIdx := idx + 2
		if idx > 0 {
			builder.WriteString(", ")
		}
		builder.WriteString(w)
		builder.WriteString(" = $")
		builder.WriteString(strconv.Itoa(argIdx))

	}
	builder.WriteString(" WHERE ")
	builder.WriteString(idColumnName)
	builder.WriteString(" = $1")

	return builder.String()
}

// insert into \"Users\" (\"Name\",\"Password\",\"Description\") values ($1,$2,$3) returning id
func GetInsertString(tableName string, columnNames ...string) string {
	var builder strings.Builder

	additionalSymbols := 37
	totalSymbols := len(tableName) + additionalSymbols

	for _, w := range columnNames {
		totalSymbols += len(w) + 1
	}

	builder.Grow(totalSymbols)
	builder.WriteString("INSERT")
	builder.WriteString(" INTO ")
	builder.WriteString(tableName)
	builder.WriteString(" (")

	for idx, w := range columnNames {
		if idx > 0 {
			builder.WriteString(",")
		}
		builder.WriteString(w)
	}

	builder.WriteString(") VALUES (")

	for idx := range columnNames {
		if idx > 0 {
			builder.WriteString(",")
		}
		builder.WriteString("$")
		builder.WriteString(strconv.Itoa(idx + 1))
	}

	builder.WriteString(") RETURNING " + IdColumnName)

	return builder.String()
}

func GetInsertStringReflect(tableName string, nigger any) string {
	var builder strings.Builder

	additionalSymbols := 37
	totalSymbols := len(tableName) + additionalSymbols

	builder.WriteString("INSERT")
	builder.WriteString(" INTO ")
	builder.WriteString(tableName)
	builder.WriteString(" (")
	builder.Grow(totalSymbols)

	counter := 0

	typeOfN := reflect.TypeOf(nigger)
	if kind := typeOfN.Kind(); kind == reflect.Pointer || kind == reflect.Interface {
		val := reflect.ValueOf(nigger)
		typeOfN = val.Elem().Type()
	}
	isFieldDb := false
	isPrevFieldDb := isFieldDb
	for i := 0; i < typeOfN.NumField(); i++ {
		tag := typeOfN.Field(i).Tag.Get("db")

		isPrevFieldDb = isFieldDb
		isFieldDb = len(tag) > 0 && strings.ToLower(tag) != "id"

		if isFieldDb && isPrevFieldDb {
			builder.WriteString(",")
		}
		if isFieldDb {
			builder.WriteString(TrapNigger(tag))
			counter++
		}

	}

	builder.WriteString(") VALUES (")
	for idx := range counter {
		if idx > 0 {
			builder.WriteString(",")
		}
		builder.WriteString("$")
		builder.WriteString(strconv.Itoa(idx + 1))
	}

	builder.WriteString(") RETURNING " + IdColumnName)
	return builder.String()
}

// "delete from \"Users\" where \"Id\" = $1"
func GetDeleteString(tableName string, idColumnName string) string {
	return "DELETE FROM " + tableName + " WHERE " + idColumnName + " = $1"
}

func CommonInsert(tableName string, record any) (int, error) {
	result := -1

	var args []any
	valueOfR := reflect.ValueOf(record)
	typeOfR := reflect.TypeOf(record)

	if kind := typeOfR.Kind(); kind == reflect.Pointer || kind == reflect.Interface {
		valueOfR = valueOfR.Elem()
		typeOfR = valueOfR.Type()
	}
	query := GetInsertStringReflect(tableName, record)
	for i := 0; i < valueOfR.NumField(); i++ {
		tpField := typeOfR.Field(i)
		tag := tpField.Tag.Get("db")
		if len(tag) > 0 && strings.ToLower(tag) != "id" {
			field := valueOfR.Field(i)
			args = append(args, field.Interface())
		}
	}
	err := Db.QueryRow(query, args...).Scan(&result)
	return result, err
}

func GetUpdateSignleStringReflect(tableName string, idColumnName string, record any, excludedTags ...string) string {
	var builder strings.Builder
	additionalSymbols := 11
	totalSymbols := len(tableName) + len(idColumnName) + additionalSymbols
	builder.Grow(totalSymbols)

	builder.WriteString("UPDATE ")
	builder.WriteString(tableName)
	builder.WriteString(" SET ")

	typeOfN := reflect.TypeOf(record)
	if kind := typeOfN.Kind(); kind == reflect.Pointer || kind == reflect.Interface {
		typeOfN = reflect.ValueOf(record).Elem().Type()
	}
	isFieldDb := false
	counter := 0
	var innerBuilder strings.Builder
	for i := 0; i < typeOfN.NumField(); i++ {
		tag := typeOfN.Field(i).Tag.Get("db")

		isFieldDb = len(tag) > 0 && strings.ToLower(tag) != "id" && !slices.Contains(excludedTags, tag)

		if isFieldDb {
			innerBuilder.WriteString(TrapNigger(tag))
			innerBuilder.WriteString(" = ")
			innerBuilder.WriteString("$")
			innerBuilder.WriteString(strconv.Itoa(counter + 2))
			innerBuilder.WriteString(", ")
			counter++
		}

	}

	res := innerBuilder.String()
	builder.WriteString(strings.TrimSuffix(res, ", "))
	builder.WriteString(" WHERE ")
	builder.WriteString(idColumnName)
	builder.WriteString(" = $1")

	return builder.String()
}

func CommonUpdate(tableName string, idColumnName string, record any, excludedTags ...string) error {

	var args []any
	typeOfR := reflect.TypeOf(record)
	valOfR := reflect.ValueOf(record)
	if kind := typeOfR.Kind(); kind == reflect.Pointer || kind == reflect.Interface {
		valOfR = valOfR.Elem()
		typeOfR = valOfR.Type()
	}
	query := GetUpdateSignleStringReflect(tableName, idColumnName, record)
	for i := 0; i < valOfR.NumField(); i++ {
		tag := typeOfR.Field(i).Tag.Get("db")
		if len(tag) > 0 {
			args = append(args, valOfR.Field(i).Interface())
		}
	}
	_, err := Db.Exec(query, args...)
	return err
}

func TrapNigger(n string) string {
	return "\"" + n + "\""
}

//endregion

//region UserMethods

func GetUser(id int) (*User, error) {
	defer gf.TryCatch()
	row := Db.QueryRowx(GetWhereSelectString(UsersTbName, IdColumnName), id)
	var u User = User{}
	err := row.StructScan(&u)
	if err != nil {
		return nil, err
	}
	rolesOfUser := GetAllRolesOfUser(u.Id)
	u.Roles = gf.Select(gf.From(rolesOfUser), func(r *Role) int32 { return int32(r.Id) }).ToList()
	return &u, err
}

func UpdateUser(user User) error {
	if len(user.Name) == 0 {
		return errors.New("Пустое имя пользователя")
	}
	if len(user.Password) == 0 {
		return CommonUpdate(UsersTbName, IdColumnName, user, "Password")
	}
	return CommonUpdate(UsersTbName, IdColumnName, user)
}

func AddUser(user User) (int, error) {
	defer gf.TryCatch()
	if len(user.Name) == 0 {
		return -1, errors.New("empty name")
	}
	return CommonInsert(UsersTbName, user)
}

func DeleteUser(id int32) error {
	defer gf.TryCatch()
	_, err := Db.Exec(GetDeleteString(UsersTbName, IdColumnName), id)
	return err
}

func GetAllUsers() ([]*User, error) {
	defer gf.TryCatch()
	users := []*User{}
	err := Db.Select(&users, GetSelectString(UsersTbName))

	for _, val := range users {
		rolesOfUser := GetAllRolesOfUser(val.Id)
		val.Roles = gf.Select(gf.From(rolesOfUser), func(r *Role) int32 { return int32(r.Id) }).ToList()
	}

	return users, err
}

func GetAllRolesOfUser(userId int) []*Role {
	defer gf.TryCatch()
	var rIds []int = []int{}
	rows, err := Db.Queryx(GetWhereSelectString(rolesOfUsrTbName, "\"UserId\""), userId)

	if err != nil {
		lg.Logger.Error("Ошибка при чтении ролей для пользователя ", zap.Error(err))
		return nil
	}

	for rows.Next() {
		var UserId, RoleId, Id int
		err1 := rows.Scan(&Id, &UserId, &RoleId)
		if err1 != nil {
			lg.Logger.Error("Ошибка при чтении роли пользователя -> ", zap.Error(err1))
			continue
		}
		rIds = append(rIds, RoleId)
	}

	rows.Close()

	rows, err = Db.Queryx(GetSelectString(rolesTbName))
	defer stdCloseRows(rows, err)

	var roles []*Role
	for rows.Next() {
		var role *Role = new(Role)
		err := rows.StructScan(role)
		if err != nil {
			lg.Logger.Error("Ошибка при чтении роли у пользователя -> ", zap.Error(err))
			continue
		}

		if slices.Contains(rIds, role.Id) {
			roles = append(roles, role)
		}

	}
	return roles
}

func SetUserRoles(userId int, roles []int) error {
	_, err := Db.Exec(GetDeleteString(rolesOfUsrTbName, "\"UserId\""), userId)
	if err != nil {
		return err
	}
	for _, value := range roles {
		_, err = Db.Exec(GetInsertString(rolesOfUsrTbName, "\"UserId\"", "\"RoleId\""), userId, value)
		if err != nil {
			lg.Logger.Error("Ошибка при задании роли пользователю -> ", zap.Error(err))
		}
	}
	return nil
}

func GetAllPermissionsOfUser(userId int) ([]Permission, error) {
	result := []Permission{}
	query := "SELECT \"Permissions\".\"Id\", \"Permissions\".\"Name\" FROM \"Permissions\" " +
		"JOIN \"PermissionsForRoles\" ON \"Permissions\".\"Id\" = \"PermissionsForRoles\".\"PermId\" " +
		"JOIN  \"Roles\" ON  \"PermissionsForRoles\".\"RoleId\" = \"Roles\".\"Id\" " +
		"JOIN \"RolesOfUsers\" ON \"Roles\".\"Id\" = \"RolesOfUsers\".\"RoleId\" " +
		"JOIN \"Users\" ON \"RolesOfUsers\".\"UserId\" = \"Users\".\"Id\" " +
		"WHERE \"Users\".\"Id\" = $1"

	err := Db.Select(&result, query, userId)

	return result, err
}

//endregion UserMethods

//region RoleMethods

func GetRole(id int) (*Role, error) {
	var role Role = Role{}
	err := Db.Get(&role, GetWhereSelectString(rolesTbName, IdColumnName), id)
	return &role, err
}

func GetAllRoles() ([]Role, error) {
	var roles []Role = []Role{}
	err := Db.Select(&roles, GetSelectString(rolesTbName))

	if err != nil {
		return nil, err
	}

	var permsForRoles = []DbPermForRole{}
	err = Db.Select(&permsForRoles, GetSelectString(permsOfRolesTbName))

	if err != nil {
		return nil, err
	}

	var permissions = []Permission{}

	err = Db.Select(&permissions, GetSelectString(permsTbName))

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

func GetRolePermissions(roleId int) []Permission {
	result := []Permission{}
	query := "SELECT \"Permissions\".\"Id\", \"Permissions\".\"Name\" FROM \"Permissions\" " +
		"JOIN \"PermissionsForRoles\" ON \"Permissions\".\"Id\" = \"PermissionsForRoles\".\"PermId\" " +
		"JOIN  \"Roles\" ON  \"PermissionsForRoles\".\"RoleId\" = \"Roles\".\"Id\" " +
		"WHERE \"Roles\".\"Id\" = $1"

	Db.Select(&result, query, roleId)

	return result
}

func AddRole(role Role) (int, error) {
	if len(role.Name) == 0 {
		return -1, errors.New("empty name")
	}
	return CommonInsert(rolesTbName, role)
}

func UpdateRole(role *Role) error {
	if len(role.Name) == 0 {
		return errors.New("empty name")
	}

	return CommonUpdate(rolesTbName, IdColumnName, role)
}

func DeleteRole(id int32) error {
	_, err := Db.Exec(GetDeleteString(rolesTbName, IdColumnName), id)
	return err
}

func GetAllPermissions() ([]Permission, error) {
	var res = []Permission{}
	err := Db.Select(&res, GetSelectString(permsTbName))
	return res, err
}

func SetRolePermissions(roleId int32, perms []int32) error {
	_, err := Db.Exec(GetDeleteString(permsOfRolesTbName, "\"RoleId\""), roleId)
	if err != nil {
		return err
	}

	for _, val := range perms {
		Db.Exec(GetInsertString(permsOfRolesTbName, "\"RoleId\"", "\"PermId\""), roleId, val)
	}

	return nil
}

//endregion RoleMethods

//region Items related methods

func GetAllItems() ([]Item, error) {
	items := []Item{}
	err := Db.Select(&items, GetSelectString(itemsTableName))
	return items, err
}

func GetAllObjects() ([]Object, error) {
	items := []Object{}
	err := Db.Select(&items, GetSelectString(objTbName))
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
	var query string = ""
	switch it2.(type) {
	case GroupingProp:
		query = GetSelectString(grptName)
		mess = "Ошибка при чтении свойства группировки -> "
	case Producer:
		query = GetSelectString(PrtName)
		mess = "Ошибка при чтении производителя -> "
	case Name:
		query = GetSelectString(ntName)
		mess = "Ошибка при чтении имени записи -> "
	case TypeOfUnit:
		query = GetSelectString(toutName)
		mess = "Ошибка при чтении типа ед изм -> "
	case TypeOfItem:
		query = GetSelectString(toftName)
		mess = "Ошибка при чтении типа записи -> "
	}

	err := Db.Select(&res, query)

	if err != nil {
		lg.Logger.Error(mess, zap.Error(err))
	}

	return res
}

func GetItemsByObject(objId int) ([]Item, error) {
	defer gf.TryCatch()

	var resDb = []DbItem{}
	err := Db.Select(&resDb, GetWhereSelectString(itemsTableName, "\"Objectid\""), objId)

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

		Db.Get(&obj, GetWhereSelectString(objTbName, IdColumnName), value.ObjectId)
		it.ObjItem = obj

		Db.Get(&name, GetWhereSelectString(ntName, IdColumnName), value.NameId)
		it.NameItem = name

		Db.Get(&producer, GetWhereSelectString(PrtName, IdColumnName), value.ProducerId)
		it.ItemProducer = producer

		Db.Get(&typeOfUnit, GetWhereSelectString(toutName, IdColumnName), value.TypeUnitId)
		it.UnitType = typeOfUnit

		Db.Get(&typeOfItem, GetWhereSelectString(typeOfItemTbName, IdColumnName), value.TypeOfItem)
		it.ItemType = typeOfItem

		res = append(res, it)

	}

	return res, nil
}

func GetGroupingPropsByItem(itemId int) ([]GroupingProp, error) {
	defer gf.TryCatch()
	var all = []GroupingProp{}
	query := "SELECT " + grptName + "." + IdColumnName + ", " + grptName + "." + NameColumnName + " FROM " + grptName +
		"JOIN " + grPropsForItemsTbName + " ON " + grptName + "." + IdColumnName + " = " + grPropsForItemsTbName + "." + "\"PropId\"" +
		"WHERE " + grPropsForItemsTbName + "." + "\"ItemId\" = $1"
	err := Db.Select(&all, query, itemId)

	return all, err
}

func GetMetaDataOfItem(itemId int) ([]ItemMetaData, error) {
	var res = []ItemMetaData{}

	err := Db.Select(&res, GetWhereSelectString(itemsMetaDataTbName, "\"ItemId\""), itemId)

	if err != nil {
		return nil, err
	}

	return res, nil
}

func AddItem(item Item) (int, error) {
	return CommonInsert(itemsTableName, item.ConvertToBd())
}

func UpdateItem(item Item) error {
	return CommonUpdate(itemsTableName, IdColumnName, item.ConvertToBd())
}

func RemoveItem(itemId int32) error {
	_, err := Db.Exec(GetDeleteString(itemsTableName, IdColumnName), itemId)
	return err
}

func AddGroupingProperty(prop GroupingProp) (int, error) {
	return CommonInsert(grptName, prop)
}

func AddProducer(prod Producer) (int, error) {
	return CommonInsert(PrtName, prod)
}

func AddNameItem(name Name) (int, error) {
	return CommonInsert(ntName, name)
}

func AddTypeOfUnit(unit TypeOfUnit) (int, error) {
	return CommonInsert(toutName, unit)
}

func AddItemMetaData(md ItemMetaData) (int, error) {
	return CommonInsert(itemsMetaDataTbName, md)
}

func AddGroupingPropertyOfItem(itemId int, prop GroupingProp) error {
	_, err := CommonInsert(grPropsForItemsTbName, DbGroupPropForItem{ItemId: itemId, PropId: prop.Id})
	return err
}

func RemoveGroupingPropertyOfItem(itemId int, prop GroupingProp) error {
	query := "DELETE FROM " + grPropsForItemsTbName + " WHERE \"ItemId\"=$1 AND \"PropId\"=$2"
	_, err := Db.Exec(query, itemId, prop.Id)
	return err
}

func RemoveItemMetaData(id int) error {
	_, err := Db.Exec(GetDeleteString(itemsMetaDataTbName, IdColumnName), id)
	return err
}

func SetGroupingPropertiesOfItem(itemId int, grProps []GroupingProp) error {
	query := "DELETE FROM " + grPropsForItemsTbName + " WHERE \"ItemId\"=$1"
	_, err := Db.Exec(query, itemId)
	if err != nil {
		return err
	}

	for _, val := range grProps {
		_, err1 := CommonInsert(grPropsForItemsTbName, DbGroupPropForItem{ItemId: itemId, PropId: val.Id})
		err = errors.Join(err1)
	}

	return err
}

//endregion Items related methods

//region ObjMethods

func GetAllObjectMetaData(objId int32) ([]ObjectMetaData, error) {
	var res = []ObjectMetaData{}

	err := Db.Select(&res, GetWhereSelectString(objMetaDataTbName, "\"ObjectId\""), objId)

	if err != nil {
		return nil, err
	}

	return res, nil
}

func AddObject(obj *Object) (int, error) {
	return CommonInsert(objTbName, obj)
}

func UpdateObject(obj *Object) error {
	return CommonUpdate(objTbName, IdColumnName, obj)
}

func RemoveObject(objId int32) error {
	_, err := Db.Exec(GetDeleteString(objTbName, IdColumnName), objId)
	return err
}

func AddObjectMetaData(md ObjectMetaData) (int, error) {
	return CommonInsert(objMetaDataTbName, md)
}

func RemoveObjectMetadata(id int) error {
	_, err := Db.Exec(GetDeleteString(objMetaDataTbName, IdColumnName), id)
	return err
}

//endregion ObjMethods

//region ShareMethods

func GetAllMetaDataTypes() ([]MetaDataType, error) {
	var res = []MetaDataType{}
	err := Db.Select(&res, GetSelectString(typeOfMetaDataTbName))
	if err != nil {
		return nil, err
	}
	return res, nil
}

//endregion
