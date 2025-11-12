using System.ComponentModel.DataAnnotations;

namespace BankSysteam.Api.Models
{
    public class ClienteInputModel
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
