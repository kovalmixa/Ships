namespace Assets.Common.Interfaces
{
    public interface ICrud
    {
        public void OnUpdate();
        
        public void OnDelete();

        public void OnInsert();
    }
}
