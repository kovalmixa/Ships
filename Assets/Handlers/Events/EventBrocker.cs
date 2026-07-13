using Assets.Handlers.Events;
using System;
using System.Collections.Generic;

public static class EventBrocker
{
    private static readonly Dictionary<Type, List<Delegate>> _listeners = new();

    public static void Subscribe<T>(Action<T> listener) where T : IGameplayEvent
    {
        Type eventType = typeof(T);
        if (!_listeners.TryGetValue(eventType, out var list))
        {
            list = new List<Delegate>();
            _listeners[eventType] = list;
        }
        list.Add(listener);
    }

    public static void Unsubscribe<T>(Action<T> listener) where T : IGameplayEvent
    {
        Type eventType = typeof(T);
        if (_listeners.TryGetValue(eventType, out var list)) list.Remove(listener);
    }

    public static void Raise<T>(T eventData) where T : IGameplayEvent
    {
        Type eventType = typeof(T);
        if (_listeners.TryGetValue(eventType, out var list))
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i] is Action<T> action) action.Invoke(eventData);
    }

    // ВАЖНО: Вызывать этот метод при смене сцены (например, в GameManager), чтобы избежать утечек памяти!
    public static void Clear()
    {
        _listeners.Clear();
    }
}