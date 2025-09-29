using ClosedXML.Excel;

using GrpcServiceClient;
using GrpcServiceClient.DataContracts;

using ObjectsManager.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Helpers
{
   

    public static class ExcelSerializatorHelper
    {
        public static string NameColumn { get; } = "Наименование(Обязательно)";
        public static string TypeOfUnitColumn { get; } = "Ед.изм.(Обязательно)";
        public static string CountOfUnitsColumn { get; } = "Кол-во закупленных ед. изм.(по ум. 0)";
        public static string PricePerUnitColumn { get; } = "Цена за ед.(по ум. 0)";
        public static string TypeOfItemColumn { get; } = "Тип записи(М или Р по ум. М)";
        public static string ExpectedCountColumn { get; } = "Ожидаемый расход(по ум. 0)";
        public static string CountOfUsedUnits { get; } = "Кол-во исп. ед.(по ум. 0)";
        public static string ProducerColumn { get; } = "Производитель(по ум. Не определено)";
        public static string DescriptionColumn { get; } = "Примечание(по ум. пустая строка)";

        public static async Task<List<ItemWrapper>?> LoadExcelFile(string? filePath,MainService service,ConObject? conObject, ObservableCollection<TypeOfItem> typesOfItems, ObservableCollection<NameItem>? names = null,
             ObservableCollection<TypeOfUnit>? typeOfUnits = null, ObservableCollection<Producer>? producers = null)
        {
            var list = new List<ItemWrapper>();


            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return null;
                }

                using var workbook = new XLWorkbook(filePath);

                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed();

                foreach (var row in rows.Skip(1))
                {
                    try
                    {
                        var item = new Item();
                        item.Id = -1;

                        item.Obj = conObject;

                        var name = row.Cell(1).GetValue<string>();
                        var nameItem = names?.FirstOrDefault(x => x.Name == name);

                        if (nameItem is null)
                        {
                            nameItem = new NameItem() { Id = -1, Name = name };
                            nameItem.Id = await service.AddNameItemAsync(nameItem);
                            if (nameItem.Id == -1) continue;
                            item.NameItem = nameItem;
                            names?.Add(nameItem);
                        }
                        else
                        {
                            item.NameItem = nameItem;
                        }

                        var typeOfUnit = row.Cell(2).GetValue<string>();
                        var typeOfUnitItem = typeOfUnits?.FirstOrDefault(x => x.Name == typeOfUnit);
                        if (typeOfUnitItem is null)
                        {
                            typeOfUnitItem = new TypeOfUnit() { Id = -1, Name = typeOfUnit };
                            typeOfUnitItem.Id = await service.AddTypeOfUnitAsync(typeOfUnitItem);
                            if (typeOfUnitItem.Id == -1) continue;
                            item.UnitType = typeOfUnitItem;
                            typeOfUnits?.Add(typeOfUnitItem);
                        }
                        else
                        {
                            item.UnitType = typeOfUnitItem;
                        }

                        var isRead = row.Cell(3).TryGetValue(out double countOfUnits);
                        item.CountOfUnits = isRead ? countOfUnits : 0;

                        isRead = row.Cell(4).TryGetValue(out double price);
                        item.PricePerUnit = isRead ? price : 0;

                        var type = row.Cell(5).GetValue<string>();
                        var typeOfItem = typesOfItems.FirstOrDefault(x => x.Name.Contains(type, StringComparison.OrdinalIgnoreCase)) ?? typesOfItems[0];

                        item.Type = typeOfItem;

                        isRead = row.Cell(6).TryGetValue(out double expectedCost);
                        item.ExpectedCost = isRead ? expectedCost : 0;

                        isRead = row.Cell(7).TryGetValue(out double countUsedUnits);
                        item.CountOfUsedUnits = isRead ? countUsedUnits : 0;

                        isRead = row.Cell(8).TryGetValue(out string producerName);
                        producerName = string.IsNullOrWhiteSpace(producerName) ? "Не определено" : producerName;
                        var producer = producers?.FirstOrDefault(x => x.Name == producerName);

                        if (producer is null)
                        {
                            producer = new Producer() { Id = -1, Name = producerName };
                            producer.Id = await service.AddProducerAsync(producer);
                            if (producer.Id == -1) continue;
                            item.Producer = producer;
                            producers?.Add(producer);
                        }
                        else
                        {
                            item.Producer = producer;
                        }

                        isRead = row.Cell(9).TryGetValue(out string description);
                        item.Description = isRead ? description : "";

                        item.Id = await service.AddItemAsync(item);

                        if(item.Id == -1) continue;

                        list.Add(new(item));
                    }
                    catch 
                    {

                    }
                }
            }
            catch 
            {
                list = null;
            }

            return list;
        }

        public static Exception? SaveAsExcelTable(string? filePath,ConObject? conObject, List<ItemWrapper> wrappers)
        {
            Exception? result = null;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                result = new ArgumentException("Путь до файла должен быть не пустым");
                return result;
            }

            try
            {
                using var workbook = new XLWorkbook();

                var workSheet = workbook.Worksheets.Add($"Записи объекта {conObject?.Name}");

                workSheet.Cell(1, 1).Value = NameColumn;
                workSheet.Cell(1, 2).Value = TypeOfUnitColumn;
                workSheet.Cell(1, 3).Value = CountOfUnitsColumn;
                workSheet.Cell(1, 4).Value = PricePerUnitColumn;
                workSheet.Cell(1, 5).Value = TypeOfItemColumn;
                workSheet.Cell(1, 6).Value = ExpectedCountColumn;
                workSheet.Cell(1, 7).Value = CountOfUsedUnits;
                workSheet.Cell(1, 8).Value = ProducerColumn;
                workSheet.Cell(1, 9).Value = DescriptionColumn;

                for(int i=0; i<wrappers.Count; i++)
                {
                    var wrapper = wrappers[i];
                    workSheet.Cell(i + 2, 1).Value = wrapper.SourceItem.NameItem?.Name ?? "NULL";
                    workSheet.Cell(i + 2, 2).Value = wrapper.SourceItem.UnitType?.Name ?? "NULL";
                    workSheet.Cell(i + 2, 3).Value = wrapper.SourceItem.CountOfUnits;
                    workSheet.Cell(i + 2, 4).Value = wrapper.SourceItem.PricePerUnit;
                    workSheet.Cell(i + 2, 5).Value = $"{wrapper.SourceItem.Type?.Name[0] ?? 'N'}";
                    workSheet.Cell(i + 2, 6).Value = wrapper.SourceItem.ExpectedCost;
                    workSheet.Cell(i + 2, 7).Value = wrapper.SourceItem.CountOfUsedUnits;
                    workSheet.Cell(i + 2, 8).Value = wrapper.SourceItem.Producer?.Name ?? "NULL";
                    workSheet.Cell(i + 2, 9).Value = wrapper.SourceItem.Description ?? "";
                }
                
                workSheet.Columns().AdjustToContents();
                workSheet.Rows().AdjustToContents();

                workbook.SaveAs(filePath);
            }
            catch (Exception e)
            {
                result = e;
            }

            return result;
        }
    }
}
