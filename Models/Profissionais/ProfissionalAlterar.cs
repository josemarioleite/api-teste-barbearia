using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Profissionais
{
    public class ProfissionalAlterar
    {
        [Key]
        [Required(ErrorMessage = "É necessário informar o Id do Profissional")]
        public int Id { get; set; }
        public string Nome { get; set; }        
        public DateTime DataNascimento { get; set; }
        public string TelefoneCelular { get; set; }
        public int Porcentagem { get; set; }
		public string GeraPorcentagemProduto { get; set; }
		public int? PorcentagemProduto { get; set; }
        public string CPF { get; set; }
        public string Ativo { get; set; } = "S";
    }
}