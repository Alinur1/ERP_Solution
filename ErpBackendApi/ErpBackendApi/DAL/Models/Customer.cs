using System.ComponentModel.DataAnnotations;

namespace ErpBackendApi.DAL.Models
{
    public class Customer
    {
        public int id { get; set; }
        public string? name { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        [Phone]
        public string? phone { get; set; }
        public string? address { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
