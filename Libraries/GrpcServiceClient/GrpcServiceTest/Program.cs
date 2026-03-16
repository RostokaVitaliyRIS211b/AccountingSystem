using GrpcServiceClient;
using GrpcServiceClient.DataContracts;
using System.Diagnostics;

Random random = new Random();

Console.WriteLine("Прогрузка");
var client = new MainService("sa", "yW8Z5rDb7E5Ko9A+CHyFfw==");
Thread.Sleep(2000);

Console.WriteLine("Старт старого");
Stopwatch stopwatch = Stopwatch.StartNew();

var item = new Item();
var listOfProps = new List<GroupingProperty>();
for (int i = 0; i < 10; i++)
{
    var items = client.GetItemsByObject(9);
    if (items.Count > 0)
    {
        item = items[random.Next(items.Count)];
    }

    foreach (var item1 in items)
    {
        listOfProps = client.GetGroupingPropsByItem(item1.Id);
    }
}
Console.WriteLine(item.Id);
if (listOfProps.Count > 0)
{
    Console.WriteLine(listOfProps[random.Next(listOfProps.Count)].Name);
}
stopwatch.Stop();
Console.WriteLine($"Время старого {stopwatch.ElapsedMilliseconds}\n");

Console.WriteLine("Старт нового");
stopwatch.Restart();
for (int i = 0; i < 10; i++)
{
    var res = client.GetItemsByObjectWithGroupingProps(9,default);
    if (res.Count > 0)
    {
        item = res[random.Next(res.Count)].item;
    }

    foreach (var item1 in res)
    {
        listOfProps = item1.groupingProperties;
    }
}
Console.WriteLine(item.Id);
if (listOfProps.Count > 0)
{
    Console.WriteLine(listOfProps[random.Next(listOfProps.Count)].Name);
}
stopwatch.Stop();
Console.WriteLine($"Время нового {stopwatch.ElapsedMilliseconds}");


