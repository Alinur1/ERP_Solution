using ErpBackendApi.DAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpBackendApi.DAL.Models
{
    public class Account
    {
        public int id { get; set; }
        public string? name { get; set; }
        //[Column(TypeName = "int")]
        public AccountType? type { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
