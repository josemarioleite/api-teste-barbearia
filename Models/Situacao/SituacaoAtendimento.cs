using System.ComponentModel.DataAnnotations;

namespace API.Models.Situacao
{
    public class SituacaoAtendimento : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Ativo { get; set; }
    }
}