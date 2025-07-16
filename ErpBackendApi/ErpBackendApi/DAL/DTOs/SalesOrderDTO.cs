using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.DTOs
{
    public class SalesOrderDTO
    {
        public int id { get; set; }
        public string? order_number { get; set; }
        public int? customer_id { get; set; }
        public string? customer_name { get; set; }
        public DateOnly? order_date { get; set; }
        public DateOnly? delivery_date { get; set; }
        public DeliveryStatus? delivery_status { get; set; }
        public SalesOrderStatus? status { get; set; }
        public string? notes { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
