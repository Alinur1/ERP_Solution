namespace ErpBackendApi.DAL.DTOs
{
    public class PayrollDTO
    {
        public int id { get; set; }
        public int? employee_id { get; set; }
        public int? user_id { get; set; }
        public string? employee_name { get; set; }
        public DateTime? period_start { get; set; }
        public DateTime? period_end { get; set; }
        public decimal? base_salary { get; set; }
        public decimal? deductions { get; set; }
        public decimal? bonuses { get; set; }
        public decimal? net_pay { get; set; }
        public DateTime? paid_on { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
