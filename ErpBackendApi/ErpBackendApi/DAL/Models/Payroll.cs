namespace ErpBackendApi.DAL.Models
{
    public class Payroll
    {
        public int id { get; set; }
        public int? employee_id { get; set; }
        public DateOnly? period_start { get; set; }
        public DateOnly? period_end { get; set; }
        public decimal? base_salary { get; set; }
        public decimal? deductions { get; set; }
        public decimal? bonuses { get; set; }
        public decimal? net_pay { get; set; }
        public DateOnly? paid_on { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}