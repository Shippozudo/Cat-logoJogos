using ApiCatalogoJogos.Exceptions;
using ApiCatalogoJogos.InputModel;
using ApiCatalogoJogos.Services;
using ApiCatalogoJogos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {

        private readonly IJogoService _jogoService;
        
        public JogosController(IJogoService jogoService)
        {
            _jogoService = jogoService;
        }


        /// <summary>
        /// Busca todos os jogos de forma paginada;
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="quantidade"></param>
        /// <response code="200"> Retorna a lista de jogos</response>
        /// <Response code="204">Caso não haja johos</Response>
        /// <returns></returns>
        /// 
        [HttpGet]
        public  async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quantidade = 5)
        {
           
            var jogos = await _jogoService.Obter(pagina, quantidade);
            
            if (jogos.Count() == 0) // nao sei se é '==' ou '--'
                    return NoContent();
            
            
            
            return Ok(jogos);
        }

        /// <summary>
        /// Busca todos os jogos de forma paginada;
        /// </summary>
        /// <param name="idJogo"></param>
        /// <response code="200"> Retorna o jogo filtrado</response>
        /// <Response code="204">Caso não haja jogos com este id</Response>
        /// <returns></returns>
        /// 
        [HttpGet("{idJogo:guid}")]
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo)
        {
            var jogo = await _jogoService.Obter(idJogo);

            if (jogo == null)
                return NoContent();

            return Ok(jogo);
        }

        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody] JogoInputModel JogoInputModel)
        {

            try
            {
                var jogo = await _jogoService.Inserir(JogoInputModel);
                return Ok(jogo);
            }
            catch (JogoJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com este nome para esta produtora");

            }
            
        }

        [HttpPut("{idJogo:guid}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromBody] JogoInputModel JogoInputModel)
        {
            try
            {
                 await _jogoService.Atualizar(idJogo, JogoInputModel);
                 return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            
            {
                return NotFound("Não existe este jogo");
            }
        }

        [HttpPatch("{idJogo:guid}/preco/{preco:double}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] double preco)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, preco);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> ApagarJogo([FromRoute] Guid idJogo)
        {
            try
            {
                await _jogoService.Remover(idJogo);
                return Ok();
            }
           
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
            
        }
    }
}
