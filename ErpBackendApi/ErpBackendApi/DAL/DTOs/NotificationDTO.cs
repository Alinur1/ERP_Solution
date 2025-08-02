namespace ErpBackendApi.DAL.DTOs
{
    public class NotificationDTO
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public string? user_name { get; set; }
        public string? title { get; set; }
        public string? message { get; set; }
        public bool? is_read { get; set; }
        public DateTime? created_at { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
