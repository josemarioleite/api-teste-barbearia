using System.ComponentModel.DataAnnotations;

namespace API.Models.ClientesUsuarios
{
    public class ClienteUsuario : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Ativo { get; set; }
        public string PrimeiroAcesso { get; set; }
        public string Root { get; set; }
        public byte[] SenhaDificuldade { get; set; }
        public byte[] SenhaHash { get; set; }
    }
}
