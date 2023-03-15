namespace PosterDelivery.Utility.EntityModel
{
    public class InvoiceModel
    {
        public int InvoiceHeaderId { get; set; }
        public string? ActualInvoiceAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceSerialNo { get; set; }
        public string? InvoiceDateWithFormat { get; set; }
        public string? Tax { get; set; }
        public string? Discount { get; set; }
        public string? InvoiceFileName { get; set; }
        public string? InvoiceFilePath { get; set; }
        public string? AccountName { get; set; }
    }
}
