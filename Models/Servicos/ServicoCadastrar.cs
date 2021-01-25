using System.ComponentModel.DataAnnotations;

namespace API.Models.Servicos
{
    public class ServicoCadastrar
    {
        [Required(ErrorMessage = "É necessário informar a Descrição do Serviço")]
        public string Descricao { get; set; }
        [Required(ErrorMessage = "É necessário informar o Valor do Serviço")]
        public double Valor { get; set; }
        [Required(ErrorMessage = "É necessário informar a Quantidade do Serviço")]
        public string Ativo { get; set; } = "S";
    }
}