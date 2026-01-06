package mainservice

import (
	"context"
	"fmt"
	"io"
	"os"
	"os/exec"
	"path/filepath"
	"sync"
	"sync/atomic"

	lg "ServerModule/logging"

	mb "ServerModule/gen/proto/mains"

	"github.com/google/uuid"
	"go.uber.org/zap"

	"google.golang.org/protobuf/types/known/emptypb"

	gf "ServerModule/generalFunctions"

	db "ServerModule/BdHandling"

	sd "ServerModule/server/sessionDataHandling"

	"google.golang.org/grpc/metadata"

	"google.golang.org/grpc/status"

	"google.golang.org/grpc/codes"

	"google.golang.org/grpc"
)

type MainService struct {
	mb.UnimplementedAccountingSystemServer
}

func (*MainService) AddSessionData(con context.Context, em *emptypb.Empty) (*mb.PBool, error) {
	defer gf.TryCatch()
	var result *mb.PBool = new(mb.PBool)
	result.Val = true
	md, ok := metadata.FromIncomingContext(con)
	if !ok {
		result.Val = false
		return result, status.Error(codes.Unauthenticated, "missing metadata")
	}

	id := md["id"]
	if len(id) == 0 {
		result.Val = false
		return result, status.Error(codes.Unauthenticated, "missing id header")
	} else {
		sd.AddUser(id[0])
	}

	return result, nil
}

func (*MainService) AbortSessionData(con context.Context, em *emptypb.Empty) (*emptypb.Empty, error) {
	defer gf.TryCatch()

	md, ok := metadata.FromIncomingContext(con)
	if !ok {
		return nil, status.Error(codes.Unauthenticated, "missing metadata")
	}

	id := md["id"]
	if len(id) == 0 {
		return nil, status.Error(codes.Unauthenticated, "missing id header")
	} else {
		sd.AddUser(id[0])
	}

	return new(emptypb.Empty), nil
}

func (*MainService) GetAllObjects(context.Context, *emptypb.Empty) (*mb.List_Objects, error) {
	defer gf.TryCatch()

	result := new(mb.List_Objects)

	objs, err := db.GetAllObjects()

	for _, val := range objs {
		result.Objs = append(result.Objs, val.Convert())
	}

	return result, err
}

func (*MainService) GetAllProducers(context.Context, *emptypb.Empty) (*mb.List_Producers, error) {
	defer gf.TryCatch()

	result := new(mb.List_Producers)

	for _, val := range db.GetAllProducers() {
		result.Producers = append(result.Producers, val.Convert())
	}

	return result, nil
}

func (*MainService) GetAllNames(context.Context, *emptypb.Empty) (*mb.List_NameItems, error) {
	defer gf.TryCatch()

	result := new(mb.List_NameItems)

	for _, val := range db.GetAllItemNames() {
		result.Names = append(result.Names, val.Convert())
	}

	return result, nil
}

func (*MainService) GetAllTypesOfUnit(context.Context, *emptypb.Empty) (*mb.List_TypesOfUnit, error) {
	defer gf.TryCatch()

	result := new(mb.List_TypesOfUnit)

	for _, val := range db.GetAllTypesOfUnits() {
		result.Types = append(result.Types, val.Convert())
	}

	return result, nil
}

func (*MainService) GetAllTypesOfItems(context.Context, *emptypb.Empty) (*mb.List_TypeOfItems, error) {

	defer gf.TryCatch()

	result := new(mb.List_TypeOfItems)

	for _, val := range db.GetAllTypesOfItems() {
		result.Types = append(result.Types, val.Convert())
	}

	return result, nil
}

func (*MainService) GetAllGroupingProps(context.Context, *emptypb.Empty) (*mb.List_GroupingProps, error) {

	defer gf.TryCatch()

	result := new(mb.List_GroupingProps)

	for _, val := range db.GetAllGroupingProps() {
		result.Props = append(result.Props, val.Convert())
	}

	return result, nil
}

func (*MainService) CheckActive(context.Context, *emptypb.Empty) (*emptypb.Empty, error) {
	return &emptypb.Empty{}, nil
}

func (*MainService) GetItemsByObject(c context.Context, req *mb.PInt) (*mb.List_Items, error) {
	defer gf.TryCatch()
	var res *mb.List_Items = new(mb.List_Items)

	items, err := db.GetItemsByObject(int(req.Val))

	if err != nil {
		return nil, err
	}

	for _, val := range items {
		res.Items = append(res.Items, val.Convert())
	}

	return res, nil
}

