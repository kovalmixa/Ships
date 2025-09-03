using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Entity.DataContainers
{
    public class TextureProperties
    {
        public string ID { get; set; }

        public bool IsLooped { get; set; }

        public TextureProperties OnFinish { get; set; }

        public TextureMovement[] Movement { get; set; }

        //private Vector2 size;
        //public Vector2 Size
        //{
        //    get
        //    {
        //        if (size == Vector2.zero) size = Vector2.one;
        //        return size;
        //    }
        //    set
        //    {
        //        if (size == Vector2.zero) return;
        //        size = value;
        //    } 
        //}
    }
}
