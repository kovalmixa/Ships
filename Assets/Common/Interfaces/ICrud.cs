using System;

namespace Assets.Common.Interfaces
{
    public interface ICrud
    {
        public event Action OnChange;
        
        public event Action OnDelete;

        public event Action OnInsert;
    }
}
