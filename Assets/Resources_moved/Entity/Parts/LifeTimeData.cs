using Assets.Scripts.Actions.VFX;
using GameplayActions;
using System;
using UnityEngine;

namespace Assets.Resources_moved.Entity.Parts
{
    [Serializable]
    public struct LifeTimeData
    {

        [Header("Life time VFX")]
        public VfxType vfxDuringLife;
        public VfxType vfxOnLowHP;
        public ExplosionData explosionOnDeath;
    }
}
