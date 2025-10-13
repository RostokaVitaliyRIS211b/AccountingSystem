using AccountingSystemService;
using AccountingSystemService.DataCollections;
using AccountingSystemService.Helpers;
using AccountingSystemService.Interfaces;
using AccountingSystemService.Wrappers;

using BdClasses;

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel;
using System.Diagnostics;

namespace AccountingSystemService.Services
{
    [Authorize]
    public class AccountingService : AccountingSystem.AccountingSystemBase
    {
        private IErrorHandler ErrorHandler { get; set; }
        private UsersCollection usrColl { get; }
        private ObjectCollection objColl { get; }
        private ConstructionContext db { get; set; }

        private static object LockBackupTryObj { get; } = new();

        private static object LockObjectMetaData { get; } = new();

        private static object LockItemGroupingProperty { get; } = new();

        private static object LockItem { get; } = new();

        private static object LockItemMetaData { get; } = new();

        private static object LockConObject { get; } = new();

        private static object LockNameItem { get; } = new();

        private static object LockProducer { get; } = new();

        private static object LockTypeUnit { get; } = new();

        private static object LockRole { get; } = new();

        private static object LockUser { get; } = new();

        private static string ErrorMessage { get; set; } = "";

        private static bool IsBackupDone { get; set; } = false;

        private static string BackupFilePath { get; set; } = "";

        private static bool IsBackupInProcess { get; set; } = false;

        public AccountingService(IErrorHandler errorHandler, UsersCollection usersCollection, ObjectCollection objectCollection, ConstructionContext db)
        {
            ErrorHandler = errorHandler;
            usrColl = usersCollection;
            objColl = objectCollection;
            this.db = db;
        }

        #region Adders

