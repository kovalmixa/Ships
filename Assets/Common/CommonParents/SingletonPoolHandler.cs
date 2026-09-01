using Assets.Handlers.SceneHandlers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Handlers.CommonParents
{
    public abstract class SingletonPoolHandler<TSingleton, TPooled> : SingletonMonoBehaviour<TSingleton>
        where TSingleton : MonoBehaviour
    {
        [SerializeField] protected GameObject poolNode;
        [SerializeField] protected int initialCapacity;
        [SerializeField] protected int maxPoolSize;

        protected bool isClearing = false;
        private const int _clearingPercentage = 25;
        protected int ClearingDelayQuantity => maxPoolSize / _clearingPercentage;
        protected bool IsIndexOverClearDelay(int i) => i > 0 && i % ClearingDelayQuantity == 0;

        #region Setup

        protected abstract UniTask ClearOnSceneChangeAsync();

        protected virtual void OnEnable() => SceneController.OnBeforeSceneLoad += ClearOnSceneChangeAsync;

        protected virtual void OnDisable() => SceneController.OnBeforeSceneLoad -= ClearOnSceneChangeAsync;

        #endregion 
    }
}