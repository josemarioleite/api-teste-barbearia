using System.ComponentModel.DataAnnotations;

namespace API.Models.FormaPagamentos
{
    public class FormaPagamento : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Titulo { get; set; }
        public string Ativo { get; set; } = "S";
    }
}