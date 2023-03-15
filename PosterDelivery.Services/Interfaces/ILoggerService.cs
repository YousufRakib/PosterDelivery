using PosterDelivery.Utility.EntityModel;

namespace PosterDelivery.Services.Interfaces {
    public interface ILoggerService {
        public Task<bool> LogExceptionInformation(Error error);
    }
}
