using System.Collections.Generic;
using Assets.Handlers.SceneHandlers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Handlers.CommonParents
{
    public abstract class SingletonPoolHandler<TSingleton, TPooled> : SingletonMonoBehaviour<TSingleton>
        where TSingleton : MonoBehaviour
        where TPooled : class
    {
        [SerializeField] protected GameObject poolNode;
        [SerializeField] protected int initialCapacity;
        [SerializeField] protected int maxPoolSize;

        protected bool isClearing = false;
        protected bool isPrewarmed = false;

        public bool IsPrewarmed => isPrewarmed;

        private const int _clearingPercentage = 25;
        protected int ClearingDelayQuantity => maxPoolSize / _clearingPercentage;
        protected bool IsIndexOverClearDelay(int i) => i > 0 && i % ClearingDelayQuantity == 0;

        #region Setup

        protected abstract UniTask ClearOnSceneChangeAsync();

        protected virtual UniTask PrewarmAsync() => UniTask.CompletedTask;

        protected virtual void PrewarmPool(IObjectPool<TPooled> pool, int amount)
        {
            if (pool == null || amount <= 0) return;

            var tempList = new List<TPooled>(amount);
            for (int i = 0; i < amount; i++) tempList.Add(pool.Get());
            foreach (var item in tempList) pool.Release(item);
        }

        protected virtual async UniTask OnAfterSceneLoadAsync()
        {
            if (isPrewarmed) return;
            await PrewarmAsync();
            isPrewarmed = true;
        }

        protected virtual void OnEnable()
        {
            SceneController.OnBeforeSceneLoad += ClearOnSceneChangeAsync;
            SceneController.OnAfterSceneLoad += OnAfterSceneLoadAsync;
        }

        protected virtual void OnDisable()
        {
            SceneController.OnBeforeSceneLoad -= ClearOnSceneChangeAsync;
            SceneController.OnAfterSceneLoad -= OnAfterSceneLoadAsync;
        }

        #endregion 
    }
}