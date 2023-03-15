using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility.EntityModel 
{
    public class ProductCategory 
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ParentCategoryId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public string? IsActive { get; set; }
    }
}
