namespace ErpBackendApi.DAL.Models
{
    public class Product
    {
        public int id { get; set; }
        public string? name { get; set; }
        public int? category_id { get; set; }
        public int? supplier_id { get; set; }
        public string? sku { get; set; }
        public string? description { get; set; }
        public string? unit { get; set; }
        public decimal? price { get; set; }
        public DateTime? created_at { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
