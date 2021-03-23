using API.Models.ClientesEmpresas;
using API.Models.ClientesUsuarios;
using API.Models.Empresas;
using API.Models.FormaPagamentos;
using API.Models.Atendimentos;
using API.Models.ItensAtendimento;
using API.Models.Produtos;
using API.Models.Profissionais;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using API.Models.Servicos;
using API.Models.Situacao;
using API.Models.Selects;
using API.Models.FluxoCaixa;

namespace API.Brokers
{
    public class ClienteDatabase : DbContext
    {
        private Empresa empresa;
        public DbSet<ClienteUsuario> Usuario { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Atendimento> Atendimento { get; set; }
        public DbSet<AtendimentoGeral> AtendimentoGeral { get; set; }
		public DbSet<AtendimentoGeralFormaPagamento> AtendimentoGeralFormaPagamento { get; set; }
        public DbSet<FormaPagamento> FormaPagamento { get; set; }
        public DbSet<Profissional> Profissional { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<ItemAtendimento> ItemAtendimento { get; set; }
        public DbSet<ItemFormaPagamento> ItemFormaPagamento { get; set; }
        public DbSet<SituacaoAtendimento> SituacaoAtendimento { get; set; }
        public DbSet<CaixaOperador> CaixaOperador { get; set; }
        public DbSet<Caixa> Caixa { get; set; }
        public DbSet<PeriodoCaixa> PeriodoCaixa { get; set; }

        public ClienteDatabase(DbContextOptions<ClienteDatabase> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                empresa = (Empresa)httpContextAccessor.HttpContext.Items["EMPRESA"];
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(empresa.StringConexao);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
