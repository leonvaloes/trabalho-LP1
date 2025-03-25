using IntroAPI.DTOs;
using IntroAPI.Enums;
using IntroAPI.Models;
using IntroAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntroAPI.Controllers
{
    [ApiController]
    [Route("api/pagamentos/")]
    public class TransacaoController : ControllerBase
    {
        private readonly ILogger<TransacaoController> _logger;
        private readonly TransacaoService _transacaoService;
        private readonly CartaoService _cartaoService;
        private readonly CartaoService cartaoService;
        public TransacaoController(ILogger<TransacaoController> logger, TransacaoService transacaoService, CartaoService cartaoService)
        {
            _transacaoService = transacaoService;
            _cartaoService = cartaoService;
            _logger = logger;
        }

        [HttpPost("calcular-parcelas")]
        public ActionResult<PagamentoResponse> CalcularParcelas([FromBody]PagamentoRequest pagamentoRequest)
        {
            try
            {
                var pagamentoResponse = _transacaoService.CalcularParcelas(pagamentoRequest);

                return Ok(pagamentoResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular parcelas.");
                return BadRequest($"Erro ao calcular parcelas: {ex.Message}");
            }
        }

        [HttpPost("pagamentos")]
        public ActionResult<PagamentoResponse> EfetuarPagamento([FromBody]CriarPagamentoDTO criarPagamentoDTO)
        {
            try
            {
                if (cartaoService.Validar(criarPagamentoDTO.Cartao))
                {
                    return BadRequest("Cartão inválido.");
                }

                var pagamentoResponse = _transacaoService.EfetuarPagamento(criarPagamentoDTO);

                return Ok(pagamentoResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao efetuar pagamento.");
                return BadRequest($"Erro ao efetuar pagamento: {ex.Message}");
            }
        }
        [HttpGet("{id}/situacao")]
        public ActionResult<SituacaoEnum> ConsultarSituacaoPagamento([FromBody]int id)
        {
            try
            {
                var situacao = _transacaoService.getStatusTransacao(id);

                return Ok(situacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar situação do pagamento.");
                return BadRequest($"Erro ao consultar situação do pagamento: {ex.Message}");
            }
        }

        [HttpPut("{id}/confirmar")]
        public ActionResult ConfirmarPagamento([FromBody]int id)
        {
            try
            {
                return Ok(_transacaoService.ConfirmarPagamento(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar pagamento.");
                return BadRequest($"Erro ao confirmar pagamento: {ex.Message}");
            }
        }

        [HttpPut("{id}/cancelar")]
        public ActionResult CancelarPagamento([FromBody]int id)
        {
            try
            {
                return Ok(_transacaoService.CancelarPagamento(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar pagamento.");
                return BadRequest($"Erro ao cancelar pagamento: {ex.Message}");
            }
        }

    }
}
