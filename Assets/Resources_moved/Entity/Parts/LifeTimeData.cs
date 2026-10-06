using Assets.Scripts.Actions.VFX;
using GameplayActions;
using System;
using UnityEngine;

namespace Assets.Entity.Parts
{
    public enum LifeState { FullHealth, Damaged, RecentlyDead, OldDeath}

    [Serializable]
    public struct LifeTimeData
    {
        [Header("Life time VFX")]
        public VfxType vfxDuringLife;
        public VfxType vfxOnLowHP;
        public ExplosionData explosionOnDeath;
        public LifeState lifeState;
		public bool leftCorpseOnDeath;
    }
}
