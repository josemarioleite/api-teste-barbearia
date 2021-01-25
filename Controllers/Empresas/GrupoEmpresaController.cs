using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Grupos;
using API.Utils;
using Brokers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Empresas
{
    [Route("api/v1/empresa/grupo")]
    [ApiController]
    [Authorize]
    public class GrupoEmpresaController : ControllerBase
    {
        private readonly Database _database;
        private readonly JWT _jwt;
        public GrupoEmpresaController(Database database)
        {
            _database = database;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<GrupoEmpresa>>> ObterGrupoEmpresa()
        {
            return await _database.GrupoEmpresa.AsNoTracking().ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CadastraNovoGrupoEmpresa(GrupoEmpresa grupo)
        {
            if (ModelState.IsValid)
            {
                GrupoEmpresa gp = new GrupoEmpresa
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = await _jwt.RetornaIdUsuarioDoToken(HttpContext),
                    Descricao = grupo.Descricao
                };

                _database.Add(gp);
                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new
                    {
                        status = true,
                        msg = $"O Grupo-Empresa {grupo.Descricao} foi cadastrado com sucesso!"
                    });
                } catch (Exception e)
                {
                    return Ok(new
                    {
                        status = false,
                        msg = "Não foi possível cadastrar o Grupo-Empresa",
                        erro = e.Message
                    });
                }
            } else {
                return BadRequest(new
                {
                    status = false,
                    msg = "Erro, verificar..."
                });
            }
        }
    }
}