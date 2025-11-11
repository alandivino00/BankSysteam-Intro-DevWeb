namespace BankSysteam.Api.Models
{
    public class ContaViewModel
    {
        public string Numero { get; set; } = string.Empty;
        public string Titular { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
    }
}