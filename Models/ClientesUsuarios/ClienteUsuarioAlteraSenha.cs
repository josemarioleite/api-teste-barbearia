using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models.ClientesUsuarios
{
    public class ClienteUsuarioAlteraSenha
    {
        [Required(ErrorMessage = "É necessário saber o ID do usuário")]
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo 'Senha' é obrigatório")]
        public string Senha { get; set; }
        public char PrimeiroAcesso { get; set; }
        [JsonIgnore]
        public byte[] SenhaDificuldade { get; set; }
        [JsonIgnore]
        public byte[] SenhaHash { get; set; }
    }
}
