using Assets.Scripts.Actions.VFX;
using GameplayActions;
using System;
using UnityEngine;

namespace Assets.Entity.Parts
{
    [Serializable]
    public struct LifeCycleData
    {
        [Header("Life time VFX")]
        public VfxType vfxDuringLife;

        [Header("Damage & death VFX")]
        public VfxType vfxOnLowHP;
        public ExplosionData explosionOnDeath;
        public Sprite corpseSprite;

        public bool leftCorpseOnDeath;
        public bool sunkOnDeath;
        [Tooltip("<= 0 means use CorpseInstance default")]
        public float corpseDisappearTime;
    }
}