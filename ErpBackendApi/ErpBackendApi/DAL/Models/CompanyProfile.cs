namespace ErpBackendApi.DAL.Models
{
    public class CompanyProfile
    {
        public int id { get; set; }
        public string? company_name { get; set; }
        public string? address { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? tax_number { get; set; }
        public string? logo { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
