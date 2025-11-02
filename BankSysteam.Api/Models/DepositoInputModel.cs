using System.ComponentModel.DataAnnotations;

namespace BankSysteam.Api.Models
{
    public class DepositoInputModel
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do depósito deve ser maior que zero.")]
        public decimal Valor { get; set; }
    }
}