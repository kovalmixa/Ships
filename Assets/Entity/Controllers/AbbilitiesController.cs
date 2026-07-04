using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class AbbilityUnit
    {
        public string Name { get; set; }
        public GameObject Source { get; set; }
        public Action action { get; set; }
    }

    public class AbbilitiesController
    {
        public List<AbbilityUnit> abbilities { get; set; } = new();

        public void ActivateAbility() // add some key
        {

        }
    }
}
