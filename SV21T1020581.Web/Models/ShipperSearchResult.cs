using SV21T1020581.DomainModels;

namespace SV21T1020581.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}
