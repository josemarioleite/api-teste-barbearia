using System.ComponentModel.DataAnnotations;

namespace API.Models.Produtos
{
    public class ProdutoCadastrar
    {
        [Required(ErrorMessage = "É obrigatório colocar o nome do Produto")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar o Valor do produto")]
        public double Valor { get; set; }
    }
}