namespace BankSysteam.Api.Models
{
    public class ContaViewModel
    {
        public int Numero { get; set; }
        public string Titular { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
    }
}