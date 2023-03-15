using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;

namespace PosterDelivery.Repository.Repository {
    public class LoggerRepository : ILoggerRepository {

        private readonly IConfiguration? _configuration;
        private readonly string? _connectionString;

        public LoggerRepository(IConfiguration configuration) {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString("PosterDeliveryConnection");
        }

        public async Task<bool> LogExceptionInformation(Error error) {
            bool isSaved = false;
            try {
                if (error != null) {
                    using (var connection = new SqlConnection(_connectionString)) {
                        connection.Open();
                        string query = string.Format(DapperQuery.InsertExceptionLogs); ;

                        await connection.QueryAsync<int>(query, error);
                        isSaved = true;
                    }
                }
            } catch (Exception ex) {
                isSaved = false;
            }
            
            return isSaved;
        }
    }
}
