using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AI
{
    public class RelationsController
    {
        public enum Relationship
        {
            enemy,
            ally,
            neutral
        }

        public Relationship EntityRelationShip()
        {
            return Relationship.enemy;
        }
    }
}
