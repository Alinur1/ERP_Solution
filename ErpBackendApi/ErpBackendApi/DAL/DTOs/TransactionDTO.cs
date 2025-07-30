using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.DTOs
{
    public class TransactionDTO
    {
        public int id { get; set; }
        public int? account_id { get; set; }
        public string? account_name { get; set; }
        public DateOnly? transaction_date { get; set; }
        public string? description { get; set; }
        public decimal? amount { get; set; }
        //[Column(TypeName = "int")]
        public TransactionType? type { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
