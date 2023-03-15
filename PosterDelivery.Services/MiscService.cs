using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Repository.Repository;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;

namespace PosterDelivery.Services {
    public class MiscService : IMiscServiceInterface {
        private readonly MiscRepoInterface _miscRepository;
        public MiscService(MiscRepoInterface miscRepository) {
            this._miscRepository = miscRepository;
        }
        public Task<int> UploadUpdateCustomerService(IList<CustomerUploadExcel> lstCustomerUploadExcel) {
            return _miscRepository.UploadUpdateCustomerRepo(lstCustomerUploadExcel);
        }
    }
}
