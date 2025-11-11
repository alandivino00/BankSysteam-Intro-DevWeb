using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace BankSysteam.Api.models
{
    public class Conta
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Stored as string to allow MaxLength(20) mapping in BankContext
        [Required]
        [MaxLength(20)]
        public string Numero { get; set; }

        // Column mapped as decimal(18,2) in BankContext
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        // Alias property used by existing controller code: keep compatibility
        [NotMapped]
        public decimal Saldo
        {
            get => Balance;
            set => Balance = value;
        }

        [Required]
        public Tipo Tipo { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Alias for CreatedAt used in some code (DataCriacao)
        [NotMapped]
        public DateTime DataCriacao
        {
            get => CreatedAt;
            set => CreatedAt = value;
        }

        // Foreign key and navigation
        public Guid? ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public Conta() { }

        public Conta(string numero, decimal balance, Guid? clienteId = null)
        {
            Numero = numero;
            Balance = balance;
            ClienteId = clienteId;
        }
    }
}
