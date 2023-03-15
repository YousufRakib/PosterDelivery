using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosterDelivery.Utility.EntityModel;

namespace PosterDelivery.Repository.Interface {
    public interface MiscRepoInterface {
        Task<int> UploadUpdateCustomerRepo(IList<CustomerUploadExcel> lstCustomerUploadExcel);
    }
}
