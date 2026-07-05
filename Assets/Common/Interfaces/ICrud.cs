namespace Assets.Common.Interfaces
{
    public interface ICrud
    {
        public void OnChange();
        
        public void OnDelete();

        public void OnInsert();
    }
}
