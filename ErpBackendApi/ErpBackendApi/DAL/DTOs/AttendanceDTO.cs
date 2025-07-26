using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.DTOs
{
    public class AttendanceDTO
    {
        public int id { get; set; }
        public int? employee_id { get; set; }
        public string? employee_name { get; set; }
        public DateOnly? date_of_attendance { get; set; }
        public TimeOnly? check_in { get; set; }
        public TimeOnly? check_out { get; set; }
        public AttendanceStatus? status { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
