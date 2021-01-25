using System.ComponentModel.DataAnnotations;

namespace API.Models.ClientesUsuarios
{
    public class ClienteUsuario
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public char Ativo { get; set; }
        public char PrimeiroAcesso { get; set; }
        public byte[] SenhaDificuldade { get; set; }
        public byte[] SenhaHash { get; set; }
    }
}
