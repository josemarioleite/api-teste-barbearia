using API.Models.Empresas;
using API.Models.Grupos;
using API.Models.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Brokers
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options) {}

        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<GrupoEmpresa> GrupoEmpresa { get; set; }
    }
}