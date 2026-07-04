using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using UnityEngine;
using UnityEngine.Events;

public enum BuffApplicationPolicy
{
    Stack,           // несколько одинаковых баффов
    Replace,         // заменяем старый на новый (самый частый для абилок)
    Refresh,         // обновляем только длительность
    UniquePerSource, // только один от конкретного source
}

public abstract class BuffStatus : MonoBehaviour
{
    public string BuffId { get; private set; }
    public string SourceId { get; set; }
    public float Duration { get; set; } = -1f;
    public bool IsPermanent => Duration < 0;

    public Modifiers modifiers;
    public UnityEvent onRemove;
    public UnityEvent onRefresh;
    private float _lifetime = 0f;

    public BuffApplicationPolicy Policy { get; set; } = BuffApplicationPolicy.Replace;

    public void Initialize(string buffId, string sourceId, float duration = -1f)
    {
        BuffId = buffId;
        SourceId = sourceId;
        Duration = duration;
    }

    public virtual bool Tick(InterractionContext interractionContext)
    {
        if (Duration < 0) return false;
        _lifetime += Time.deltaTime;
        return _lifetime >= Duration;
    }
}