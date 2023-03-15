using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility {
    public class OrderDeliveryCount
    {
        public int MonthlyOrderDelivery { get; set; }
        public int YesterdayOrderyDelivery { get; set; }
        public int TomorrowOrderDelivery { get; set; }
        public int TodayOrderDelivery { get; set; }

        public int InProgressStores { get; set; }
        public int CompletedStores { get; set; }
        public int PendingStores { get; set; }
    }
}
