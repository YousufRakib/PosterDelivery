namespace PosterDelivery.Utility.EntityModel {
    public class DriverCustomerTrack {
        public int DriverId { get; set; }
        public int CustomerId { get; set; }
        public int ActualProductsPicked { get; set; }
        public int ActualProductsShipped { get; set; }
        public DateTime ProposedDate { get; set; }
        public DateTime ActualDate { get; set; }
        public string? ActualDateString { get; set; }
        public int IsCompleted { get; set; }
        public string? ReasonForNotCompletion { get; set; }
        public int IsActive { get; set; }
        public int CreatedByUser { get; set; }
        public int DriverCustomerTrackId { get; set; }
        public int DeliveryDay { get; set; }
    }
}
