using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class HealAction : MonoBehaviour, IGameAction
    {
        public bool IsPassive { get; set; } = true;

        public int HealValue;

        public void Execute(GameObject source, Vector3 targetPos)
        {
            Debug.Log($"Healed:{HealValue}");
            //var stats = context.Target.GetComponent<CharacterStats>();
            //if (stats != null)
            //{
            //    stats.Heal(context.HealAmount.Value);
            //}
        }
    }
}
