using System.ComponentModel.DataAnnotations;

namespace API.Models.FormaPagamentos
{
    public class FormaPagamentoCadastrar
    {
        [Required(ErrorMessage = "Informa uma descrição para a Forma de Pagamento")]
        public string Descricao { get; set; }
        [Required(ErrorMessage = "Informe uma abreviação para identificação da Forma de Pagamento")]
        public string Titulo { get; set; }
        public string Ativo { get; set; } = "S";
    }
}