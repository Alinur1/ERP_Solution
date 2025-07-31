namespace ErpBackendApi.DAL.DTOs
{
    public class ReportDTO
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? module { get; set; }
        public int? created_by { get; set; }
        public string? created_by_name { get; set; }
        public DateTime? created_at { get; set; }
        public string? filters_json { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
