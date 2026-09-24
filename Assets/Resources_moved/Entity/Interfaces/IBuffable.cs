using Assets.Entity.Controllers;

namespace Assets.Entity.Interfaces
{
    public interface IBuffable
    {
        BuffStatusesController Buffs { get; }
    }
}
