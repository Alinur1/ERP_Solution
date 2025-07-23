namespace ErpBackendApi.DAL.DTOs
{
    public class ExpenseDTO
    {
        public int id { get; set; }
        public int? purchase_order_id { get; set; }
        public int? product_id { get; set; }
        public int? category_id { get; set; }
        public string? category_name { get; set; }
        public string? description { get; set; }
        public decimal? amount { get; set; }
        public DateOnly? expense_date { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
