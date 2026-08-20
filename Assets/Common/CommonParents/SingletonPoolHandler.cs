using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Assets.Handlers.CommonParents
{
    public abstract class SingletonPoolHandler<TSingleton, TPooled> : SingletonMonoBehaviour<TSingleton>
        where TSingleton : MonoBehaviour
    {
        [SerializeField] protected GameObject poolNode;
        [SerializeField] protected int initialCapacity;
        [SerializeField] protected int maxPoolSize;

        #region Setup

        protected abstract void ClearOnSceneChange();

        protected virtual void OnEnable() => SceneController.OnBeforeSceneLoad += ClearOnSceneChange;

        protected virtual void OnDisable() => SceneController.OnBeforeSceneLoad -= ClearOnSceneChange;

        #endregion 
    }
}