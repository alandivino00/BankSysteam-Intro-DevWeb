namespace BankSysteam.Api.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ContaInputModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Numero { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Titular { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Saldo { get; set; }
    }
}