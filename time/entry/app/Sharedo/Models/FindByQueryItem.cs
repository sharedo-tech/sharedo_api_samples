namespace App.Sharedo.Models
{
    public class FindByQueryItem
    {
        public Guid Id { get; set; }
        public FindByQueryItemData Data{ get; set; }

        public FindByQueryItem()
        {
            Data = new FindByQueryItemData();
        }
    }
}