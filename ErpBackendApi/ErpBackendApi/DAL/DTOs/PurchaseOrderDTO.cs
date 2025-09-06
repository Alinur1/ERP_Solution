using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.DTOs
{
    public class PurchaseOrderDTO
    {
        public int id { get; set; }
        public int? supplier_id { get; set; }
        public string? company_name { get; set; }
        public string? contact_person_name { get; set; }
        public DateTime? order_date { get; set; }
        public DateTime? expected_delivery_date { get; set; }
        public DeliveryStatusPurchaseOrder? delivery_status { get; set; }
        public string? notes { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
        public SupplierDTO? supplier { get; set; }
        public List<PurchaseOrderItemDTO> items { get; set; } = new();
    }
}
