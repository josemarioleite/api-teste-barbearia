using System.ComponentModel.DataAnnotations;

namespace API.Models.FormaPagamentos
{
    public class ItemFormaPagamento : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public int AtendimentoId { get; set; }
        public int FormaPagamentoId { get; set; }
        public FormaPagamento FormaPagamento { get; set;}
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public string Ativo { get; set; }
        //public int PeriodoCaixaId { get; set; }
    }
}