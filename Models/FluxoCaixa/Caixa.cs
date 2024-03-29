using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models.FluxoCaixa
{
    public class Caixa : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CaixaAberto { get; set; }
        public string Ativo { get; set; }
		public string Observacao { get; set; }
    }
}