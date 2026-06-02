using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Entity.Controllers.AI
{
    public class RelationsHandler
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
