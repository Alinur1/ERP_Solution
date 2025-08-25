namespace ErpBackendApi.DAL.Models
{
    public class Inventory
    {
        public int id { get; set; }
        public int? product_id { get; set; }
        public int? quantity { get; set; }
        public int? reorder_level { get; set; }
        public DateTime? last_updated { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
