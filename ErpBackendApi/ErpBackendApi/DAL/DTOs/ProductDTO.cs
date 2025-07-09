namespace ErpBackendApi.DAL.DTOs
{
    public class ProductDTO
    {
        public int id { get; set; }
        public string? name { get; set; }
        public int? category_id { get; set; }
        public string? category_name { get; set; }
        public int? supplier_id { get; set; }
        public string? supplier_company_name { get; set; }
        public string? sku { get; set; }
        public string? description { get; set; }
        public string? unit { get; set; }
        public decimal? price { get; set; }
        public DateTime? created_at { get; set; }
    }
}
