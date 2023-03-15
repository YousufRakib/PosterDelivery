using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Utility.EntityModel;
using PosterDelivery.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Data;

namespace PosterDelivery.Repository.Repository {
    public class ProductRepository : IProductRepository {
        private readonly IConfiguration? _configuration;
        private readonly string? _connectionString;

        public ProductRepository(IConfiguration configuration) {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString("PosterDeliveryConnection");
        }

        ///Product Category
        public async Task<IList<Product>> GetProducts() {

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetProducts);
                var products = await connection.QueryAsync<Product>(query);

                var result = products.ToList();
                return result;
            }

        }

        public async Task<IList<ProductDetails>> GetProductDetails() {

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetProductView);
                var products = await connection.QueryAsync<ProductDetails>(query);
                var result = products.ToList();
                return result;
            }

        }

        public async Task<string?> SaveProductInformation(Product product) {
            try {
                string result = "";

                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    string query = "";

                    if (product.ProductId > 0) {
                        query = string.Format(DapperQuery.UpdateProductInformation);
                    } else {
                        query = string.Format(DapperQuery.SaveProductInformation);
                    }

                    await connection.QueryAsync<int>(query, product);

                    result = "1";
                }

                return result;
            } catch (Exception ex) {
                return "0";
            }
        }

        public async Task<IList<ProductCategory>> GetProductCategory() {
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetProductCategories);
                var productCategory = await connection.QueryAsync<ProductCategory>(query);

                var result = productCategory.ToList();
                return result;
            }
        }

        public async Task<Product> GetProductInfo(int productId) {
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetProductInfo);
                var product = await connection.QueryAsync<Product>(query, new { Id = productId });

                var result = product.FirstOrDefault();
                return result;
            }
        }

        ///////Product Category
        public async Task<IList<ProductCategory>> GetProductCategories() {
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetAllProductCategory);
                var productCategories = await connection.QueryAsync<ProductCategory>(query);

                var result = productCategories.ToList();
                return result;
            }
        }

        public async Task<string?> SaveProductCategoryInformation(ProductCategory productCategory) {
            try {
                string result = "";

                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    string query = "";

                    if (productCategory.CategoryId > 0) {
                        query = string.Format(DapperQuery.UpdateProductCategoryInformation);
                    } else {
                        query = string.Format(DapperQuery.SaveProductCategoryInformation);
                    }

                    await connection.QueryAsync<int>(query, productCategory);

                    result = "1";
                }

                return result;
            } catch (Exception ex) {
                return "0";
            }
        }

        public async Task<ProductCategory> GetProductCategoryInfo(int productCategoryId) {
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetProductCategory);
                var productCategory = await connection.QueryAsync<ProductCategory>(query, new { CategoryId = productCategoryId });

                var result = productCategory.FirstOrDefault();
                return result;
            }
        }

        public async Task<IList<ProductDetails>> GetProductDetailsByStore(int CustomerId, int DriverCustomerTrackId) {
            try {
                IList<ProductDetails> lstProductDetails = new List<ProductDetails>();
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    var products = await connection.QueryAsync<ProductDetails>("PickupProductsByStore", new {
                        CustomerId = CustomerId,
                        DriverCustomerTrackId = DriverCustomerTrackId,
                    }, commandType: CommandType.StoredProcedure); ;
                    lstProductDetails = (IList<ProductDetails>)products;
                }
                if (lstProductDetails.Count == 0) {
                    return new List<ProductDetails>();
                }
                return lstProductDetails;
            } catch {
                return new List<ProductDetails>();
            }
        }
    }
}
