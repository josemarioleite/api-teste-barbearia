using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Atendimentos
{
    public class AtendimentoAlterar
    {
        [Key]
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ProfissionalId { get; set; }
        public int SituacaoId { get; set; }
        public string Ativo { get; set; }
    }
}