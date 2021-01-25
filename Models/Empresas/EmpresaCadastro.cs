using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Empresas
{
    public class EmpresaCadastro
    {
        [Required(ErrorMessage = "É necessário saber a qual Grupo a Empresa pertence")]
        public int GrupoEmpresaId { get; set; }
        [Required(ErrorMessage = "O Campo 'Nome' é necessário")]
        public string Nome { get; set; }
        public string StringConexao { get; set; }
        public char Ativo { get; set; } = 'S';
    }
}