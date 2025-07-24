using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.DTOs
{
    public class EmployeeDTO
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public string? employee_name { get; set; }
        public string? employee_email { get; set; }
        public string? employee_phone { get; set; }
        public DateTime? employee_created_at { get; set; }
        public int? department_id { get; set; }
        public string? department_name { get; set; }
        public DateOnly? date_hired { get; set; }
        public decimal? salary { get; set; }
        public EmployeeStatus? status { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
