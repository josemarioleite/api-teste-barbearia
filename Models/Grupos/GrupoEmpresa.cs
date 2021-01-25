using System.ComponentModel.DataAnnotations;

namespace API.Models.Grupos
{
    public class GrupoEmpresa : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
    }
}