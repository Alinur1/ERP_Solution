namespace ErpBackendApi.DAL.Models
{
    public class Department
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}