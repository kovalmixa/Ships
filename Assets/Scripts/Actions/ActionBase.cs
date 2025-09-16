using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public abstract class ActionBase : MonoBehaviour, IGameAction
    {
        public bool IsPassive { get; set; } = true;
        public abstract void Execute(GameObject source, Vector3 targetPos);
    }
}
