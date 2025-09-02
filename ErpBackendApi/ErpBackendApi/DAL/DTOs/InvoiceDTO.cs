using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.DTOs
{
    public class InvoiceDTO
    {
        public int id { get; set; }
        public DateTime? invoice_date { get; set; }
        public decimal? total_amount { get; set; }
        public bool? is_paid { get; set; }
        public DateTime? due_date { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
        public SalesOrderDTO? sales_order { get; set; }
    }
}
