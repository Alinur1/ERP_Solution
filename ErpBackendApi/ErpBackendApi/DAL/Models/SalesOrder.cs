using ErpBackendApi.DAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpBackendApi.DAL.Models
{
    public class SalesOrder
    {
        public int id { get; set; }
        public int? customer_id { get; set; }
        public DateTime? order_date { get; set; } //Place today's order date in the frontend by default so that the datetime can be chose by the user
        public DateTime? delivery_date { get; set; }
        //[Column(TypeName = "int")]
        public DeliveryStatus? delivery_status { get; set; }
        //[Column(TypeName = "int")]
        public SalesOrderStatus? status { get; set; }
        public string? notes { get; set; }
        public DateTime? last_updated { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
