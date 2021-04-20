using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.FluxoCaixa
{
    public class PeriodoCaixaCadastro
    {
        [Required(ErrorMessage = "É necessário incluir a Data de Abertura do Caixa")]
        public DateTime DataAbertura { get; set; } = DateTime.Now;
        public DateTime? DataFechamento { get; set; }
        [Required(ErrorMessage = "É necessário informar o Valor para troco")]
        public double ValorTroco { get; set; }
        [Required(ErrorMessage = "É necessário informar o Saldo anterior")]
        public double ValorSaldo { get; set; } = 0;
        [Required(ErrorMessage = "É necessário informar o Valor de retirada")]
        public double ValorSangria { get; set; } = 0;
		[Required(ErrorMessage = "É necessário informar o Valor Total")]
		public double ValorTotal { get; set; } = 0;
        [Required(ErrorMessage = "É necessário informat o Valor do Período")]
        public double ValorPeriodo { get; set; } = 0;
        [Required(ErrorMessage = "É necessário incluir o usuário que abriu o caixa")]
        public int UsuarioAberturaId { get; set; }
        public int? UsuarioFechamentoId { get; set; }
        [Required(ErrorMessage = "É necessário informar a qual caixa pertence o período")]
        public int CaixaId { get; set; }
        public string Status { get; set; } = "A";
        public string Observacao { get; set; } = "S/ Observação";
    }
}