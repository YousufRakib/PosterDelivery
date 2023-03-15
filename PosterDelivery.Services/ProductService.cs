using Microsoft.AspNetCore.Http;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services 
{
    public class ProductService : IProductService 
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository) {
            this._productRepository = productRepository;
        }
        public async Task<IList<Product>> GetProducts() {
            return await _productRepository.GetProducts();
        }
        public async Task<string?> SaveProductInformation(Product product) 
        {
            return await _productRepository.SaveProductInformation(product);
        }
        public async Task<IList<ProductCategory>> GetProductCategory() 
        {
            return await _productRepository.GetProductCategory();
        }

        public async Task<Product> GetProductInfo(int productId)
        {
            return await _productRepository.GetProductInfo(productId);
        }


        public async Task<IList<ProductCategory>> GetProductCategories() 
        {
            return await _productRepository.GetProductCategories();
        }
        public async Task<string?> SaveProductCategoryInformation(ProductCategory productCategory) 
        {
            return await _productRepository.SaveProductCategoryInformation(productCategory);
        }
        public async Task<ProductCategory> GetProductCategoryInfo(int productCategoryId) 
        {
            return await _productRepository.GetProductCategoryInfo(productCategoryId);
        }
        public async Task<IList<ProductDetails>> GetProductDetailsService() {
            return await _productRepository.GetProductDetails();
        }
        public async Task<IList<ProductDetails>> GetProductDetailsByStore(int CustomerId, int DriverCustomerTrackId) {
            return await _productRepository.GetProductDetailsByStore(CustomerId, DriverCustomerTrackId);
        }
    }
}
