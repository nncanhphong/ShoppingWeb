using SV21T1020581.DomainModels;

namespace SV21T1020581.Web.Models
{
    public class ProductSearchResult: PaginationSearchResult
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public required List<Product> Data { get; set; }
    }
}
