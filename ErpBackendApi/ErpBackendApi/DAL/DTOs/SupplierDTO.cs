using System.ComponentModel.DataAnnotations;

namespace ErpBackendApi.DAL.DTOs
{
    public class SupplierDTO
    {
        public int id { get; set; }
        public string? company_name { get; set; }
        public string? contact_person_name { get; set; }
        [Phone]
        public string? phone { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        public string? address { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
