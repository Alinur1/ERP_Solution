using System.ComponentModel.DataAnnotations;

namespace ErpBackendApi.DAL.Models
{
    public class Supplier
    {
        public int id { get; set; }
        [Required]
        public string company_name { get; set; }
        [Required]
        public string contact_person_name { get; set; }
        [Required]
        [Phone]
        public string phone { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        public string? address { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
