using ErpBackendApi.DAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpBackendApi.DAL.Models
{
    public class Transaction
    {
        public int id { get; set; }
        public int? account_id { get; set; }
        public DateTime? transaction_date { get; set; }
        public string? description { get; set; }
        public decimal? amount { get; set; }
        public DebitCreditType? normal_balance { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
