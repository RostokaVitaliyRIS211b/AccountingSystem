using AccountingSystemService.Helpers;
using AccountingSystemService.Interfaces;
using AccountingSystemService.Wrappers;

using BdClasses;

using System.Linq;

namespace AccountingSystemService.DataCollections
{
    public class ObjectCollection
    {
        public ObjectCollection(IErrorHandler errorHandler)
        {
            ErrorHandler = errorHandler;

            ErrorHandler.HandleError($"Начата загрузка основных данных", Severity.Information);
            try
            {


                using var db = DbContextHelper.GetConstructionContext();

                var objects = db.Objects.ToList();
                var types = db.TypeOfItems.ToList();
                var typesofUnit = db.TypesOfUnits.ToList();
                var producers = db.Producers.ToList();
                var groupingProperties = db.GroupingProperties.ToList();
                var namesOfItems = db.NameItems.ToList();
                var items = db.Items.ToList();


                ErrorHandler.HandleError($"Основные данные загружены из базы", Severity.Error);

                foreach (var item in objects)
                {
                    var oWrapper = new ObjectWrapper();
                    oWrapper.Name = item.Name;
                    oWrapper.Description = item.Description ?? "";
                    oWrapper.Address = item.Address;
                    oWrapper.Id = item.Id;
                    Objects.Add(oWrapper);
                }

                foreach (var item in types)
                {
                    var tWrapper = new TypeOfItemWrapper();
                    tWrapper.Name = item.Name;
                    tWrapper.Id = item.Id;
                    TypesOfItems.Add(tWrapper);
                }

                foreach (var item in typesofUnit)
                {
                    var tWrapper = new TypeOfUnitWrapper();
                    tWrapper.Name = item.Name;
                    tWrapper.Id = item.Id;
                    TypesOfUnits.Add(tWrapper);
                }

                foreach (var item in producers)
                {
                    var pWrapper = new ProducerWrapper();
                    pWrapper.Name = item.Name;
                    pWrapper.Id = item.Id;
                    Producers.Add(pWrapper);
                }

                foreach (var item in groupingProperties)
                {
                    var gWrapper = new GroupingPropertyWrapper();
                    gWrapper.Name = item.Name;
                    gWrapper.Id = item.Id;
                    GroupingProperties.Add(gWrapper);
                }

                foreach (var item in namesOfItems)
                {
                    var nWrapper = new NameItemWrapper();
                    nWrapper.Name = item.Name;
                    nWrapper.Id = item.Id;
                    NamesOfItems.Add(nWrapper);
                }

                foreach (var item in items)
                {
                    var iWrapper = new ItemWrapper();
                    iWrapper.PricePerUnit = item.PricePerUnit;
                    iWrapper.CountOfUsedUnits = item.CountOfUsedUnits;
                    iWrapper.CountOfUnits = item.CountOfUnits;
                    iWrapper.Description = item.Description ?? "";
                    iWrapper.ExpectedCost = item.ExcpectedCost;
                    iWrapper.Id = item.Id;

                    var nameItem = NamesOfItems.FirstOrDefault(x => x.Id == item.NameId);
                    if (nameItem != null)
                    {
                        iWrapper.NameItem = nameItem;
                    }
                    else
                    {
                        ErrorHandler.HandleError($"Не найдено имя для записи {item.Id}", Severity.Warning);
                    }

                    var obj = Objects.FirstOrDefault(x => x.Id == item.Objectid);
                    if (obj != null)
                    {
                        iWrapper.Obj = obj;
                    }
                    else
                    {
                        ErrorHandler.HandleError($"Не найден объект для записи {item.Id}  {iWrapper.NameItem?.Name}", Severity.Warning);
                    }

                    var type = TypesOfItems.FirstOrDefault(x => x.Id == item.TypeOfItemId);
                    if (type != null)
                    {
                        iWrapper.Type = type;
                    }
                    else
                    {
                        ErrorHandler.HandleError($"Не найден тип для записи {item.Id}  {iWrapper.NameItem?.Name}", Severity.Warning);
                    }

                    var typeOfUnit = TypesOfUnits.FirstOrDefault(x => x.Id == item.TypeUnitId);
                    if (typeOfUnit != null)
                    {
                        iWrapper.UnitType = typeOfUnit;
                    }
                    else
                    {
                        ErrorHandler.HandleError($"Не найден тип единицы измерения для записи {item.Id}  {iWrapper.NameItem?.Name}", Severity.Warning);
                    }

                    var producer = Producers.FirstOrDefault(x => x.Id == item.ProducerId);
                    if (producer != null)
                    {
                        iWrapper.Producer = producer;
                    }
                    else
                    {
                        ErrorHandler.HandleError($"Не найден производитель для записи {item.Id}  {iWrapper.NameItem?.Name}", Severity.Warning);
                    }

                    Items.Add(iWrapper);
                }


                ErrorHandler.HandleError($"Загрузка основных данных завершена", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при загрузке основных данных -> {e.Message}", Severity.Error);
            }
        }

        private IErrorHandler ErrorHandler { get; }

        private List<ObjectWrapper> Objects { get; set; } = [];

        private List<ItemWrapper> Items { get; set; } = [];

        private List<GroupingPropertyWrapper> GroupingProperties { get; set; } = [];

        private List<NameItemWrapper> NamesOfItems { get; set; } = [];

        private List<TypeOfUnitWrapper> TypesOfUnits { get; set; } = [];

        private List<TypeOfItemWrapper> TypesOfItems { get; set; } = [];

        private List<ProducerWrapper> Producers { get; set; } = [];

        /// <summary>
        /// Если неудачно прошло добавление то вернет -1
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="description"></param>
        /// <returns>Если неудачно прошло добавление то вернет -1</returns>
        public int AddObject(string name, string address, string? description)
        {
            int result = -1;

            try
            {
                using var db = DbContextHelper.GetConstructionContext();

                var obj = new BdClasses.Object();
                obj.Name = name;
                obj.Address = address;
                obj.Description = description;

                db.Objects.Add(obj);
                db.SaveChanges();

                ErrorHandler.HandleError($"Объект добавлен {obj.Name} в базу", Severity.Information);

                var oWrapper = new ObjectWrapper();
                oWrapper.Name = name;
                oWrapper.Address = address;
                oWrapper.Description = description ?? "";
                oWrapper.Id = obj.Id;

                Objects.Add(oWrapper);

                result = obj.Id;

                ErrorHandler.HandleError($"Объект добавлен {obj.Name} в коллекцию", Severity.Information);
            }
            catch (Exception e)
            {
                result = -1;
                ErrorHandler.HandleError($"Ошибка при добавлении объекта {name} -> {e.Message}", Severity.Error);
            }
            return result;
        }

        public bool UpdateObject(ObjectWrapper objWrapper)
        {
            try
            {
                using var db = DbContextHelper.GetConstructionContext();
                var obj = db.Objects.FirstOrDefault(x => x.Id == objWrapper.Id);
                var oWrapper = Objects.FirstOrDefault(x => x.Id == objWrapper.Id);
                if (obj != null && oWrapper is not null)
                {
                    obj.Address = objWrapper.Address;
                    obj.Description = objWrapper.Description;
                    obj.Name = objWrapper.Name;

                    db.SaveChanges();

                    oWrapper.Name = objWrapper.Name;
                    oWrapper.Description = objWrapper.Description;
                    oWrapper.Address = objWrapper.Address;

                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при обновлении объекта {objWrapper.Name} -> {e.Message}", Severity.Error);
            }
            return false;
        }

        public bool RemoveObject(int objId, string objName)
        {

            try
            {

                using var db = DbContextHelper.GetConstructionContext();
                var obj = db.Objects.FirstOrDefault(x => x.Id == objId);
                var oWrapper = Objects.FirstOrDefault(x => x.Id == objId);
                if (obj != null && oWrapper is not null)
                {
                    db.Objects.Remove(obj);
                    db.SaveChanges();

                    Objects.Remove(oWrapper);

                    foreach (var item in Items.Where(x => x.Obj?.Id == objId).ToList())
                    {
                        Items.Remove(item);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении объекта {objName} -> {e.Message}", Severity.Error);
            }
            return false;
        }

        public List<ProtoObject> GetAllObjects()
        {
            return [.. Objects.Select(x => x.ProtoObject)];
        }

        public int AddGroupingProperty(string property)
        {
            int result = -1;
            try
            {
                var gr = new GroupingProperty();
                gr.Name = property;


                using var db = DbContextHelper.GetConstructionContext();

                db.GroupingProperties.Add(gr);
                db.SaveChanges();

                var gWrapper = new GroupingPropertyWrapper();
                gWrapper.Name = property;
                gWrapper.Id = gr.Id;

                GroupingProperties.Add(gWrapper);

                result = gWrapper.Id;

                ErrorHandler.HandleError($"Добавлено свойство группировки {property}", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении нового свойства группировки {property} -> {e.Message}", Severity.Error);
            }
            return result;
        }

        public List<ProtoGroupingProperty> GetPropertiesOfItem(int itemId, string name)
        {
            var res = new List<ProtoGroupingProperty>();

            try
            {
                using var db = DbContextHelper.GetConstructionContext();
                var properties = db.GroupingPropertiesForItems.Where(x => x.ItemId == itemId).Select(x => x.PropId).ToList();
                foreach (var property in GroupingProperties.Where(x => properties.Contains(x.Id)))
                {
                    res.Add(property.ProtoObject);
                }

            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при получении свойств группировки объекта {name} -> {e.Message}", Severity.Error);
            }
            return res;
        }

        public List<ProtoGroupingProperty> GetAllGroupingProperties()
        {
            return [.. GroupingProperties.Select(x => x.ProtoObject)];
        }

        public int AddName(string name)
        {
            int result = -1;
            try
            {
                var nm = new NameItem();
                nm.Name = name;


                using var db = DbContextHelper.GetConstructionContext();

                db.NameItems.Add(nm);
                db.SaveChanges();

                var nWrapper = new NameItemWrapper();
                nWrapper.Name = name;
                nWrapper.Id = nm.Id;

                NamesOfItems.Add(nWrapper);

                result = nWrapper.Id;

                ErrorHandler.HandleError($"Добавлено наименование {name}", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении нового наименования {name} -> {e.Message}", Severity.Error);
            }
            return result;
        }

        public List<ProtoNameItem> GetAllNameItems()
        {
            return [.. NamesOfItems.Select(x => x.ProtoObject)];
        }

        public int AddTypeOfUnit(string name)
        {
            int result = -1;
            try
            {
                var tpu = new TypesOfUnit();
                tpu.Name = name;


                using var db = DbContextHelper.GetConstructionContext();

                db.TypesOfUnits.Add(tpu);
                db.SaveChanges();

                var tuWrapper = new TypeOfUnitWrapper();
                tuWrapper.Name = name;
                tuWrapper.Id = tpu.Id;

                TypesOfUnits.Add(tuWrapper);

                result = tuWrapper.Id;

                ErrorHandler.HandleError($"Добавлена единица измерения {name}", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении единицы измерения {name} -> {e.Message}", Severity.Error);
            }
            return result;
        }

        public List<ProtoTypeOfUnit> GetAllTypesOfUnits()
        {
            return [.. TypesOfUnits.Select(x => x.ProtoObject)];
        }

        public int AddProducer(string name)
        {
            int result = -1;
            try
            {
                var pr = new Producer();
                pr.Name = name;


                using var db = DbContextHelper.GetConstructionContext();

                db.Producers.Add(pr);
                db.SaveChanges();

                var rWrapper = new ProducerWrapper();
                rWrapper.Name = name;
                rWrapper.Id = pr.Id;

                Producers.Add(rWrapper);

                result = rWrapper.Id;

                ErrorHandler.HandleError($"Добавлен производитель {name}", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении производителя {name} -> {e.Message}", Severity.Error);
            }
            return result;
        }

        public List<ProtoProducer> GetAllProducers()
        {
            return [.. Producers.Select(x => x.ProtoObject)];
        }

        public int AddItem(ItemWrapper wrapper)
        {
            int result = -1;
            try
            {
                var item = new Item();
                item.ExcpectedCost = wrapper.ExpectedCost;
                item.CountOfUnits = wrapper.CountOfUnits;
                item.CountOfUsedUnits = wrapper.CountOfUsedUnits;
                item.PricePerUnit = wrapper.PricePerUnit;
                item.Description = wrapper.Description;
                item.NameId = wrapper.NameItem?.Id ?? -1;
                item.Objectid = wrapper.Obj?.Id ?? -1;
                item.ProducerId = wrapper.Producer?.Id ?? -1;
                item.TypeOfItemId = wrapper.Type?.Id ?? -1;
                item.TypeUnitId = wrapper.UnitType?.Id ?? -1;


                using var db = DbContextHelper.GetConstructionContext();

                db.Items.Add(item);
                db.SaveChanges();

                result = item.Id;

                wrapper.Id = item.Id;

                Items.Add(wrapper);

                ErrorHandler.HandleError($"Добавлена новая запись в план объекта {wrapper.Obj?.Name} имя {wrapper.NameItem?.Name}", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении записи {wrapper.NameItem?.Name} -> {e.Message}", Severity.Error);
            }
            return result;
        }

        public bool UpdateItem(ItemWrapper wrapper)
        {

            try
            {
                using var db = DbContextHelper.GetConstructionContext();

                var item = db.Items.FirstOrDefault(x => x.Id == wrapper.Id);
                var iWrapper = Items.FirstOrDefault(x => x.Id == wrapper.Id);
                if (iWrapper != null && item != null)
                {
                    item.CountOfUnits = wrapper.CountOfUnits;
                    item.ExcpectedCost = wrapper.ExpectedCost;
                    item.Description = wrapper.Description;
                    item.PricePerUnit = wrapper.PricePerUnit;
                    item.CountOfUsedUnits = wrapper.CountOfUsedUnits;
                    item.NameId = wrapper.NameItem?.Id ?? -1;
                    item.Objectid = wrapper.Obj?.Id ?? -1;
                    item.ProducerId = wrapper.Producer?.Id ?? -1;
                    item.TypeOfItemId = wrapper.Type?.Id ?? -1;
                    item.TypeUnitId = wrapper.UnitType?.Id ?? -1;

                    db.SaveChanges();

                    iWrapper.PricePerUnit = wrapper.PricePerUnit;
                    iWrapper.CountOfUnits = wrapper.CountOfUnits;
                    iWrapper.CountOfUsedUnits = wrapper.CountOfUsedUnits;
                    iWrapper.Description = wrapper.Description;
                    iWrapper.ExpectedCost = wrapper.ExpectedCost;
                    iWrapper.Type = wrapper.Type;
                    iWrapper.UnitType = wrapper.UnitType;
                    iWrapper.Obj = wrapper.Obj;
                    iWrapper.Producer = wrapper.Producer;
                    iWrapper.NameItem = wrapper.NameItem;

                    ErrorHandler.HandleError($"Обновлена запись {wrapper.NameItem?.Name}   {wrapper.Id}",Severity.Information);

                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при обновлении записи {wrapper.Id}  {wrapper.NameItem?.Name} -> {e.Message}", Severity.Error);
            }
            return false;
        }

        public bool RemoveItem(int itemId, string name)
        {
            try
            {
                using var db = DbContextHelper.GetConstructionContext();
                var item = db.Items.FirstOrDefault(x => x.Id == itemId);
                var iWrapper = Items.FirstOrDefault(x => x.Id == itemId);
                if(item is not null && iWrapper != null)
                {
                    db.Items.Remove(item);
                    db.SaveChanges();

                    Items.Remove(iWrapper);

                    ErrorHandler.HandleError($"Удалена запись {itemId}   {name}",Severity.Information);

                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении записи {itemId}  {name} -> {e.Message}", Severity.Error);
            }
            return false;
        }

        public List<ProtoItem> GetItemsByObject(int objectId)
        {
            return [.. Items.Where(x => x.Obj?.Id == objectId).Select(x => x.ProtoObject)];
        }


    }
}
