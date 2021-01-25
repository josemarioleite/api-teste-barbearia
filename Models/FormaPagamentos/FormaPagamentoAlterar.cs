using System.ComponentModel.DataAnnotations;

namespace API.Models.FormaPagamentos
{
    public class FormaPagamentoAlterar
    {
        [Required(ErrorMessage = "É necessário informar o Id da forma de pagamento")]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Titulo { get; set; }
        public string Ativo { get; set; } = "S";
    }
}