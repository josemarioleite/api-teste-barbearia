using System.ComponentModel.DataAnnotations;

namespace API.Models.Servicos
{
    public class Servico : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public string Ativo { get; set; }
    }
}