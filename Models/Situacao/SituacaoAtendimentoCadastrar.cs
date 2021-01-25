using System.ComponentModel.DataAnnotations;

namespace API.Models.Situacao
{
    public class SituacaoAtendimentoCadastrar
    {
        [Required(ErrorMessage = "O campo Descrição é obrigatório")]
        public string Descricao { get; set; }
        public string Ativo { get; set; } = "S";
    }
}