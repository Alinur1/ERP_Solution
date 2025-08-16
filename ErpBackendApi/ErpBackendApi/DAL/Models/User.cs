using System.ComponentModel.DataAnnotations;

namespace ErpBackendApi.DAL.Models
{
    public class User
    {
        public int id { get; set; }
        public string? name { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        public string? password { get; set; }
        [Phone]
        public string? phone { get; set; }
        public DateTime? created_at { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
