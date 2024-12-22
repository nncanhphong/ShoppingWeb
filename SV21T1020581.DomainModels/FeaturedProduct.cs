using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DomainModels
{
    public class FeaturedProduct
    {
        public int CategoryID { get; set; } = 0;
        public int ProductID { get; set; }= 0;
        public String Photo { get; set; } = "";
        public String CategoryName { get; set; } = "";
        public String ProductName { get; set; } = "";
        public decimal Price { get; set; } = 0;
    }
}
