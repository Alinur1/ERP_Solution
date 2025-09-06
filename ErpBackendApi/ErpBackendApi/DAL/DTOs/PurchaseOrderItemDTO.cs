namespace ErpBackendApi.DAL.DTOs
{
    public class PurchaseOrderItemDTO
    {
        public int id { get; set; }
        public int? purchase_order_id { get; set; }
        public int? supplier_id { get; set; }
        public string? supplier_name { get; set; }
        public int? product_id { get; set; }
        public string? product_name { get; set; }
        public string? sku { get; set; }
        public decimal? price { get; set; }
        public int? quantity { get; set; }
        public decimal? amount { get; set; }
        public decimal? discount { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
        public ProductDTO? product { get; set; }
    }
}
