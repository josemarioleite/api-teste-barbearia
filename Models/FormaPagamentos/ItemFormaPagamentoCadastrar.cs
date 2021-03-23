using System.ComponentModel.DataAnnotations;

namespace API.Models.FormaPagamentos
{
    public class ItemFormaPagamentoCadastrar
    {
        [Required(ErrorMessage = "É necessário informar qual atendimento pertence")]
        public int AtendimentoId { get; set; }
        [Required(ErrorMessage = "É necessário informar a forma de pagamento")]
        public int FormaPagamentoId { get; set; }
        [Required(ErrorMessage = "É necessário informar o Valor")]
        public double Valor { get; set; }
        [Required(ErrorMessage = "É necessário informar a Descrição")]
        public string Descricao { get; set; }
        public string Ativo { get; set; } = "S";
        //[Required(ErrorMessage = "É necessário informar o Perído Caixa")]
        //public int PeriodoCaixaId { get; set; }
    }
}