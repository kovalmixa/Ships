using Assets.InGameMarkers.Actions;
using UnityEngine;

public class HealAction : IGameAction
{
    public void Execute(ActionContext context)
    {
        if (!context.AmountValue.HasValue)
        {
            Debug.LogWarning("No heal amount provided.");
            return;
        }
        Debug.Log($"Healed:{context.AmountValue}");
        //var stats = context.Target.GetComponent<CharacterStats>();
        //if (stats != null)
        //{
        //    stats.Heal(context.HealAmount.Value);
        //}
    }
}
