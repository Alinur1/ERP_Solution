namespace ErpBackendApi.DAL.Models
{
    public class UserRole
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public int? role_id { get; set; }
    }
}
