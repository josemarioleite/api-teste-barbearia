using System.ComponentModel.DataAnnotations;

namespace API.Models.ClientesEmpresas
{
    public class ClienteCadastrar
    {
        [Required(ErrorMessage = "É necessário informar o Nome do Cliente")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "É necessário informar o Telefone do Cliente")]
        public string TelefoneCelular { get; set; }
        public string Ativo { get; set; } = "S";
    }
}