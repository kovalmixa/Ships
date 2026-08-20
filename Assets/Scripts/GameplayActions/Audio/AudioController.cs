using Assets.Scripts.Actions;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameplayActions.Audio
{
    public readonly struct AudioKey : IEquatable<AudioKey>
    {
        public readonly GameObject source;
        public readonly EventReference @event;

        public AudioKey(GameObject source, EventReference @event)
        {
            this.source = source;
            this.@event = @event;
        }
        public bool Equals(AudioKey other) => source == other.source && @event.Equals(other.@event);
        public override bool Equals(object obj) => obj is AudioKey other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(source, @event);
    }

    public class AudioController : SingletonMonoBehaviour<AudioController>
    {
        private readonly Dictionary<AudioKey, AudioInstance> _instances = new();

        public void Play(InteractionContext context, AudioData data, Vector3 position, Transform target = null)
        {
            if (data.isOneShot)
            {
                RuntimeManager.PlayOneShot(data.sound, position);
                return;
            }

            var key = new AudioKey(context.SourceObject, data.sound);

            if (_instances.TryGetValue(key, out var instance))
            {
                instance.Refresh(position, target, data.loopStopTimeout);
                return;
            }

            instance = new AudioInstance(context, data, () => _instances.Remove(key));
            _instances.Add(key, instance);
            instance.Start(position, target, data.loopStopTimeout);
        }
    }
}
