using API.Models.Empresas;
using API.Models.FormaPagamentos;
using API.Models.Atendimentos;
using API.Models.Produtos;
using API.Models.Profissionais;
using API.Models.Usuarios;
using AutoMapper;
using API.Models.Servicos;
using API.Models.ClientesEmpresas;
using API.Models.ItensAtendimento;
using API.Models.Situacao;
using API.Models.ClientesUsuarios;
using API.Models.FluxoCaixa;

namespace Utils
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<UsuarioCadastro, Usuario>();
            CreateMap<ClienteUsuarioCadastro, ClienteUsuario>(); 
            CreateMap<EmpresaCadastro, Empresa>();
            CreateMap<ClienteCadastrar, Cliente>();
            CreateMap<ProdutoCadastrar, Produto>();
            CreateMap<AtendimentoCadastrar, Atendimento>();
            CreateMap<FormaPagamentoCadastrar, FormaPagamento>();
            CreateMap<ProfissionalCadastrar, Profissional>();
            CreateMap<ServicoCadastrar, Servico>();
            CreateMap<ItemFormaPagamentoCadastrar, ItemFormaPagamento>();
            CreateMap<ItemAtendimentoCadastrar, ItemAtendimento>();
            CreateMap<SituacaoAtendimentoCadastrar, SituacaoAtendimento>();
            CreateMap<CaixaCadastro, Caixa>();
            CreateMap<PeriodoCaixaCadastro, PeriodoCaixa>();
        }
    }
}