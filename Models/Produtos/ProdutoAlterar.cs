using System.ComponentModel.DataAnnotations;

namespace API.Models.Produtos
{
    public class ProdutoAlterar
    {
        [Key]
        [Required(ErrorMessage = "É obrigatório colocar o Id do Produto")]
        public int Id { get; set; }
        public string Nome { get; set; }
        public double Valor { get; set; }
    }
}