using Assets.Global.FileGlobal;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs
{
    public enum GUIAssetType
    {
        Slot
    }

    public static class GUIAssetsHandler
    {
        public static Dictionary<GUIAssetType, string> assetsDict = new()
        {
            { GUIAssetType.Slot, "Slot" },
        };

        public static VisualTreeAsset GetPrefabSync(GUIAssetType type)
        {
            return PrefabLoader.Instance.LoadAssetSync<VisualTreeAsset>(assetsDict[type]);
        }

        public static async UniTask<VisualTreeAsset> GetPrefabAsync(GUIAssetType type)
        {
            return await PrefabLoader.Instance.LoadAssetAsync<VisualTreeAsset>(assetsDict[type]);
        }
    }
}
