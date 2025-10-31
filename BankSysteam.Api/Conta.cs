namespace BankSysteam.Api
{
    public class Conta
    {
        public int Numero { get; }
        public string Titular { get; }
        public decimal Saldo { get; set; }
    

        public Conta(int numero, string titular, decimal saldo)
        {
            Numero = numero;
            Titular = titular;
            Saldo = saldo;          
        }


        public override string ToString()
        {
            return $"{Numero} - {Titular} | Saldo: {Saldo:C}";
        }
    }
}
