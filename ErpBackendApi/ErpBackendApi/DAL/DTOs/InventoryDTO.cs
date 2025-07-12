namespace ErpBackendApi.DAL.DTOs
{
    public class InventoryDTO
    {
        public int id { get; set; }
        public int? product_id { get; set; }
        public string? product_name { get; set; }
        public int? quantity { get; set; }
        public int? reorder_level { get; set; }
        public DateTime? last_updated { get; set; }
    }
}
