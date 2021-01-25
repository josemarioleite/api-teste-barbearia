using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Atendimentos
{
    public class AtendimentoCadastrar
    {
        [Required(ErrorMessage = "É necessário preencher o campo Id do Cliente")]
        public int ClienteId { get; set; }
        [Required(ErrorMessage = "É necessário preencher o campo o Profissional")]
        public int ProfissionalId { get; set; }
        [Required(ErrorMessage = "É necessário preencher o campo Situação")]
        public int SituacaoId { get; set; } = 1;
        public string Ativo { get; set; } = "S";
    }
}