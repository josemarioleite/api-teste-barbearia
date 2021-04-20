using System.ComponentModel.DataAnnotations;
using API.Models.ClientesEmpresas;
using API.Models.FormaPagamentos;
using API.Models.Profissionais;
using API.Models.Situacao;

namespace API.Models.Atendimentos
{
    public class Atendimento : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int ProfissionalId { get; set; }
        public Profissional Profissional { get; set; }
        public int SituacaoId { get; set; }
        public SituacaoAtendimento Situacao { get; set; }
        public string Ativo { get; set; }
		public int PeriodoCaixaId { get; set; }
    }
}