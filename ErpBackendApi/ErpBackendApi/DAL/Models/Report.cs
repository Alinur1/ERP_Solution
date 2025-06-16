namespace ErpBackendApi.DAL.Models
{
    public class Report
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? module { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_at { get; set; }
        public string? filters_json { get; set; }
    }
}
