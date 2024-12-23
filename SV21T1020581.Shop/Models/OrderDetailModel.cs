using SV21T1020581.DomainModels;

namespace SV21T1020581.Shop.Models
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}
