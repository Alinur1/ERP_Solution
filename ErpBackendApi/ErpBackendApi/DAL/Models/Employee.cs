using System.ComponentModel.DataAnnotations.Schema;
using ErpBackendApi.DAL.Enums;

namespace ErpBackendApi.DAL.Models
{
    public class Employee
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public int? department_id { get; set; }
        public DateOnly? date_hired { get; set; }
        public decimal? salary { get; set; }
        //[Column(TypeName = "int")]
        public EmployeeStatus? status { get; set; }
        public bool? is_deleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}