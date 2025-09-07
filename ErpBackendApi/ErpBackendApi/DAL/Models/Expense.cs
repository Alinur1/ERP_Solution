namespace ErpBackendApi.DAL.Models
{
    public class Expense
    {
        public int id { get; set; }
        public int? purchase_order_id { get; set; }
        public string? description { get; set; }
        public decimal? total_amount { get; set; }
        public DateTime? expense_date { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
