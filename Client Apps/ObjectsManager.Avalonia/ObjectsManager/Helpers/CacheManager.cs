using GrpcServiceClient.DataContracts;
using ObjectsManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Helpers
{
    public static class CacheManager
    {
        private static Dictionary<int, List<ItemWrapper>> ObjectsItemsCache { get; } = [];

        private static Dictionary<int, List<ObjectMetadata>> ObjectsMetadataCache { get; } = [];

        private static Dictionary<int, List<ItemMetaData>> ItemsMetadataCache { get; } = [];

        public static void AddOrUpdateObjectCache(int objectId, List<ItemWrapper> items)
        {
            if (!ObjectsItemsCache.TryAdd(objectId, items))
            {
                ObjectsItemsCache[objectId] = items;
            }
        }

        public static List<ItemWrapper>? GetObjectCache(int objectId)
        {
            List<ItemWrapper>? res = [];
            bool isExist = ObjectsItemsCache.TryGetValue(objectId, out res);
            return res;
        }

        public static void RemoveObjectCache(int objectId)
        {
            ObjectsItemsCache.Remove(objectId);
        }

        public static void AddOrUpdateObjectMetadataCachee(int objectId, List<ObjectMetadata> metadata)
        {
            if (!ObjectsMetadataCache.TryAdd(objectId, metadata))
            {
                ObjectsMetadataCache[objectId] = metadata;
            }
        }

        public static List<ObjectMetadata>? GetObjectMetadataCache(int objectId)
        {
            List<ObjectMetadata>? res = [];
            bool isExist = ObjectsMetadataCache.TryGetValue(objectId, out res);
            return res;
        }

        public static void RemoveObjectMetadataCache(int objectId)
        {
            ObjectsMetadataCache.Remove(objectId);
        }

        public static void AddOrUpdateItemMetadataCache(int itemId, List<ItemMetaData> metadata)
        {
            if (!ItemsMetadataCache.TryAdd(itemId, metadata))
            {
                ItemsMetadataCache[itemId] = metadata;
            }
        }

        public static List<ItemMetaData>? GetItemMetadataCache(int itemId)
        {
            List<ItemMetaData>? res = [];
            bool isExist = ItemsMetadataCache.TryGetValue(itemId, out res);
            return res;
        }

        public static void RemovItemMetadataCache(int itemId)
        {
            ItemsMetadataCache.Remove(itemId);
        }

        public static void ClearAllCache()
        {
            ObjectsItemsCache.Clear();
            ObjectsMetadataCache.Clear();
            ItemsMetadataCache.Clear();
        }
    }
}
