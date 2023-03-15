using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility.EntityModel {
    public class Error {
        public string? Source { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? StatusCode { get; set; }
        public string? UserId { get; set; }
        public string? Type { get; set; }
    }
}
