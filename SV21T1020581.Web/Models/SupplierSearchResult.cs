using SV21T1020581.DomainModels;

namespace SV21T1020581.Web.Models
{
    public class SupplierSearchResult: PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
