using System.ComponentModel.DataAnnotations;
using API.Models.Atendimentos;

namespace API.Models.ItensAtendimento
{
    public class ItemAtendimento : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string TipoItem { get; set; }
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
        public double ValorUnitario { get; set; }
        public double ValorTotal { get; set; }
        public int AtendimentoId { get; set; }
    }
}