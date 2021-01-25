using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Profissionais
{
    public class Profissional : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string TelefoneCelular { get; set; }
        public string CPF { get; set; }
        public int Porcentagem { get; set; }
		public string GeraPorcentagemProduto { get; set; }
		public int? PorcentagemProduto { get; set; }
        public string Ativo { get; set; }
    }
}