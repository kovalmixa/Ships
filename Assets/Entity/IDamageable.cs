using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Entity
{
    internal interface IDamageable
    {
        void TakeDamage(float damage);
    }
}
