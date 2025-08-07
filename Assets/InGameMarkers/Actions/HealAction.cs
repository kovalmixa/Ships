using Assets.InGameMarkers.Actions;
using UnityEngine;

public class HealAction : IGameAction
{
    public bool IsPassive { get; set; } = true;

    public void Execute(ActionContext context)
    {
        if (!context.Value.HasValue)
        {
            Debug.LogWarning("No heal amount provided.");
            return;
        }
        Debug.Log($"Healed:{context.Value}");
        //var stats = context.Target.GetComponent<CharacterStats>();
        //if (stats != null)
        //{
        //    stats.Heal(context.HealAmount.Value);
        //}
    }
}
