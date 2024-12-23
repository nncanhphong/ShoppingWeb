namespace SV21T1020581.Shop.Models
{
    public class ListOrderDetailModel
    {
        public int CustomerID { get; set; }
        public required List<OrderDetailModel> ListOrder { get; set; }
    }
}
