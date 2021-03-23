using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Profissionais
{
    public class ProfissionalCadastrar
    {
        [Required(ErrorMessage = "É necessário informar o Nome do profissional")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "É necessário informar a Data de Nascimento do profissional")]
        public DateTime DataNascimento { get; set; }
        [Required(ErrorMessage = "É necessário informar o CPF do Profissional")]
        public string CPF { get; set; }
        [Required(ErrorMessage = "É necessário informar o Telefone/Celular do Profissional")]
        public string TelefoneCelular { get; set; }
        [Required(ErrorMessage = "É necessário informar a Porcentagem do Profissional")]
        public double Porcentagem { get; set; }
        public string Ativo { get; set; } = "S";
    }
}