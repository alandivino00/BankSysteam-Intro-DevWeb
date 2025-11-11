namespace BankSysteam.Api.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ContaInputModel
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Numero { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Titular { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Saldo { get; set; }

        [Required]
        public Guid ClienteId { get; set; }
    }
}