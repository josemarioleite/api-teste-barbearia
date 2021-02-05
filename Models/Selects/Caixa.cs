using System.ComponentModel.DataAnnotations;

namespace API.Models.Selects
{
    public class Caixa
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string TipoItem { get; set; }
        public double ValorTotal { get; set; }
        public double PorcentagemServico { get; set; }
        public string GeraPorcentagemProduto { get; set; }
        public double PorcentagemProduto { get; set; }
        public double TotalGeral { get; set; }
    }
}