using AccountingSystemService;
using AccountingSystemService.DataCollections;
using AccountingSystemService.Interfaces;

using BdClasses;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.AspNetCore.Authorization;

namespace AccountingSystemService.Services
{
    [Authorize]
    public class AccountingService : AccountingSystem.AccountingSystemBase
    {
        private IErrorHandler ErrorHandler { get; set; }
        private UsersCollection usrColl { get; }
        private ObjectCollection objColl { get; }
        private ConstructionContext db { get; set; }
        public AccountingService(IErrorHandler errorHandler, UsersCollection usersCollection, ObjectCollection objectCollection, ConstructionContext db)
        {
            ErrorHandler = errorHandler;
            usrColl = usersCollection;
            objColl = objectCollection;
            this.db = db;
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
                result.Val = objColl.AddGroupingPropertyToItem(new(request.Prop), request.ItemId);
                ErrorHandler.HandleError($"Добавлено свойство группировки у записи {request.ItemId} : {request.Prop.Name} ", Severity.Information);
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
                result.Val = objColl.AddItem(new(request));
                ErrorHandler.HandleError($"Добавлена запись {request.NameItem.Name}", Severity.Information);
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
                result.Val = objColl.AddName(request.Name);
                ErrorHandler.HandleError($"Добавлено наименование {request.Name}", Severity.Information);
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
                result.Val = objColl.AddObject(request.Name, request.Address, request.Description);
                ErrorHandler.HandleError($"Добавлен объект {request.Name}", Severity.Information);
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
                result.Val = objColl.AddProducer(request.Name);
                ErrorHandler.HandleError($"Добавлен производитель {request.Name}", Severity.Information);
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
                result.Val = usrColl.AddRole(request.Name, request.Description);
                ErrorHandler.HandleError($"Добавлена роль {request.Name}", Severity.Information);
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
                result.Val = objColl.AddTypeOfUnit(request.Name);
                ErrorHandler.HandleError($"Добавлена единица измерения {request.Name}", Severity.Information);
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
                result.Val = usrColl.AddUser(request.Name,request.Password,request.Description);
                ErrorHandler.HandleError($"Пользователь {request.Name} добавлен", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении пользователя {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
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

        [Authorize(Roles = "15")]
        public override Task<PBool> RemoveGroupingPropertyOfItem(ChangeGroupingPropertyofItem request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                result.Val = objColl.RemoveGroupingPropertyOfItem(new(request.Prop), request.ItemId);
                ErrorHandler.HandleError($"Свойство группировки удалено из записи {request.ItemId} удалено", Severity.Information);
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
                result.Val = objColl.RemoveItem(request.Id, request.NameItem.Name);
                ErrorHandler.HandleError($"Запись {request.Id} удалена", Severity.Information);
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
                var metadata = db.ItemMetaData.FirstOrDefault(x => x.Id == request.Id);
                if (metadata != null)
                {
                    db.ItemMetaData.Remove(metadata);
                    db.SaveChanges();
                    result.Val = true;
                    ErrorHandler.HandleError($"Метаданные {request.Name} удалены", Severity.Information);
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
                result.Val = objColl.RemoveObject(request.Id,request.Name);
                ErrorHandler.HandleError($"Объект {request.Name} удален", Severity.Information);
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
                result.Val = usrColl.RemoveRole(request.Id,request.Name);
                ErrorHandler.HandleError($"Роль {request.Name} удалена", Severity.Information);
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
                result.Val = usrColl.RemoveUser(request.Id,request.Name);
                ErrorHandler.HandleError($"Пользователь {request.Name} удален", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении пользователя {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
        }

        [Authorize(Roles = "15")]
        public override Task<PBool> UpdateItem(ProtoItem request, ServerCallContext context)
        {
            PBool result = new() { Val = false };
            try
            {
                result.Val = objColl.UpdateItem(new(request));
                ErrorHandler.HandleError($"Запись {request.Id} обновлена", Severity.Information);
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
                result.Val = objColl.UpdateObject(new(request));
                ErrorHandler.HandleError($"Объект {request.Name} обновлен", Severity.Information);
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
                result.Val = usrColl.UpdateRole(new(request));
                ErrorHandler.HandleError($"Роль {request.Name} обновлена", Severity.Information);
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
                result.Val = usrColl.UpdateUser(new(request));

                ErrorHandler.HandleError($"Пользователь {request.Name} обновлен", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при обновлении пользователя {request.Id} : {request.Name} -> {e.Message}", Severity.Error);
            }
            return Task.FromResult(result);
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
    }
}
