using System.ComponentModel.DataAnnotations;

namespace API.Models.ClientesEmpresas
{
    public class Cliente : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string TelefoneCelular { get; set; }
        public string Ativo { get; set; }
    }
}
