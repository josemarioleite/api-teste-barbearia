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
        public double Porcentagem { get; set; }
        public string Ativo { get; set; }
    }
}