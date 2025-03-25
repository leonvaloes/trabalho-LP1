using IntroAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace IntroAPI.Controllers
{
    [ApiController]
    [Route("api/cartoes")]
    public class CartaoController : ControllerBase
    {
        private readonly CartaoService _cartaoService;

        public CartaoController(CartaoService cartaoService)
        {
            _cartaoService = cartaoService;
        }

        /// <summary>
        /// Obtém a bandeira de um cartão de crédito com base no número informado.
        /// </summary>
        /// <param name="cartao">Número do cartão de crédito (16 dígitos)</param>
        /// <returns>Retorna a bandeira do cartão ou um erro 404 se desconhecida.</returns>
        /// <response code="200">Retorna a bandeira do cartão</response>
        /// <response code="404">Bandeira desconhecida ou número do cartão inválido</response>
        [HttpGet("{cartao}/obter-bandeira")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult ObterBandeira(string cartao)
        {
            Log.Information("Recebida solicitação para obter a bandeira do cartão: {Cartao}", cartao);

            var bandeira = _cartaoService.ObterBandeira(cartao);

            if (bandeira == null)
            {
                Log.Warning("Bandeira desconhecida ou número do cartão inválido: {Cartao}", cartao);
                return NotFound("Bandeira desconhecida ou número do cartão inválido.");
            }

            Log.Information("Bandeira identificada: {Bandeira} para o cartão {Cartao}", bandeira, cartao);
            return Ok(new { bandeira });
        }

        /// <summary>
        /// Valida Um cartão com base no banco de dados
        /// </summary>
        /// <param name="cartao">Número do cartão de crédito (16 dígitos)</param>
        /// <returns>Retorna Veradeiro ou falso ou um erro 404 se desconhecida.</returns>
        [HttpGet("{cartao}/valido")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult CartaoValido(string cartao)
        {
            Log.Information("Recebida solicitação para validar o cartão: {Cartao}", cartao);

            var valido = _cartaoService.Validar(cartao);

            if (!valido)
            {
                Log.Warning("Cartão inválido: {Cartao}", cartao);
                return NotFound("Cartão inválido.");
            }

            Log.Information("Cartão válido: {Cartao}", cartao);
            return Ok(new { valido });
        }
    }
}
