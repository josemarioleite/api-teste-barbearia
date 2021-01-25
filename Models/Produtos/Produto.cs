using System.ComponentModel.DataAnnotations;

namespace API.Models.Produtos
{
    public class Produto : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public double? Valor { get; set; }
        public string Ativo { get; set; } = "S";
    }
}