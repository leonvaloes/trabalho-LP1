using IntroAPI.Models;
using IntroAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IntroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransacaoController : ControllerBase
    {
        private readonly ILogger<TransacaoController> _logger;
        private readonly TransacaoRepository _transacaoRepository;

        public TransacaoController(ILogger<TransacaoController> logger, TransacaoRepository transacaoRepository)
        {
            _logger = logger;
            _transacaoRepository = transacaoRepository;
        }

        // GET api/transacao
        [HttpGet]
        public ActionResult<IEnumerable<Transacao>> Get()
        {
            try
            {
                var transacoes = _transacaoRepository.ObterTodos(); // Chama o método para obter as transações

                return Ok(transacoes); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter transações.");
                return BadRequest($"Erro ao obter transações: {ex.Message}");
            }
        }

    }
}
