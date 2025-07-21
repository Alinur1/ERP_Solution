using System.ComponentModel.DataAnnotations.Schema;
using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.Models
{
    public class PurchaseOrder
    {
        public int id { get; set; }
        public int? supplier_id { get; set; }
        public DateOnly? order_date { get; set; }
        public DateOnly? expected_delivery_date { get; set; }
        //[Column(TypeName = "int")]
        public DeliveryStatusPurchaseOrder? delivery_status { get; set; }
        public string? notes { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}