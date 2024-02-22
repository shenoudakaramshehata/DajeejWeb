namespace Gameapp.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public float ItemPrice { get; set; }
        public int ItemQty { get; set; }
        public float ItemTotal { get; set; }

       
    }
}