        [Authorize(Roles = "37")]
        public override Task<PInt> AddObjectMetaData(ProtoObjectMetadata request, ServerCallContext context)
        {
            PInt result = new PInt() { Val = -1 };
            try
            {
                lock (LockObjectMetaData)
                {
                    var metaData = new ObjectMetaDatum()
                    {
                        Data = request.Data.ToByteArray(),
                        DataTypeId = request.TypeId,
                        Name = request.Name,
                        ObjectId = request.ObjId,
                    };

                    var db = DbContextHelper.GetConstructionContext();
                    db.ObjectMetaData.Add(metaData);
                    db.SaveChanges();

                    result.Val = metaData.Id;

                    ErrorHandler.HandleError($"Добавлены метаданные объекту {request.ObjId}", Severity.Information);
                }
            }
            catch (Exception e)
            {
                result.Val = -1;
                ErrorHandler.HandleError($"Ошибка при добавлении метаданных объекту {request.Id} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "27")]
        public override Task<PInt> AddGroupingProperty(ProtoGroupingProperty request, ServerCallContext context)
        {
            PInt result = new() { Val = -1 };
            try
            {
                result.Val = objColl.AddGroupingProperty(request.Name);
                ErrorHandler.HandleError($"Добавлено свойство группировки {request.Name}", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении свойства группировки {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "15")]
        public override Task<PBool> AddGroupingPropertyOfItem(ChangeGroupingPropertyofItem request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockItemGroupingProperty)
                {
                    result.Val = objColl.AddGroupingPropertyToItem(new(request.Prop), request.ItemId);
                    ErrorHandler.HandleError($"Добавлено свойство группировки у записи {request.ItemId} : {request.Prop.Name} ", Severity.Information);
                }

            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении свойства группировки записи {request.ItemId}  {request.Prop.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "13")]
        public override Task<PInt> AddItem(ProtoItem request, ServerCallContext context)
        {
            PInt result = new() { Val = -1 };
            try
            {
                lock (LockItem)
                {
                    result.Val = objColl.AddItem(new(request));
                    ErrorHandler.HandleError($"Добавлена запись {request.NameItem.Name}", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении записи -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "25")]
        public override Task<PInt> AddItemMetaData(ProtoItemMetaData request, ServerCallContext context)
        {
            PInt ret = new() { Val = -1 };
            try
            {
                lock (LockItemMetaData)
                {
                    var metaData = new ItemMetaDatum();
                    metaData.Data = request.Data.ToByteArray();
                    metaData.DataTypeId = request.TypeId;
                    metaData.ItemId = request.ItemId;
                    metaData.Name = request.Name;

                    db.ItemMetaData.Add(metaData);
                    db.SaveChanges();

                    ret.Val = metaData.Id;

                    ErrorHandler.HandleError($"Добавлены метаданные для записи {request.ItemId}", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении метаданных записи {request.Id} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(ret);
        }

        [Authorize(Roles = "29")]
        public override Task<PInt> AddNameItem(ProtoNameItem request, ServerCallContext context)
        {
            PInt result = new() { Val = -1 };
            try
            {
                lock (LockNameItem)
                {
                    result.Val = objColl.AddName(request.Name);
                    ErrorHandler.HandleError($"Добавлено наименование {request.Name}", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении наименования записи {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "9")]
        public override Task<PInt> AddObject(ProtoObject request, ServerCallContext context)
        {
            PInt result = new() { Val = -1 };
            try
            {
                lock (LockConObject)
                {
                    result.Val = objColl.AddObject(request.Name, request.Address, request.Description);
                    ErrorHandler.HandleError($"Добавлен объект {request.Name}", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении объекта {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "31")]
        public override Task<PInt> AddProducer(ProtoProducer request, ServerCallContext context)
        {
            PInt result = new() { Val = -1 };
            try
            {
                lock (LockProducer)
                {
                    result.Val = objColl.AddProducer(request.Name);
                    ErrorHandler.HandleError($"Добавлен производитель {request.Name}", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении производителя {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "16")]
        public override Task<PBool> AddRecordToJournal(ProtoJournalRecord request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                var j = new Journal();
                j.Text = request.Text;

                db.Journals.Add(j);
                db.SaveChanges();

                result.Val = true;

                ErrorHandler.HandleError($"Добавлена запись в журнал {request.Text}", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении записи в журнал {request.Text} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "4")]
        public override Task<PInt> AddRole(ProtoRole request, ServerCallContext context)
        {
            PInt result = new() { Val = -1 };
            try
            {
                lock (LockRole)
                {
                    result.Val = usrColl.AddRole(request.Name, request.Description);
                    ErrorHandler.HandleError($"Добавлена роль {request.Name}", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении роли {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "33")]
        public override Task<PInt> AddTypeOfUnit(ProtoTypeOfUnit request, ServerCallContext context)
        {
            PInt result = new() { Val = -1 };
            try
            {
                lock (LockTypeUnit)
                {
                    result.Val = objColl.AddTypeOfUnit(request.Name);
                    ErrorHandler.HandleError($"Добавлена единица измерения {request.Name}", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении единицы измерения {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "1")]
        public override Task<PInt> AddUser(ProtoUser request, ServerCallContext context)
        {
            PInt result = new() { Val = -1 };
            try
            {
                lock (LockUser)
                {
                    result.Val = usrColl.AddUser(request.Name, request.Password, request.Description);
                    ErrorHandler.HandleError($"Пользователь {request.Name} добавлен", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении пользователя {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        #endregion

        #region Getters

        public override Task<List_MetaDataTypes> GetAllMetaDataTypes(Empty request, ServerCallContext context)
        {
            List_MetaDataTypes dataTypes = new();
            try
            {
                var db = DbContextHelper.GetConstructionContext();
                foreach (var type in db.TypesOfMetaData.ToList())
                {
                    dataTypes.Types_.Add(new ProtoMetaDataType() { Id = type.Id, Name = type.Name });
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении всех типов метаданных {e.Message}", Severity.Error);
            }
            return Task.FromResult(dataTypes);
        }

        [Authorize(Roles = "36")]
        public override Task<List_ProtoObjectMetadata> GetAllObjectMetaData(PInt request, ServerCallContext context)
        {
            var result = new List_ProtoObjectMetadata();
            try
            {
                var db = DbContextHelper.GetConstructionContext();
                var metadata = db.ObjectMetaData.AsNoTracking().Where(x => x.ObjectId == request.Val).ToList();
                foreach (var meta in metadata)
                {
                    var met = new ObjectMetaDataWrapper()
                    {
                        Id = meta.Id,
                        Name = meta.Name,
                        TypeId = meta.DataTypeId,
                        ObjId = meta.ObjectId,
                        Data = meta.Data
                    };
                    result.Metadata.Add(met.ProtoObject);
                }

                ErrorHandler.HandleError($"Метаданные объекта {request.Val} получены", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении метаданных объекта {request.Val} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        public override Task<List_TypeOfItems> GetAllTypesOfItems(Empty request, ServerCallContext context)
        {
            List_TypeOfItems res = new();
            try
            {
                var types = db.TypeOfItems.AsNoTracking().ToList();
                foreach (var type in types)
                {
                    var pType = new TypeOfItemWrapper();
                    pType.Name = type.Name;
                    pType.Id = type.Id;
                    res.Types_.Add(pType.ProtoObject);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении списка типов записей -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(res);

        }

        [Authorize(Roles = "35")]
        public override Task<List_Permissions> GetAllPermissions(Empty request, ServerCallContext context)
        {
            var res = new List_Permissions();
            try
            {
                foreach (var item in usrColl.GetAllPermissions())
                {
                    res.Permissions.Add(item);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении списка разрешений -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(res);
        }

        [Authorize(Roles = "26")]
        public override Task<List_GroupingProps> GetAllGroupingProps(Empty request, ServerCallContext context)
        {
            List_GroupingProps list = new();
            try
            {
                foreach (var item in objColl.GetAllGroupingProperties())
                {
                    list.Props.Add(item);
                }
                ErrorHandler.HandleError($"Получены все свойства группировки", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении всех свойств группировки -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        [Authorize(Roles = "28")]
        public override Task<List_NameItems> GetAllNames(Empty request, ServerCallContext context)
        {
            List_NameItems list = new();
            try
            {
                foreach (var item in objColl.GetAllNameItems())
                {
                    list.Names.Add(item);
                }
                ErrorHandler.HandleError($"Получены все наименования", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении всех наименований -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        [Authorize(Roles = "23")]
        public override Task<List_Objects> GetAllObjects(Empty request, ServerCallContext context)
        {
            List_Objects list = new();
            try
            {
                foreach (var item in objColl.GetAllObjects())
                {
                    list.Objs.Add(item);
                }
                ErrorHandler.HandleError($"Получены все объекты", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении всех объектов -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        [Authorize(Roles = "30")]
        public override Task<List_Producers> GetAllProducers(Empty request, ServerCallContext context)
        {
            List_Producers list = new();
            try
            {
                foreach (var item in objColl.GetAllProducers())
                {
                    list.Producers.Add(item);
                }
                ErrorHandler.HandleError($"Получены все производители", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении всех производителей -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        [Authorize(Roles = "21")]
        public override Task<List_Roles> GetAllRoles(Empty request, ServerCallContext context)
        {
            List_Roles list = new();
            try
            {
                foreach (var item in usrColl.GetAllRoles())
                {
                    list.Roles.Add(item);
                }
                ErrorHandler.HandleError($"Получены все роли", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении всех ролей -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        [Authorize(Roles = "32")]
        public override Task<List_TypesOfUnit> GetAllTypesOfUnit(Empty request, ServerCallContext context)
        {
            List_TypesOfUnit list = new();
            try
            {
                foreach (var item in objColl.GetAllTypesOfUnits())
                {
                    list.Types_.Add(item);
                }
                ErrorHandler.HandleError($"Получены все единицы измерения", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении всех единиц измерения -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        [Authorize(Roles = "20")]
        public override Task<List_Users> GetAllUsers(Empty request, ServerCallContext context)
        {
            List_Users list = new();
            try
            {
                foreach (var item in usrColl.GetAllUsers())
                {
                    list.Users.Add(item);
                }
                ErrorHandler.HandleError($"Получены все пользователи", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении всех пользователей -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        [Authorize(Roles = "20")]
        public override Task<List_GroupingProps> GetGroupingPropsByItem(PInt request, ServerCallContext context)
        {
            List_GroupingProps list = new();
            try
            {
                foreach (var item in objColl.GetPropertiesOfItem(request.Val))
                {
                    list.Props.Add(item);
                }
                ErrorHandler.HandleError($"Получены свойства группировки записи {request.Val}", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении свойств группировки записи {request.Val} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        [Authorize(Roles = "19")]
        public override Task<List_Items> GetItemsByObject(PInt request, ServerCallContext context)
        {
            List_Items list = new();
            try
            {
                foreach (var item in objColl.GetItemsByObject(request.Val))
                {
                    list.Items.Add(item);
                }
                ErrorHandler.HandleError($"Загружены записи объекта {request.Val}", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении всех записей объекта {request.Val} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        [Authorize(Roles = "22")]
        public override Task<ProtoJournal> GetJournal(Empty request, ServerCallContext context)
        {
            ProtoJournal jour = new();
            try
            {
                var jo = db.Journals.ToList();
                foreach (var item in jo)
                {
                    var Ppj = new ProtoJournalRecord();
                    Ppj.Id = item.Id;
                    Ppj.Text = item.Text;
                    jour.Records.Add(Ppj);
                }
                ErrorHandler.HandleError($"Журнал загружен", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении журнала -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(jour);
        }

        [Authorize(Roles = "24")]
        public override Task<List_MetaData> GetMetaDataOfItem(PInt request, ServerCallContext context)
        {
            List_MetaData list = new();
            try
            {
                foreach (var item in objColl.GetItemMetaData(request.Val))
                {
                    list.Metadata.Add(item);
                }
                ErrorHandler.HandleError($"Метаданные записи {request.Val} загружены", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении метаданных записи {request.Val} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(list);
        }

        #endregion

        #region Removers

        [Authorize(Roles = "37")]
        public override Task<PBool> RemoveObjectMetadata(ProtoObjectMetadata request, ServerCallContext context)
        {
            PBool p = new() { Val = false };
            try
            {
                lock (LockObjectMetaData)
                {
                    var db = DbContextHelper.GetConstructionContext();
                    var metadata = db.ObjectMetaData.FirstOrDefault(x => x.Id == request.Id);
                    if (metadata != null)
                    {
                        db.ObjectMetaData.Remove(metadata);
                        db.SaveChanges();
                        p.Val = true;

                        ErrorHandler.HandleError($"Метаданные {request.Name} объекта {request.Id} удалены", Severity.Information);
                    }
                }

            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении метаданных объекта {request.ObjId} / {request.Id} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(p);
        }

        [Authorize(Roles = "15")]
        public override Task<PBool> RemoveGroupingPropertyOfItem(ChangeGroupingPropertyofItem request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockItemGroupingProperty)
                {
                    result.Val = objColl.RemoveGroupingPropertyOfItem(new(request.Prop), request.ItemId);
                    ErrorHandler.HandleError($"Свойство группировки удалено из записи {request.ItemId} удалено", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении свойства группировки у записи {request.ItemId} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "14")]
        public override Task<PBool> RemoveItem(ProtoItem request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockItem)
                {
                    result.Val = objColl.RemoveItem(request.Id, request.NameItem.Name);
                    ErrorHandler.HandleError($"Запись {request.Id} удалена", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении записи {request.Id} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "25")]
        public override Task<PBool> RemoveItemMetaData(ProtoItemMetaData request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockItemMetaData)
                {
                    var metadata = db.ItemMetaData.FirstOrDefault(x => x.Id == request.Id);
                    if (metadata != null)
                    {
                        db.ItemMetaData.Remove(metadata);
                        db.SaveChanges();
                        result.Val = true;
                        ErrorHandler.HandleError($"Метаданные {request.Name} удалены", Severity.Information);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении записи {request.ItemId} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "10")]
        public override Task<PBool> RemoveObject(ProtoObject request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockConObject)
                {
                    result.Val = objColl.RemoveObject(request.Id, request.Name);
                    ErrorHandler.HandleError($"Объект {request.Name} удален", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении объекта {request.Name}  {request.Id} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "5")]
        public override Task<PBool> RemoveRole(ProtoRole request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockRole)
                {
                    result.Val = usrColl.RemoveRole(request.Id, request.Name);
                    ErrorHandler.HandleError($"Роль {request.Name} удалена", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении роли {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "2")]
        public override Task<PBool> RemoveUser(ProtoUser request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockUser)
                {
                    result.Val = usrColl.RemoveUser(request.Id, request.Name);
                    ErrorHandler.HandleError($"Пользователь {request.Name} удален", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении пользователя {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        #endregion

        #region Updaters

        [Authorize(Roles = "15")]
        public override Task<PBool> UpdateItem(ProtoItem request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockItem)
                {
                    result.Val = objColl.UpdateItem(new(request));
                    ErrorHandler.HandleError($"Запись {request.Id} обновлена", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка обновления записи {request.Id} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "11")]
        public override Task<PBool> UpdateObject(ProtoObject request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockConObject)
                {
                    result.Val = objColl.UpdateObject(new(request));
                    ErrorHandler.HandleError($"Объект {request.Name} обновлен", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при обновлении объекта {request.Id} : {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "6")]
        public override Task<PBool> UpdateRole(ProtoRole request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockRole)
                {
                    result.Val = usrColl.UpdateRole(new(request));
                    ErrorHandler.HandleError($"Роль {request.Name} обновлена", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при обновлении роли {request.Id} : {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "3")]
        public override Task<PBool> UpdateUser(ProtoUser request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                lock (LockUser)
                {
                    result.Val = usrColl.UpdateUser(new(request));
                    ErrorHandler.HandleError($"Пользователь {request.Name} обновлен", Severity.Information);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при обновлении пользователя {request.Id} : {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        #endregion

        #region Other

        public override Task<Empty> CheckActive(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        [Authorize(Roles = "15")]
        public override Task<PBool> SetGroupingPropertiesOfItem(ChangeGroupingPropertiesofItem request, ServerCallContext context)
        {
            PBool result = new() { Val = false };

            try
            {
                lock (LockItemGroupingProperty)
                {
                    var db = DbContextHelper.GetConstructionContext();
                    var propsOfItem = db.GroupingPropertiesForItems.AsNoTracking().Where(x => x.ItemId == request.ItemId).ToList();
                    foreach (var prop in propsOfItem)
                    {
                        db.GroupingPropertiesForItems.Remove(prop);
                    }

                    db.SaveChanges();

                    ErrorHandler.HandleError($"Свойства группировки записи {request.ItemId}  удалены", Severity.Information);

                    foreach (var prop in request.Props.Props)
                    {
                        var gg = new GroupingPropertiesForItem();
                        gg.PropId = prop.Id;
                        gg.ItemId = request.ItemId;
                        db.GroupingPropertiesForItems.Add(gg);
                    }

                    db.SaveChanges();

                    ErrorHandler.HandleError($"Свойства группировки записи {request.ItemId} добавлены", Severity.Information);

                    result.Val = true;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при изменении свойств группировки записи -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "38")]
        public override async Task StartBackup(Empty request, IServerStreamWriter<ProtoBackupChunk> responseStream, ServerCallContext context)
        {
            var id = context.RequestHeaders.Get("Id")?.Value;
            var sessionData = SessionDataManager.GetUserData(id ?? "");
            try
            {

                lock (LockBackupTryObj)
                {
                    if (IsBackupInProcess)
                    {
                        ErrorHandler.HandleError($"Бэкап уже выполняется. Запрос отклонён.", Severity.Warning);
                        if (sessionData is not null)
                        {
                            sessionData.ErrorMessage = "Бэкап уже выполняется. Запрос отклонён.";
                            sessionData.IsBackupDone = false;
                            sessionData.BackupFilePath = "";
                        }
                        else
                        {
                            ErrorMessage = "Бэкап уже выполняется. Запрос отклонён.";
                        }
                        return;
                    }

                    BackupFilePath = "";
                    ErrorMessage = "";
                    IsBackupDone = false;
                    IsBackupInProcess = true;

                    if (sessionData is not null)
                    {
                        sessionData.ErrorMessage = "";
                        sessionData.IsBackupDone = false;
                        sessionData.BackupFilePath = "";
                    }
                }

                var backupId = Guid.NewGuid();
                var backupFilePath = Path.Combine(DbContextHelper.BackupDir, $"{backupId}.backup");

                ErrorHandler.HandleError($"Начато создание бэкапа базы", Severity.Information);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = DbContextHelper.PgDumpPath,
                        Arguments = $"-h localhost -p {DbContextHelper.DatabasePort} -U {DbContextHelper.Username} -d {DbContextHelper.DatabaseName} -F c -f \"{backupFilePath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        EnvironmentVariables =
                        {
                            ["PGPASSWORD"] = DbContextHelper.Password
                        }

                    }
                };

                process.Start();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    throw new InvalidOperationException($"pg_dump failed with exit code {process.ExitCode}: {error}");
                }

                BackupFilePath = backupFilePath;

                if (sessionData is not null)
                {
                    sessionData.ErrorMessage = "";
                    sessionData.IsBackupDone = false;
                    sessionData.BackupFilePath = backupFilePath;
                }

                ErrorHandler.HandleError($"Бэкап создан {backupFilePath}", Severity.Information);


                using var fileStream = File.OpenRead(backupFilePath);
                var buffer = new byte[8192];
                int bytesRead = 0;

                while ((bytesRead = await fileStream.ReadAsync(buffer, context.CancellationToken)) > 0)
                {

                    await responseStream.WriteAsync(new ProtoBackupChunk
                    {
                        Data = ByteString.CopyFrom(buffer, 0, bytesRead),
                        IsLast = false
                    }, context.CancellationToken);
                }


                await responseStream.WriteAsync(new ProtoBackupChunk
                {
                    Data = ByteString.Empty,
                    IsLast = true
                }, context.CancellationToken);


                IsBackupInProcess = false;

                if (sessionData is not null)
                {
                    sessionData.ErrorMessage = "";
                    sessionData.IsBackupDone = true;
                }
                else
                {
                    IsBackupDone = true;
                }

                ErrorHandler.HandleError($"Успешный бэкап базы", Severity.Information);
            }
            catch (OperationCanceledException)
            {
                ErrorMessage = "Бэкап отменён клиентом.";
                ErrorHandler.HandleError("Бэкап отменён клиентом.", Severity.Information);
            }
            catch (Exception e)
            {
                if (sessionData is not null)
                {
                    sessionData.IsBackupDone = false;
                    sessionData.ErrorMessage = e.Message;
                }
                else
                {
                    IsBackupDone = false;
                    ErrorMessage = e.Message;
                }

                ErrorHandler.HandleError($"Ошибка при бэкапе базы -> {e.Message}", Severity.Error);
            }
            finally
            {
                lock (LockBackupTryObj)
                {
                    var backupfilePath = "";

                    backupfilePath = sessionData is not null ? sessionData.BackupFilePath : BackupFilePath;

                    ErrorMessage = "";
                    if (!string.IsNullOrEmpty(backupfilePath) && File.Exists(backupfilePath))
                    {
                        try
                        {
                            File.Delete(backupfilePath);

                            ErrorHandler.HandleError($"Временный файл удален", Severity.Information);
                        }
                        catch (Exception e)
                        {
                            ErrorHandler.HandleError($"Ошибка при удалении временного файла -> {e.Message}", Severity.Error);
                        }
                    }

                    if (sessionData is not null)
                    {
                        sessionData.BackupFilePath = "";
                    }
                    else
                    {
                        BackupFilePath = "";
                    }

                    IsBackupInProcess = false;
                }

            }

        }

        [Authorize(Roles = "38")]
        public override Task<ProtoBackupStatusResponse> GetBackupStatus(Empty request, ServerCallContext context)
        {
            var id = context.RequestHeaders.Get("Id")?.Value;
            var sessionData = SessionDataManager.GetUserData(id ?? "");

            lock (LockBackupTryObj)
            {


                ProtoBackupStatusResponse status = new()
                {
                    ErrorMessage = ErrorMessage,
                    IsDone = IsBackupDone,
                    FileSizeIn8KbChunks = 0,
                    IsInProcess = IsBackupInProcess
                };

                var backupfilePath = "";

                if (sessionData is not null)
                {
                    backupfilePath = sessionData.BackupFilePath;
                    status.ErrorMessage = sessionData.ErrorMessage;
                    status.IsDone = sessionData.IsBackupDone;
                }
                else
                {
                    backupfilePath = BackupFilePath;
                }

                if (File.Exists(backupfilePath))
                {
                    var fileInfo = new FileInfo(backupfilePath);
                    int size = 0;
                    try
                    {
                        size = (int)(fileInfo.Length / 8192);
                    }
                    catch
                    {

                    }
                    status.FileSizeIn8KbChunks = size;
                }
                return Task.FromResult(status);
            }
        }

        public override Task<Empty> AbortSessionData(Empty request, ServerCallContext context)
        {
            var id = context.RequestHeaders.Get("Id")?.Value;
            try
            {
                if (id is not null && SessionDataManager.TryRemoveUser(id))
                {
                    ErrorHandler.HandleError($"Данные сессии пользователя успешно удалены", Severity.Information);
                }
                else
                {
                    ErrorHandler.HandleError($"Данные сессии пользователя не удалены", Severity.Warning);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении данных сессии пользователя {id} -> {e.Message} ", Severity.Error);
            }
            return Task.FromResult(new Empty());
        }

        public override Task<PBool> AddSessionData(Empty request, ServerCallContext context)
        {
            var id = context.RequestHeaders.Get("Id")?.Value;
            var result = new PBool() { Val = false };
            try
            {

                if (id is not null)
                {
                    result.Val = SessionDataManager.TryAddUser(id);
                    if (result.Val)
                    {
                        ErrorHandler.HandleError($"Данные сессии для пользователя были добавлены", Severity.Information);
                    }
                    else
                    {
                        ErrorHandler.HandleError($"Данные сессии для пользователя не были добавлены", Severity.Warning);
                    }
                }

            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении данных сессии пользователя {id} -> {e.Message} ", Severity.Error);
            }
            return Task.FromResult(result);
        }

        #endregion
    }
}
