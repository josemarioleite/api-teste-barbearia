using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Brokers;
using API.Models.Situacao;
using API.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Empresa.Controllers.Clientes
{
    [Route("api/v1/situacao")]
    [ApiController]
    [Authorize]
    public class SituacaoAtendimentoController : ControllerBase
    {
        private readonly ClienteDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public SituacaoAtendimentoController(ClienteDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]  
        public async Task<ActionResult<List<SituacaoAtendimento>>> ObterSituacaoAtendimento()
        {
            var situacao =  await _database.SituacaoAtendimento
                                    .AsNoTracking()
									.Where(s => s.Ativo == "S")
                                    .OrderByDescending(s => s.Id)
                                    .ToListAsync();
            
            if (situacao != null)
            {
                return Ok(situacao);
            } else {
                return Ok (new {
                    status = false,
                    msg = "Não existe Situação Atendimento cadastrado no momento"
                });
            }
        }
    }
}