namespace Assets.Entity.Controllers
{
    public interface IDirty
    {
        public bool IsDirty { get; }

        public void MarkDirty();
    }
}
