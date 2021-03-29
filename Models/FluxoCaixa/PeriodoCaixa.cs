using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.FluxoCaixa
{
    public class PeriodoCaixa
    {
        [Key]
        public int Id { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime DataFechamento { get; set; }
        public double ValorTroco { get; set; }
        public double ValorSaldo { get; set; }
        public double ValorSangria { get; set; }
        public int UsuarioAberturaId { get; set; }
        public int UsuarioFechamentoId { get; set; }
        public int CaixaId { get; set; }
        public string Observacao { get; set; }
    }
}