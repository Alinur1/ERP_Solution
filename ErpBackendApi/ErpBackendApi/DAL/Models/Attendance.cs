using System.ComponentModel.DataAnnotations.Schema;
using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.Models
{
    public class Attendance
    {
        public int id { get; set; }
        public int? employee_id { get; set; }
        public DateOnly? date_of_attendance { get; set; }
        public TimeOnly? check_in { get; set; }
        public TimeOnly? check_out { get; set; }
        //[Column(TypeName = "int")]
        public AttendanceStatus? status { get; set; }
    }
}