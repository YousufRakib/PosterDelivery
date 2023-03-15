using PosterDelivery.Repository.Interface;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;

namespace PosterDelivery.Services {
    public class LoggerService : ILoggerService {

        private readonly ILoggerRepository _loggerRepository;
        public LoggerService(ILoggerRepository loggerRepository) {
            this._loggerRepository = loggerRepository;
        }

        public Task<bool> LogExceptionInformation(Error error) {
            return _loggerRepository.LogExceptionInformation(error);
        }
    }
}
