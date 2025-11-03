using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ItemStatsSystem;
using UnityEngine;

namespace Loader
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public bool isinit;
        public Harmony Harmony;
        

        void Awake()
        {
            Debug.Log("DisplayItemValue Loaded!!!");
        }
        void OnDestroy()
        {

        }
        void OnEnable()
        {
           
            LoadAllPrefabsFromBundleAsync();

        }


        void OnDisable()
        {
          //  Harmony.UnpatchAll("fsefiewfse1");
        }

        async Task LoadAllPrefabsFromBundleAsync()
        {
            // 获取 mod DLL 路径
            string dllPath = Assembly.GetExecutingAssembly().Location;
            string modDirectory = Path.GetDirectoryName(dllPath);

            // 假设 AssetBundle 名为 "examplebundle"
            string assetBundlePath = Path.Combine(modDirectory, "itemmod");

            if (!File.Exists(assetBundlePath))
            {
                Debug.LogError("AssetBundle 不存在: " + assetBundlePath);
                return;
            }

            // 加载 AssetBundle 异步
            var bundleRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
            await AwaitUntilDone(bundleRequest);

            AssetBundle bundle = bundleRequest.assetBundle;
            if (bundle == null)
            {
                Debug.LogError("AssetBundle 加载失败");
                return;
            }

            // 加载所有 GameObject 类型的资源（即 prefab）
            var loadAllRequest = bundle.LoadAllAssetsAsync(typeof(GameObject));
            await AwaitUntilDone(loadAllRequest);

            UnityEngine.Object[] loadedAssets = loadAllRequest.allAssets;

            if (loadedAssets == null || loadedAssets.Length == 0)
            {
                Debug.LogWarning("AssetBundle 中没有找到任何 prefab");
                return;
            }

            foreach (var obj in loadedAssets)
            {
                GameObject prefab = obj as GameObject;
                if (prefab == null) continue;

                Item itemComponent = prefab.GetComponent<Item>();
                if (itemComponent != null)
                {
                    if(ItemAssetsCollection.AddDynamicEntry(itemComponent))
                    {
                        Debug.Log("AddDynamicEntry Item : " + itemComponent.DisplayName);
                    }
                    Debug.Log("Load Item : " + itemComponent.DisplayName);
                }
                else
                {
                    Debug.Log("Prefab '" + prefab.name + "' 没有 Item 组件");
                }
            }

      
        }

        private async Task AwaitUntilDone(AsyncOperation asyncOp)
        {
            while (!asyncOp.isDone)
            {
                await Task.Yield(); 
            }
        }



    }
}
