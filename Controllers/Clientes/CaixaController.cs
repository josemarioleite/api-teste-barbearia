using API.Brokers;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers.Clientes
{
    [Route("api/v1/caixa")]
    [ApiController]
    [Authorize]
    public class CaixaController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public CaixaController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet("produto")]
        public async Task<ActionResult> ObterTotalProfissionalProduto()
        {
            var caixa = await _database.Caixa
                                    .FromSqlRaw("Select * From caixa() where \"TipoItem\" = 'P';")
                                    .ToListAsync();

            if (caixa != null)
            {
                return Ok(caixa);
            }
            else
            {
                return Ok(new
                {
                    status = false,
                    msg = "Não tem caixa hoje"
                });
            }
        }

        [HttpGet("servico")]
        public async Task<ActionResult> ObterTotalProfissionalServico()
        {
            var caixa = await _database.Caixa
                                    .FromSqlRaw("Select * From caixa() where \"TipoItem\" = 'S';")
                                    .ToListAsync();

            if (caixa != null)
            {
                return Ok(caixa);
            }
            else
            {
                return Ok(new
                {
                    status = false,
                    msg = "Não tem caixa hoje"
                });
            }
        }
    }
}