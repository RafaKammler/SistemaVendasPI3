using System;

namespace ProjetoIntegradorVendas.Classes
{
    public class Comentario
    {
        public int ComentarioID { get; set; }
        public string ComentarioTexto { get; set; }
        public DateTime DataComentario { get; set; }

        public Produto Produto { get; set; }

        public Cliente Cliente { get; set; }

        public int ProdutoID { get; set; }
        public int ClienteID { get; set; }
    }
}