using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.DTOs
{
    public class SalesOrderDTO
    {
        public int id { get; set; }
        public int? customer_id { get; set; }
        public string? customer_name { get; set; }
        public DateTime? order_date { get; set; }
        public DateTime? delivery_date { get; set; }
        public DeliveryStatus? delivery_status { get; set; }
        public SalesOrderStatus? status { get; set; }
        public string? notes { get; set; }
        public DateTime? last_updated { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
