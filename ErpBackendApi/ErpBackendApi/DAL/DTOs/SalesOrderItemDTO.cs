namespace ErpBackendApi.DAL.DTOs
{
    public class SalesOrderItemDTO
    {
        public int id { get; set; }
        public int? sales_order_id { get; set; }
        public string? sales_order_number { get; set; }
        public int? product_id { get; set; }
        public string? product_name { get; set; }
        public int? quantity { get; set; }
        public decimal? unit_price { get; set; }
        public decimal? discount { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
