using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Entity.DataContainers
{
    public interface IObject
    {
        public string Id { get; set; }
        public Graphics GetGraphics();
    }
}
