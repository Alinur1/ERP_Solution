namespace ErpBackendApi.DAL.Models
{
    public class Notification
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public string? title { get; set; }
        public string? message { get; set; }
        public bool? is_read { get; set; }
        public DateTime? created_at { get; set; }
    }
}
