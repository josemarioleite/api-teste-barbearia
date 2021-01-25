using System;
using System.ComponentModel.DataAnnotations;
using API.Models.Grupos;

namespace API.Models.Empresas
{
    public class Empresa : PadraoSistema
    {
        [Key]
        public int Id { get; set; }
        public int GrupoEmpresaId { get; set; }
        public GrupoEmpresa GrupoEmpresa { get; set; }
        public string Nome { get; set; }
        public Guid UUID { get; set; }
        public string StringConexao { get; set; }
        public char Ativo { get; set; }
    }
}