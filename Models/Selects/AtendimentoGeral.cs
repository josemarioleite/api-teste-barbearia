using System.ComponentModel.DataAnnotations;

namespace API.Models.Selects
{
    public class AtendimentoGeral
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public double Valor { get; set; }
    }
}