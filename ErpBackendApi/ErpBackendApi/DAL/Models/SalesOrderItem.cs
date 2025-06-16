namespace ErpBackendApi.DAL.Models
{
    public class SalesOrderItem
    {
        public int id { get; set; }
        public int? sales_order_id { get; set; }
        public int? product_id { get; set; }
        public int? quantity { get; set; }
        public decimal? unit_price { get; set; }
        public decimal? discount { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}