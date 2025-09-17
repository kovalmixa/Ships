using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class HealAction : ActionBase
    {

        public int HealValue;

        public override void Execute(GameObject source, Vector3 targetPos)
        {
            if (!CanActivate(source, targetPos)) return;
            Debug.Log($"Healed:{HealValue}");
            //var stats = context.Target.GetComponent<CharacterStats>();
            //if (stats != null)
            //{
            //    stats.Heal(context.HealAmount.Value);
            //}
        }

    }
}
