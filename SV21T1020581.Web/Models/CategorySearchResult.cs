using SV21T1020581.DomainModels;

namespace SV21T1020581.Web.Models
{
    public class CategorySearchResult: PaginationSearchResult
    {
        public required List<Category> Data{ get; set; }
    }
}
