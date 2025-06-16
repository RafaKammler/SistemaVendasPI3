using System;
using System.Collections.Generic;

namespace ProjetoIntegradorVendas.Classes
{
    public class Cliente
    {
        public int ClienteID { get; set; }
        public string ClienteNome { get; set; }
        public string ClienteCPF { get; set; }
        public string Senha { get; set; }
        public string ClienteCep { get; set; }
        public DateTime ClienteDataNascimento { get; set; }
        public string ClienteTelefone { get; set; }
        public char ClienteSexo { get; set; }

        public Cidade Cidade { get; set; }

        public int CidadeID { get; set; }

        public List<Comentario> Comentarios { get; set; }

        public Cliente()
        {
            Comentarios = new List<Comentario>();
        }
    }
}

