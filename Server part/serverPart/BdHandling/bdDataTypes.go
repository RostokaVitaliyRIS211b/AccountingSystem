package BdHandling

import mb "ServerModule/gen/proto/mains"

type User struct {
	Id          int    `db:"Id"`
	Name        string `db:"Name"`
	Password    string `db:"Password"`
	Description string `db:"Description"`
	Roles       []int32
}

type Role struct {
	Id          int    `db:"Id"`
	Name        string `db:"Name"`
	Description string `db:"Description"`
	Permissions []int32
}

type Permission struct {
	Id   int    `db:"Id"`
	Name string `db:"Name"`
}

type DbPermForRole struct {
	Id     int `db:"Id"`
	RoleId int `db:"RoleId"`
	PermId int `db:"PermId"`
}

type GroupingProp struct {
	Id   int    `db:"Id"`
	Name string `db:"Name"`
}

type DbItem struct {
	Id               int     `db:"Id"`
	ObjectId         int     `db:"Objectid"`
	TypeUnitId       int     `db:"TypeUnitId"`
	CountOfUnits     float64 `db:"CountOfUnits"`
	PricePerUnit     float64 `db:"PricePerUnit"`
	ExcpectedCost    float64 `db:"ExcpectedCost"`
	ProducerId       int     `db:"ProducerId"`
	Description      string  `db:"Description"`
	TypeOfItem       int     `db:"TypeOfItemId"`
	NameId           int     `db:"NameId"`
	CountOfUsedUnits float64 `db:"CountOfUsedUnits"`
}

type DbGroupPropForItem struct {
	Id     int `db:"Id"`
	PropId int `db:"PropId"`
	ItemId int `db:"ItemId"`
}

type Item struct {
	Id               int
	NameItem         Name
	ObjItem          Object
	UnitType         TypeOfUnit
	PricePerUnit     float64
	ExcpectedCost    float64
	ItemProducer     Producer
	Description      string
	ItemType         TypeOfItem
	CountOfUsedUnits float64
	CountOfUnits     float64
}

type ItemMetaData struct {
	Id         int    `db:"Id"`
	Data       []byte `db:"Data"`
	TypeOfData int    `db:"DataTypeId"`
	ItemId     int    `db:"ItemId"`
	Name       string `db:"Name"`
}

type Object struct {
	Id          int    `db:"Id"`
	Name        string `db:"Name"`
	Description string `db:"Description"`
	Address     string `db:"Address"`
}

type ObjectMetaData struct {
	Id         int    `db:"Id"`
	TypeOfData int    `db:"DataTypeId"`
	ObjectId   int    `db:"ObjectId"`
	Name       string `db:"Name"`
	Data       []byte `db:"Data"`
}

type MetaDataType struct {
	Id   int    `db:"Id"`
	Name string `db:"Name"`
}

type Producer struct {
	Id   int    `db:"Id"`
	Name string `db:"Name"`
}

type Name struct {
	Id   int    `db:"Id"`
	Name string `db:"Name"`
}

type TypeOfItem struct {
	Id   int    `db:"Id"`
	Name string `db:"Name"`
}

type TypeOfUnit struct {
	Id   int    `db:"Id"`
	Name string `db:"Name"`
}

type SimpleDbType interface {
	GroupingProp | TypeOfUnit | TypeOfItem | Name | Producer
}

//region ConvertFuncs

//region Db types to Proto

func (o *Object) Convert() *mb.ProtoObject {
	return &mb.ProtoObject{Id: int32(o.Id), Name: o.Name, Description: o.Description, Address: o.Address}
}

func (o Object) ConvertS() *mb.ProtoObject {
	return &mb.ProtoObject{Id: int32(o.Id), Name: o.Name, Description: o.Description, Address: o.Address}
}

func (g *GroupingProp) Convert() *mb.ProtoGroupingProperty {
	return &mb.ProtoGroupingProperty{Id: int32(g.Id), Name: g.Name}
}

func (t *TypeOfUnit) Convert() *mb.ProtoTypeOfUnit {
	return &mb.ProtoTypeOfUnit{Id: int32(t.Id), Name: t.Name}
}

func (t *TypeOfItem) Convert() *mb.ProtoTypeOfItem {
	return &mb.ProtoTypeOfItem{Id: int32(t.Id), Name: t.Name}
}

func (n *Name) Convert() *mb.ProtoNameItem {
	return &mb.ProtoNameItem{Id: int32(n.Id), Name: n.Name}
}

func (p *Producer) Convert() *mb.ProtoProducer {
	return &mb.ProtoProducer{Id: int32(p.Id), Name: p.Name}
}

func (i *Item) Convert() *mb.ProtoItem {
	return &mb.ProtoItem{
		Id:               int32(i.Id),
		NameItem:         i.NameItem.Convert(),
		Obj:              i.ObjItem.Convert(),
		UnitType:         i.UnitType.Convert(),
		Type:             i.ItemType.Convert(),
		Producer:         i.ItemProducer.Convert(),
		CountOfUnits:     i.CountOfUnits,
		CountOfUsedUnits: i.CountOfUsedUnits,
		PricePerUnit:     i.PricePerUnit,
		ExpectedCost:     i.ExcpectedCost,
		Description:      i.Description,
	}
}

