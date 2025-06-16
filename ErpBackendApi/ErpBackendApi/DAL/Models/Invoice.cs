namespace ErpBackendApi.DAL.Models
{
    public class Invoice
    {
        public int id { get; set; }
        public int? sales_order_id { get; set; }
        public DateOnly? invoice_date { get; set; }
        public decimal? total_amount { get; set; }
        public bool? is_paid { get; set; }
        public DateOnly? due_date { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}