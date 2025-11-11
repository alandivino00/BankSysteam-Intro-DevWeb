using System;
using System.Collections.Generic;

namespace BankSysteam.Api.models
{
    public class Cliente
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nome { get; set; } = string.Empty;           

        public string Email { get; set; } = string.Empty;

        // Name kept as 'Conta' to match the existing fluent mapping in BankContext (.WithMany(a => a.Conta))
        public ICollection<Conta> Conta { get; set; } = new List<Conta>();
    }
}
