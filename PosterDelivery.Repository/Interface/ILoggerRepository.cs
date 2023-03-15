using PosterDelivery.Utility.EntityModel;

namespace PosterDelivery.Repository.Interface {
    public interface ILoggerRepository {
        Task<bool> LogExceptionInformation(Error error);
    }
}
