using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetoIntegradorVendas.Classes;

namespace ProjetoIntegradorVendas.Services
{
    class FreteService
    {
        private static readonly Dictionary<string, decimal> TabelaFretePorUF = new Dictionary<string, decimal>
        {
            // Região Sudeste
            { "SP", 15.00m },
            { "RJ", 18.50m },
            { "MG", 20.00m },
            { "ES", 22.00m },

            // Região Sul
            { "RS", 12.00m },
            { "SC", 14.00m },
            { "PR", 16.00m },

            // Região Centro-Oeste
            { "GO", 25.00m },
            { "MT", 28.00m },
            { "MS", 26.50m },
            { "DF", 24.00m },

            // Região Nordeste
            { "BA", 30.00m },
            { "SE", 32.00m },
            { "AL", 32.50m },
            { "PE", 33.00m },
            { "PB", 34.00m },
            { "RN", 35.00m },
            { "CE", 36.00m },
            { "PI", 37.00m },
            { "MA", 38.00m },

            // Região Norte
            { "TO", 40.00m },
            { "PA", 42.00m },
            { "AP", 45.00m },
            { "AM", 50.00m },
            { "RR", 52.00m },
            { "AC", 55.00m },
            { "RO", 48.00m }
        };

        private const decimal FretePadrao = 25.00m; // Um valor padrão caso a UF não seja encontrada

        public decimal CalcularFrete(Endereco endereco)
        {
            if (endereco == null || string.IsNullOrWhiteSpace(endereco.Uf))
            {
                return 0m;
            }


            if (TabelaFretePorUF.TryGetValue(endereco.Uf.ToUpper(), out decimal valorFrete))
            {
                return valorFrete;
            }

            return FretePadrao;
        }
    }
}
