using Microsoft.AspNetCore.Http;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Repository.Interface 
{
    public interface IProductRepository {
        Task<IList<Product>> GetProducts();
        Task<string?> SaveProductInformation(Product product);
        Task<IList<ProductCategory>> GetProductCategory();
        Task<Product> GetProductInfo(int productId);

        Task<IList<ProductCategory>> GetProductCategories();
        Task<string?> SaveProductCategoryInformation(ProductCategory productCategory);
        Task<ProductCategory> GetProductCategoryInfo(int productCategoryId);
        Task<IList<ProductDetails>> GetProductDetails();
        Task<IList<ProductDetails>> GetProductDetailsByStore(int CustomerId, int DriverCustomerTrackId);

    }
}
