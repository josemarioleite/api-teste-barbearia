using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models.ClientesUsuarios
{
    public class ClienteUsuarioCadastro
    {
        [Required(ErrorMessage = "O Campo 'Nome' é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O Campo 'Login' é obrigatório")]
        public string Login { get; set; }
        public char Ativo { get; set; } = 'S';
        public char PrimeiroAcesso { get; set; } = 'S';
        [Required(ErrorMessage = "O Campo 'Senha' é obrigatório")]
        public string Senha { get; set; }
        [JsonIgnore]
        public byte[] SenhaDificuldade { get; set; }
        [JsonIgnore]
        public byte[] SenhaHash { get; set; }
    }
}
