namespace ErpBackendApi.DAL.Models
{
    public class Setting
    {
        public int id { get; set; }
        public string? key { get; set; }
        public string? value { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