func (*MainService) GetGroupingPropsByItem(c context.Context, req *mb.PInt) (*mb.List_GroupingProps, error) {
	defer gf.TryCatch()
	var res *mb.List_GroupingProps = new(mb.List_GroupingProps)
	resGs, err := db.GetGroupingPropsByItem(int(req.Val))

	if err != nil {
		return nil, err
	}

	for _, val := range resGs {
		res.Props = append(res.Props, val.Convert())
	}

	return res, nil
}

func (*MainService) GetAllRoles(context.Context, *emptypb.Empty) (*mb.List_Roles, error) {
	defer gf.TryCatch()
	res := new(mb.List_Roles)

	roles, err := db.GetAllRoles()

	if err != nil {
		return nil, err
	}

	for _, val := range roles {
		res.Roles = append(res.Roles, val.Convert())
	}

	return res, nil
}

func (*MainService) GetAllUsers(context.Context, *emptypb.Empty) (*mb.List_Users, error) {
	defer gf.TryCatch()
	var res = new(mb.List_Users)

	users, err := db.GetAllUsers()

	if err != nil {
		return nil, err
	}

	for _, val := range users {
		res.Users = append(res.Users, val.Convert())
	}

	return res, nil
}

func (*MainService) GetAllPermissions(context.Context, *emptypb.Empty) (*mb.List_Permissions, error) {
	defer gf.TryCatch()
	var res = new(mb.List_Permissions)

	perms, err := db.GetAllPermissions()

	if err != nil {
		return nil, err
	}

	for _, val := range perms {
		res.Permissions = append(res.Permissions, val.Convert())
	}

	return res, nil
}

func (*MainService) GetAllObjectMetaData(c context.Context, req *mb.PInt) (*mb.List_ProtoObjectMetadata, error) {
	defer gf.TryCatch()
	var res = new(mb.List_ProtoObjectMetadata)

	objsMetaData, err := db.GetAllObjectMetaData(req.Val)

	if err != nil {
		return nil, err
	}

	for _, val := range objsMetaData {
		res.Metadata = append(res.Metadata, val.Convert())
	}

	return res, nil
}

func (*MainService) GetAllMetaDataTypes(context.Context, *emptypb.Empty) (*mb.List_MetaDataTypes, error) {
	defer gf.TryCatch()
	var res = new(mb.List_MetaDataTypes)

	metaDataTypes, err := db.GetAllMetaDataTypes()

	if err != nil {
		return nil, err
	}

	for _, val := range metaDataTypes {
		res.Types = append(res.Types, val.Convert())
	}

	return res, nil
}

func (*MainService) GetMetaDataOfItem(c context.Context, req *mb.PInt) (*mb.List_MetaData, error) {
	defer gf.TryCatch()
	var res = new(mb.List_MetaData)

	metaData, err := db.GetMetaDataOfItem(int(req.Val))

	if err != nil {
		return nil, err
	}

	for _, val := range metaData {
		res.Metadata = append(res.Metadata, val.Convert())
	}

	return res, nil
}

func (*MainService) UpdateRole(c context.Context, r *mb.ProtoRole) (*mb.PBool, error) {
	defer gf.TryCatch()
	var res = new(mb.PBool)
	res.Val = true
	err := db.UpdateRole(db.ConvertR(r))

	if err != nil {
		res.Val = false
		return res, err
	}

	err = db.SetRolePermissions(r.Id, r.Permissions)

	if err != nil {
		res.Val = false
	}

	return res, err
}

func (*MainService) AddItem(ctx context.Context, req *mb.ProtoItem) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	res, err := db.AddItem(*db.ConvertItem(req))
	result.Val = (int32(res))
	return result, err
}

func (*MainService) UpdateItem(ctx context.Context, req *mb.ProtoItem) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	result.Val = false
	err := db.UpdateItem(*db.ConvertItem(req))
	result.Val = err == nil
	return result, err
}

func (*MainService) RemoveItem(ctx context.Context, req *mb.ProtoItem) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.RemoveItem(req.Id)
	result.Val = err == nil
	return result, err
}

func (*MainService) AddObject(ctx context.Context, req *mb.ProtoObject) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	res, err := db.AddObject(db.ConvertOb(req))
	result.Val = (int32(res))
	return result, err
}

func (*MainService) UpdateObject(ctx context.Context, req *mb.ProtoObject) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.UpdateObject(db.ConvertOb(req))
	result.Val = err == nil
	return result, err
}

func (*MainService) RemoveObject(ctx context.Context, req *mb.ProtoObject) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.RemoveObject(req.Id)
	result.Val = err == nil
	return result, err
}

