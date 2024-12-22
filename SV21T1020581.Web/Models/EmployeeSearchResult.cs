using SV21T1020581.DomainModels;

namespace SV21T1020581.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}
