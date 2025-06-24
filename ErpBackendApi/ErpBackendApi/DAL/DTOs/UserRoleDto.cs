namespace ErpBackendApi.DAL.DTOs
{
    public class UserRoleDto
    {
        public int user_id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public DateTime? created_at { get; set; }
        public string? role_name { get; set; }
        public string? role_description { get; set; }
    }
}