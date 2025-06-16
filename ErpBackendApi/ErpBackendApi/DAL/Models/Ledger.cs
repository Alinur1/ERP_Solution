namespace ErpBackendApi.DAL.Models
{
    public class Ledger
    {
        public int id { get; set; }
        public DateOnly? entry_date { get; set; }
        public string? description { get; set; }
        public int? debit_account_id { get; set; }
        public int? credit_account_id { get; set; }
        public decimal? amount { get; set; }
        public int? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