func (i *Item) ConvertToBd() *DbItem {
	return &DbItem{
		Id:               i.Id,
		ObjectId:         i.ObjItem.Id,
		TypeUnitId:       i.UnitType.Id,
		CountOfUnits:     i.CountOfUnits,
		PricePerUnit:     i.PricePerUnit,
		ExcpectedCost:    i.ExcpectedCost,
		ProducerId:       i.ItemProducer.Id,
		Description:      i.Description,
		TypeOfItem:       i.ItemType.Id,
		NameId:           i.NameItem.Id,
		CountOfUsedUnits: i.CountOfUsedUnits,
	}
}

func (r *Role) Convert() *mb.ProtoRole {
	return &mb.ProtoRole{Id: int32(r.Id), Name: r.Name, Description: r.Description, Permissions: r.Permissions}
}

func (u *User) Convert() *mb.ProtoUser {
	return &mb.ProtoUser{Id: int32(u.Id), Name: u.Name, Description: u.Description, Password: "", Roles: u.Roles}
}

func (p *Permission) Convert() *mb.ProtoPermission {
	return &mb.ProtoPermission{Id: int32(p.Id), Name: p.Name}
}

func (om *ObjectMetaData) Convert() *mb.ProtoObjectMetadata {
	return &mb.ProtoObjectMetadata{Id: int32(om.Id), Name: om.Name, ObjId: int32(om.ObjectId), TypeId: int32(om.TypeOfData), Data: om.Data}
}

func (mdt *MetaDataType) Convert() *mb.ProtoMetaDataType {
	return &mb.ProtoMetaDataType{Id: int32(mdt.Id), Name: mdt.Name}
}

func (mdi *ItemMetaData) Convert() *mb.ProtoItemMetaData {
	return &mb.ProtoItemMetaData{Id: int32(mdi.Id), Name: mdi.Name, ItemId: int32(mdi.ItemId), TypeId: int32(mdi.TypeOfData), Data: mdi.Data}
}

//endregion  Db types to Proto

//region Proto to db types

func ConvertOb(o *mb.ProtoObject) *Object {
	return &Object{Id: int(o.Id), Name: o.Name, Description: o.Description, Address: o.Address}
}

func ConvertGr(g *mb.ProtoGroupingProperty) *GroupingProp {
	return &GroupingProp{Id: int(g.Id), Name: g.Name}
}

func ConvertTou(g *mb.ProtoTypeOfUnit) *TypeOfUnit {
	return &TypeOfUnit{Id: int(g.Id), Name: g.Name}
}

func ConvertToi(g *mb.ProtoTypeOfItem) *TypeOfItem {
	return &TypeOfItem{Id: int(g.Id), Name: g.Name}
}

func ConvertN(g *mb.ProtoNameItem) *Name {
	return &Name{Id: int(g.Id), Name: g.Name}
}

func ConvertR(r *mb.ProtoRole) *Role {
	return &Role{Id: int(r.Id), Name: r.Name, Description: r.Description, Permissions: r.Permissions}
}

func ConvertPr(pr *mb.ProtoProducer) *Producer {
	return &Producer{Id: int(pr.Id), Name: pr.Name}
}

func ConvertItem(i *mb.ProtoItem) *Item {
	return &Item{
		Id:               int(i.Id),
		ObjItem:          *ConvertOb(i.Obj),
		UnitType:         *ConvertTou(i.UnitType),
		CountOfUnits:     i.CountOfUnits,
		PricePerUnit:     i.PricePerUnit,
		ExcpectedCost:    i.ExpectedCost,
		ItemProducer:     *ConvertPr(i.Producer),
		Description:      i.Description,
		ItemType:         *ConvertToi(i.Type),
		NameItem:         *ConvertN(i.NameItem),
		CountOfUsedUnits: i.CountOfUsedUnits,
	}
}

func ConvertUser(u *mb.ProtoUser) *User {
	return &User{
		Id:          int(u.Id),
		Name:        u.Name,
		Password:    u.Password,
		Description: u.Description,
		Roles:       u.Roles,
	}
}

func ConvertItMetaDate(md *mb.ProtoItemMetaData) ItemMetaData {
	return ItemMetaData{
		Id:         int(md.Id),
		Data:       md.Data,
		TypeOfData: int(md.TypeId),
		ItemId:     int(md.ItemId),
		Name:       md.Name,
	}
}

func ConvertObjMetaData(md *mb.ProtoObjectMetadata) ObjectMetaData {
	return ObjectMetaData{
		Id:         int(md.Id),
		TypeOfData: int(md.TypeId),
		ObjectId:   int(md.ObjId),
		Name:       md.Name,
		Data:       md.Data,
	}
}

//endregion Proto to db types

//endregion ConvertFuncs
