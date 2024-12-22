using SV21T1020581.DomainModels;

namespace SV21T1020581.Web.Models
{
    public class ProductEditModel
    {
        public Product? Product{get; set; }
        public List<ProductAttribute>? ProductAttributes { get; set;}
        public List<ProductPhoto>? ProductPhotos { get; set;}
    }
}
