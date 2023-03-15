using Microsoft.AspNetCore.Http;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services.Interfaces 
{
    public interface IProductService 
    {
        Task<IList<Product>> GetProducts();
        Task<string?> SaveProductInformation(Product product);
        Task<IList<ProductCategory>> GetProductCategory();
        Task<Product> GetProductInfo(int productId);

        Task<IList<ProductCategory>> GetProductCategories();
        Task<string?> SaveProductCategoryInformation(ProductCategory productCategory);
        Task<ProductCategory> GetProductCategoryInfo(int productCategoryId);
        Task<IList<ProductDetails>> GetProductDetailsService();
        Task<IList<ProductDetails>> GetProductDetailsByStore(int CustomerId, int DriverCustomerTrackId);
    }
}
