using System.ComponentModel.DataAnnotations;

namespace API.Models.Selects
{
    public class AtendimentoGeralFormaPagamento
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public double Total { get; set; }
    }
}