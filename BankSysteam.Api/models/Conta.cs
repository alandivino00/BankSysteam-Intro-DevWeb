using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankSysteam.Api.models
{
    public class Conta
    {
        [Key]    
        public Guid id { get; set; }

        [Required]
        [MaxLength(20)]
        public int Numero { get; }

        //[Required]
        //public string Titular { get; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; }
        
        [Required]
        public Tipo Tipo { get; set; }
        
        [Required]
        public DateTime DataCriacao { get; set; }
        
        [Required]
        public Status Status { get; set; }


        public Conta(int numero, decimal saldo)
        {
            Numero = numero;
            Saldo = saldo;          
        }

        /*
        public override string ToString()
        {
            return $"{Numero} - {Titular} | Saldo: {Saldo:C}";
        }
        */
    }
}
