using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosterDelivery.Utility.EntityModel;

namespace PosterDelivery.Services.Interfaces {
    public interface IMiscServiceInterface {
        Task<int> UploadUpdateCustomerService(IList<CustomerUploadExcel> lstCustomerUploadExcel);
    }
}