func (*MainService) AddUser(ctx context.Context, req *mb.ProtoUser) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	res, err := db.AddUser(*db.ConvertUser(req))
	result.Val = (int32(res))
	return result, err
}

func (*MainService) UpdateUser(ctx context.Context, req *mb.ProtoUser) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.UpdateUser(*db.ConvertUser(req))
	result.Val = err == nil
	return result, err
}

func (*MainService) RemoveUser(ctx context.Context, req *mb.ProtoUser) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.DeleteUser(req.Id)
	result.Val = err == nil
	return result, err
}
func (*MainService) AddRole(ctx context.Context, req *mb.ProtoRole) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	val, err := db.AddRole(*db.ConvertR(req))
	result.Val = int32(val)
	return result, err
}

func (*MainService) RemoveRole(ctx context.Context, req *mb.ProtoRole) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.DeleteRole(req.Id)
	result.Val = err == nil
	return result, err
}

func (*MainService) AddGroupingProperty(ctx context.Context, req *mb.ProtoGroupingProperty) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	val, err := db.AddGroupingProperty(*db.ConvertGr(req))
	result.Val = int32(val)
	return result, err
}

func (*MainService) AddProducer(ctx context.Context, req *mb.ProtoProducer) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	val, err := db.AddProducer(*db.ConvertPr(req))
	result.Val = int32(val)
	return result, err
}

func (*MainService) AddNameItem(ctx context.Context, req *mb.ProtoNameItem) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	val, err := db.AddNameItem(*db.ConvertN(req))
	result.Val = int32(val)
	return result, err
}

func (*MainService) AddTypeOfUnit(ctx context.Context, req *mb.ProtoTypeOfUnit) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	val, err := db.AddTypeOfUnit(*db.ConvertTou(req))
	result.Val = int32(val)
	return result, err
}

func (*MainService) AddItemMetaData(ctx context.Context, req *mb.ProtoItemMetaData) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	val, err := db.AddItemMetaData(db.ConvertItMetaDate(req))
	result.Val = int32(val)
	return result, err
}

func (*MainService) AddObjectMetaData(ctx context.Context, req *mb.ProtoObjectMetadata) (*mb.PInt, error) {
	defer gf.TryCatch()
	result := new(mb.PInt)
	result.Val = -1
	val, err := db.AddObjectMetaData(db.ConvertObjMetaData(req))
	result.Val = int32(val)
	return result, err
}

func (*MainService) AddGroupingPropertyOfItem(ctx context.Context, req *mb.ChangeGroupingPropertyofItem) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.AddGroupingPropertyOfItem(int(req.ItemId), *db.ConvertGr(req.Prop))
	result.Val = err == nil
	return result, err
}

func (*MainService) RemoveGroupingPropertyOfItem(ctx context.Context, req *mb.ChangeGroupingPropertyofItem) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.RemoveGroupingPropertyOfItem(int(req.ItemId), *db.ConvertGr(req.Prop))
	result.Val = err == nil
	return result, err
}

func (*MainService) RemoveItemMetaData(ctx context.Context, req *mb.ProtoItemMetaData) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.RemoveItemMetaData(int(req.Id))
	result.Val = err == nil
	return result, err
}

func (*MainService) RemoveObjectMetadata(ctx context.Context, req *mb.ProtoObjectMetadata) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.RemoveObjectMetadata(int(req.Id))
	result.Val = err == nil
	return result, err
}

func (*MainService) SetGroupingPropertiesOfItem(ctx context.Context, req *mb.ChangeGroupingPropertiesofItem) (*mb.PBool, error) {
	defer gf.TryCatch()
	result := new(mb.PBool)
	err := db.SetGroupingPropertiesOfItem(int(req.ItemId), gf.Select(gf.From(req.Props.GetProps()), func(prop *mb.ProtoGroupingProperty) db.GroupingProp { return *db.ConvertGr(prop) }).ToList())
	result.Val = err == nil
	return result, err
}

func (*MainService) GetBackupStatus(ctx context.Context, req *emptypb.Empty) (*mb.ProtoBackupStatusResponse, error) {
	defer gf.TryCatch()
	response := new(mb.ProtoBackupStatusResponse)
	return response, nil
}

var backupMutex sync.Mutex

var IsBackupInProcess atomic.Bool

