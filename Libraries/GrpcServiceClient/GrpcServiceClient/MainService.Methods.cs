using Grpc.Core;

using GrpcServiceClient.DataContracts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServiceClient
{
    public partial class MainService
    {
        // === GETTERS ===

        public List<Permission> GetAllPermissions()
        {
            var reply = Client.GetAllPermissions(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Permissions.Select(x => new Permission(x))];
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            var reply = await Client.GetAllPermissionsAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Permissions.Select(x => new Permission(x))];
        }

        public List<Item> GetItemsByObject(int objectId)
        {
            var reply = Client.GetItemsByObject(new PInt() { Val = objectId });
            return [.. reply.Items.Select(x => new Item(x))];
        }

        public async Task<List<Item>> GetItemsByObjectAsync(int objectId)
        {
            var reply = await Client.GetItemsByObjectAsync(new PInt() { Val = objectId });
            return [.. reply.Items.Select(x => new Item(x))];
        }

        public List<DataContracts.ConObject> GetAllObjects()
        {
            var reply = Client.GetAllObjects(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Objs.Select(x => new DataContracts.ConObject(x))];
        }

        public async Task<List<DataContracts.ConObject>> GetAllObjectsAsync()
        {
            var reply = await Client.GetAllObjectsAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Objs.Select(x => new DataContracts.ConObject(x))];
        }

        public List<GroupingProperty> GetAllGroupingProps()
        {
            var reply = Client.GetAllGroupingProps(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Props.Select(x => new GroupingProperty(x))];
        }

        public async Task<List<GroupingProperty>> GetAllGroupingPropsAsync()
        {
            var reply = await Client.GetAllGroupingPropsAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Props.Select(x => new GroupingProperty(x))];
        }

        public List<GroupingProperty> GetGroupingPropsByItem(int itemId)
        {
            var reply = Client.GetGroupingPropsByItem(new PInt() { Val = itemId });
            return [.. reply.Props.Select(x => new GroupingProperty(x))];
        }

        public async Task<List<GroupingProperty>> GetGroupingPropsByItemAsync(int itemId)
        {
            var reply = await Client.GetGroupingPropsByItemAsync(new PInt() { Val = itemId });
            return [.. reply.Props.Select(x => new GroupingProperty(x))];
        }

        public List<TypeOfUnit> GetAllTypesOfUnit()
        {
            var reply = Client.GetAllTypesOfUnit(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Types_.Select(x => new TypeOfUnit(x))];
        }

        public async Task<List<TypeOfUnit>> GetAllTypesOfUnitAsync()
        {
            var reply = await Client.GetAllTypesOfUnitAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Types_.Select(x => new TypeOfUnit(x))];
        }

        public List<Role> GetAllRoles()
        {
            var reply = Client.GetAllRoles(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Roles.Select(x => new Role(x))];
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            var reply = await Client.GetAllRolesAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Roles.Select(x => new Role(x))];
        }

        public List<User> GetAllUsers()
        {
            var reply = Client.GetAllUsers(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Users.Select(x => new User(x))];
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var reply = await Client.GetAllUsersAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Users.Select(x => new User(x))];
        }

        public List<NameItem> GetAllNames()
        {
            var reply = Client.GetAllNames(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Names.Select(x => new NameItem(x))];
        }

        public async Task<List<NameItem>> GetAllNamesAsync()
        {
            var reply = await Client.GetAllNamesAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Names.Select(x => new NameItem(x))];
        }

        public List<Producer> GetAllProducers()
        {
            var reply = Client.GetAllProducers(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Producers.Select(x => new Producer(x))];
        }

        public async Task<List<Producer>> GetAllProducersAsync()
        {
            var reply = await Client.GetAllProducersAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Producers.Select(x => new Producer(x))];
        }

        public List<ItemMetaData> GetMetaDataOfItem(int itemId)
        {
            var reply = Client.GetMetaDataOfItem(new PInt() { Val = itemId });
            return [.. reply.Metadata.Select(x => new ItemMetaData(x))];
        }

        public async Task<List<ItemMetaData>> GetMetaDataOfItemAsync(int itemId)
        {
            var reply = await Client.GetMetaDataOfItemAsync(new PInt() { Val = itemId });
            return [.. reply.Metadata.Select(x => new ItemMetaData(x))];
        }

        public Journal GetJournal()
        {
            var reply = Client.GetJournal(new Google.Protobuf.WellKnownTypes.Empty());
            return new Journal(reply);
        }

        public async Task<Journal> GetJournalAsync()
        {
            var reply = await Client.GetJournalAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return new Journal(reply);
        }

        // === ITEM ===

        public int AddItem(Item item)
        {
            var reply = Client.AddItem(item.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddItemAsync(Item item)
        {
            var reply = await Client.AddItemAsync(item.ProtoObject);
            return reply.Val;
        }

        public bool UpdateItem(Item item)
        {
            var reply = Client.UpdateItem(item.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var reply = await Client.UpdateItemAsync(item.ProtoObject);
            return reply.Val;
        }

        public bool RemoveItem(Item item)
        {
            var reply = Client.RemoveItem(item.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> RemoveItemAsync(Item item)
        {
            var reply = await Client.RemoveItemAsync(item.ProtoObject);
            return reply.Val;
        }

        // === OBJECT ===

        public int AddObject(DataContracts.ConObject obj)
        {
            var reply = Client.AddObject(obj.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddObjectAsync(DataContracts.ConObject obj)
        {
            var reply = await Client.AddObjectAsync(obj.ProtoObject);
            return reply.Val;
        }

        public bool UpdateObject(DataContracts.ConObject obj)
        {
            var reply = Client.UpdateObject(obj.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> UpdateObjectAsync(DataContracts.ConObject obj)
        {
            var reply = await Client.UpdateObjectAsync(obj.ProtoObject);
            return reply.Val;
        }

        public bool RemoveObject(DataContracts.ConObject obj)
        {
            var reply = Client.RemoveObject(obj.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> RemoveObjectAsync(DataContracts.ConObject obj)
        {
            var reply = await Client.RemoveObjectAsync(obj.ProtoObject);
            return reply.Val;
        }

        // === USER ===

        public int AddUser(User user)
        {
            var reply = Client.AddUser(user.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddUserAsync(User user)
        {
            var reply = await Client.AddUserAsync(user.ProtoObject);
            return reply.Val;
        }

        public bool UpdateUser(User user)
        {
            var reply = Client.UpdateUser(user.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var reply = await Client.UpdateUserAsync(user.ProtoObject);
            return reply.Val;
        }

        public bool RemoveUser(User user)
        {
            var reply = Client.RemoveUser(user.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> RemoveUserAsync(User user)
        {
            var reply = await Client.RemoveUserAsync(user.ProtoObject);
            return reply.Val;
        }

        // === ROLE ===

        public int AddRole(Role role)
        {
            var reply = Client.AddRole(role.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddRoleAsync(Role role)
        {
            var reply = await Client.AddRoleAsync(role.ProtoObject);
            return reply.Val;
        }

        public bool UpdateRole(Role role)
        {
            var reply = Client.UpdateRole(role.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            var reply = await Client.UpdateRoleAsync(role.ProtoObject);
            return reply.Val;
        }

        public bool RemoveRole(Role role)
        {
            var reply = Client.RemoveRole(role.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> RemoveRoleAsync(Role role)
        {
            var reply = await Client.RemoveRoleAsync(role.ProtoObject);
            return reply.Val;
        }

        // === OTHER ADD ===

        public int AddGroupingProperty(GroupingProperty prop)
        {
            var reply = Client.AddGroupingProperty(prop.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddGroupingPropertyAsync(GroupingProperty prop)
        {
            var reply = await Client.AddGroupingPropertyAsync(prop.ProtoObject);
            return reply.Val;
        }

        public int AddProducer(Producer producer)
        {
            var reply = Client.AddProducer(producer.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddProducerAsync(Producer producer)
        {
            var reply = await Client.AddProducerAsync(producer.ProtoObject);
            return reply.Val;
        }

        public int AddNameItem(NameItem nameItem)
        {
            var reply = Client.AddNameItem(nameItem.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddNameItemAsync(NameItem nameItem)
        {
            var reply = await Client.AddNameItemAsync(nameItem.ProtoObject);
            return reply.Val;
        }

        public int AddTypeOfUnit(TypeOfUnit type)
        {
            var reply = Client.AddTypeOfUnit(type.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddTypeOfUnitAsync(TypeOfUnit type)
        {
            var reply = await Client.AddTypeOfUnitAsync(type.ProtoObject);
            return reply.Val;
        }

        public int AddItemMetaData(ItemMetaData metaData)
        {
            var reply = Client.AddItemMetaData(metaData.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddItemMetaDataAsync(ItemMetaData metaData)
        {
            var reply = await Client.AddItemMetaDataAsync(metaData.ProtoObject);
            return reply.Val;
        }

        public bool AddRecordToJournal(JournalRecord record)
        {
            var reply = Client.AddRecordToJournal(record.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> AddRecordToJournalAsync(JournalRecord record)
        {
            var reply = await Client.AddRecordToJournalAsync(record.ProtoObject);
            return reply.Val;
        }

        // === GROUPING PROPERTY OF ITEM ===

        public bool AddGroupingPropertyOfItem(GroupingProperty prop, int itemId)
        {
            var reply = Client.AddGroupingPropertyOfItem(new ChangeGroupingPropertyofItem()
            {
                Prop = prop.ProtoObject,
                ItemId = itemId
            });

            return reply.Val;
        }

        public async Task<bool> AddGroupingPropertyOfItemAsync(GroupingProperty prop, int itemId)
        {
            var reply = await Client.AddGroupingPropertyOfItemAsync(new ChangeGroupingPropertyofItem()
            {
                Prop = prop.ProtoObject,
                ItemId = itemId
            });

            return reply.Val;
        }

        public bool RemoveGroupingPropertyOfItem(GroupingProperty prop, int itemId)
        {
            var reply = Client.RemoveGroupingPropertyOfItem(new ChangeGroupingPropertyofItem()
            {
                Prop = prop.ProtoObject,
                ItemId = itemId
            });

            return reply.Val;
        }

        public async Task<bool> RemoveGroupingPropertyOfItemAsync(GroupingProperty prop, int itemId)
        {
            var reply = await Client.RemoveGroupingPropertyOfItemAsync(new ChangeGroupingPropertyofItem()
            {
                Prop = prop.ProtoObject,
                ItemId = itemId
            });

            return reply.Val;
        }

        // === REMOVE ITEM META DATA ===

        public bool RemoveItemMetaData(ItemMetaData metaData)
        {
            var reply = Client.RemoveItemMetaData(metaData.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> RemoveItemMetaDataAsync(ItemMetaData metaData)
        {
            var reply = await Client.RemoveItemMetaDataAsync(metaData.ProtoObject);
            return reply.Val;
        }

        // === OTHER ===

        public void CheckActive()
        {
            Client.CheckActive(new Google.Protobuf.WellKnownTypes.Empty());
        }

        public async Task CheckActiveAsync()
        {
            await Client.CheckActiveAsync(new Google.Protobuf.WellKnownTypes.Empty());
        }

        public List<TypeOfItem> GetAllTypesOfItems()
        {
            var reply = Client.GetAllTypesOfItems(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Types_.Select(x => new TypeOfItem(x))];
        }

        public async Task<List<TypeOfItem>> GetAllTypesOfItemsAsync()
        {
            var reply = await Client.GetAllTypesOfItemsAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Types_.Select(x => new TypeOfItem(x))];
        }

        public bool SetGroupingPropertiesOfItem(List<GroupingProperty> groupingProps, int itemd)
        {
            var reply = Client.SetGroupingPropertiesOfItem(new()
            {
                Props = new List_GroupingProps()
                {
                    Props = { groupingProps.Select(x => x.ProtoObject) }
                },
                ItemId = itemd
            });

            return reply.Val;
        }

        public async Task<bool> SetGroupingPropertiesOfItemAsync(List<GroupingProperty> groupingProps, int itemd)
        {
            var reply = await Client.SetGroupingPropertiesOfItemAsync(new()
            {
                Props = new List_GroupingProps()
                {
                    Props = { groupingProps.Select(x => x.ProtoObject) }
                },
                ItemId = itemd
            });

            return reply.Val;
        }

        public List<MetaDataType> GetAllMetaDataTypes()
        {
            var reply = Client.GetAllMetaDataTypes(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Types_.Select(x => new MetaDataType(x))];
        }

        public async Task<List<MetaDataType>> GetAllMetaDataTypesAsync()
        {
            var reply = await Client.GetAllMetaDataTypesAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return [.. reply.Types_.Select(x => new MetaDataType(x))];
        }

        public List<ObjectMetadata> GetAllObjectMetaData(int objectId)
        {
            var reply = Client.GetAllObjectMetaData(new PInt() { Val = objectId });
            return [.. reply.Metadata.Select(x => new ObjectMetadata(x))];
        }

        public async Task<List<ObjectMetadata>> GetAllObjectMetaDataAsync(int objectId)
        {
            var reply = await Client.GetAllObjectMetaDataAsync(new PInt() { Val = objectId });
            return [.. reply.Metadata.Select(x => new ObjectMetadata(x))];
        }

        public bool RemoveObjectMetadata(ObjectMetadata metadata)
        {
            var reply = Client.RemoveObjectMetadata(metadata.ProtoObject);
            return reply.Val;
        }

        public async Task<bool> RemoveObjectMetadataAsync(ObjectMetadata metadata)
        {
            var reply = await Client.RemoveObjectMetadataAsync(metadata.ProtoObject);
            return reply.Val;
        }

        public int AddObjectMetaData(ObjectMetadata metadata)
        {
            var reply = Client.AddObjectMetaData(metadata.ProtoObject);
            return reply.Val;
        }

        public async Task<int> AddObjectMetaDataAsync(ObjectMetadata metadata)
        {
            var reply = await Client.AddObjectMetaDataAsync(metadata.ProtoObject);
            return reply.Val;
        }

        public BackupStatusResponse GetBackupStatus()
        {
            var reply = Client.GetBackupStatus(new Google.Protobuf.WellKnownTypes.Empty());
            return new BackupStatusResponse(reply);
        }

        public async Task<BackupStatusResponse> GetBackupStatusAsync()
        {
            var reply = await Client.GetBackupStatusAsync(new Google.Protobuf.WellKnownTypes.Empty());
            return new BackupStatusResponse(reply);
        }

        public async IAsyncEnumerable<byte[]> StartBackupAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            var stream = Client.StartBackup(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: ct);

            while (await stream.ResponseStream.MoveNext(ct))
            {
                var chunk = stream.ResponseStream.Current;

                if (chunk.IsLast)
                {
                    break;
                }

                yield return chunk.Data.ToByteArray();
            }
        }
    }
}

