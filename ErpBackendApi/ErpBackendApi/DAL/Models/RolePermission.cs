namespace ErpBackendApi.DAL.Models
{
    public class RolePermission
    {
        public int id { get; set; }
        public int? role_id { get; set; }
        public int? feature_id { get; set; }
        public bool? can_read { get; set; }
        public bool? can_create { get; set; }
        public bool? can_update { get; set; }
        public bool? can_delete { get; set; }
    }
}