func (*MainService) StartBackup(req *emptypb.Empty, stream grpc.ServerStreamingServer[mb.ProtoBackupChunk]) error {
	defer gf.TryCatch()
	ctx := stream.Context()
	md, ok := metadata.FromIncomingContext(ctx)
	if !ok {
		return status.Error(codes.Unauthenticated, "missing metadata")
	}

	id := md["id"]

	var sessionData *sd.SessionData = nil
	if len(id) == 0 {
		return status.Error(codes.Unauthenticated, "missing id header")
	} else {
		sessionData = sd.GetUserData(id[0])
	}

	backupMutex.Lock()

	if IsBackupInProcess.Load() {
		errMsg := "Бэкап уже выполняется. Запрос отклонён."
		backupMutex.Unlock()
		if sessionData != nil {
			sessionData.ErrorMessage = errMsg
			sessionData.IsBackupDone = false
			sessionData.BackupFilePath = ""
		}
	}

	IsBackupInProcess.Store(true)
	backupMutex.Unlock()
	defer func() {
		IsBackupInProcess.Store(false)
	}()

	if sessionData != nil {
		sessionData.ErrorMessage = ""
		sessionData.IsBackupDone = false
		sessionData.BackupFilePath = ""
	}

	backupID := uuid.New().String()
	backupFilePath := filepath.Join(db.BackupFilePath, fmt.Sprintf("%s.backup", backupID))

	sessionData.BackupFilePath = backupFilePath

	lg.Logger.Info("Начато создание бэкапа базы")

	cmd := exec.Command(
		db.PgDumpPath,
		"-h", db.DbHost,
		"-p", db.DbPort,
		"-U", db.DbUser,
		"-d", db.DbName,
		"-F", "c",
		"-f", backupFilePath,
	)

	cmd.Env = append(os.Environ(), fmt.Sprintf("PGPASSWORD=%s", db.DbPassword))

	if err := cmd.Run(); err != nil {
		lg.Logger.Error("Error occured while exec pg_dump: ", zap.Error(err))
		if sessionData != nil {
			sessionData.ErrorMessage = err.Error()
			sessionData.IsBackupDone = false
		}

		if _, statErr := os.Stat(backupFilePath); statErr == nil {
			_ = os.Remove(backupFilePath)
		}
		return status.Errorf(codes.Internal, "ошибка бэкапа: %v", err)
	}

	defer func() {
		if _, err := os.Stat(backupFilePath); err == nil {
			if removeErr := os.Remove(backupFilePath); removeErr != nil {
				lg.Logger.Error(fmt.Sprintf("Ошибка при удалении временного файла %s: %v", backupFilePath, removeErr))
			} else {
				lg.Logger.Info("Временный файл удален")
			}
		}
	}()

	lg.Logger.Info(fmt.Sprintf("Бэкап создан: %s", backupFilePath))

	const chunkSize = 8192
	file, err := os.Open(backupFilePath)

	if err != nil {
		lg.Logger.Error("Ошибка при открытии файла: ", zap.Error(err))
		return status.Errorf(codes.Internal, "Ошибка открытия файла: %v", err)
	}
	defer file.Close()

	buffer := make([]byte, chunkSize)

	for {
		n, readErr := file.Read(buffer)
		if readErr != nil && readErr != io.EOF {
			lg.Logger.Error("Ошибка при чтении файла: ", zap.Error(readErr))
			return status.Errorf(codes.Internal, "Ошибка чтения: %v", readErr)
		}

		select {
		case <-ctx.Done():
			lg.Logger.Info("Бэкап отменён клиентом.")
			if sessionData != nil {
				sessionData.ErrorMessage = "Бэкап отменён клиентом."
				sessionData.IsBackupDone = false
			}
			return nil
		default:
		}

		if n > 0 {
			chunk := &mb.ProtoBackupChunk{
				Data:   buffer[:n],
				IsLast: false,
			}

			if err := stream.Send(chunk); err != nil {
				lg.Logger.Error("Ошибка при передаче: ", zap.Error(err))
				return status.Errorf(codes.Aborted, "Ошибка при передаче: %v", err)
			}
		}

		if readErr == io.EOF {
			break
		}
	}

	if err := stream.Send(&mb.ProtoBackupChunk{
		Data:   []byte{},
		IsLast: true,
	}); err != nil {
		lg.Logger.Error(fmt.Sprintf("Ошибка отправки финального чанка: %v", err))
		return status.Errorf(codes.Aborted, "Ошибка отправки финального чанка: %v", err)
	}

	// Успешное завершение
	if sessionData != nil {
		sessionData.IsBackupDone = true
		sessionData.ErrorMessage = ""
	}

	lg.Logger.Info("Успешный бэкап базы")

	return nil
}
