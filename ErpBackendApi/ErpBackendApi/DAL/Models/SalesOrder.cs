using ErpBackendApi.DAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpBackendApi.DAL.Models
{
    public class SalesOrder
    {
        public int id { get; set; }
        public int? customer_id { get; set; }
        public DateTime? order_date { get; set; }
        public DateTime? delivery_date { get; set; }
        //[Column(TypeName = "int")]
        public DeliveryStatus? delivery_status { get; set; }
        //[Column(TypeName = "int")]
        public SalesOrderStatus? status { get; set; }
        public string? notes { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
