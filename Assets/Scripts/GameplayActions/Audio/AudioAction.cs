using Assets.Common;
using Assets.Scripts.Actions;
using FMODUnity;
using GameplayActions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameplayActions.Audio
{
    [Serializable]
    public struct AudioParameter
    {
        public string fmodParameterName; // parameter name in FMOD Studio
        public string sourceKey; // key for IAudioParameterSource (StatType name or arbitrary string)
        public float fallbackValue; // if the source did not provide a value
        public float remapInMin, remapInMax;
        public float remapOutMin, remapOutMax; // usually 0..1 under FMOD
    }

    [System.Serializable]
    public class AudioData : ActionData
    {
        public EventReference sound;
        public bool isOneShot;
        public float loopStopTimeout;
        public List<AudioParameter> parameters;
    }

    public class AudioAction : GameplayAction<AudioData>
    {
        protected override void ExecuteAction(InteractionContext context, AudioData data, Vector2 targetPos)
        {
            if (data == null || data.sound.IsNull) return;
            AudioController.Instance.Play(context, data, targetPos);
        }

        protected override void ExecuteAction(InteractionContext context, AudioData data, IInteractive target)
        {
            if (data == null || data.sound.IsNull) return;
            Vector3 position = target != null ? target.GameObject.transform.position : context.SourceObject.transform.position;
            AudioController.Instance.Play(context, data, position, target?.GameObject.transform);
        }
    }
}