using System.ComponentModel.DataAnnotations;

namespace ErpBackendApi.DAL.DTOs
{
    public class LoginDTO
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
