using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using IntroAPI.Repository;

namespace IntroAPI.Controllers
{

    /// <summary>
    /// Gerenciamento de Alunos
    /// </summary>
   // [Authorize("APIAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class AlunosController : ControllerBase
    {
        private readonly ILogger<AlunosController> _logger;
        private readonly UsuarioAutenticado _usuarioAutenticado;
        public AlunosController(IHttpContextAccessor httpContextAccessor, ILogger<AlunosController> logger)
        {
            _logger = logger;
            _usuarioAutenticado = new UsuarioAutenticado(httpContextAccessor);
        }

        private static List<Aluno> _alunos = new List<Aluno>()
        {
            new Aluno { Id = 1, Nome = "Joao", Idade = 18 },
            new Aluno {Id = 2, Nome = "Maria", Idade= 20}
        };

        /// <summary>
        /// Retorna todos os alunos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Aluno>> Get()
        {
            try
            {
                string nome = _usuarioAutenticado.Nome;

                int.Parse("sdshdgsjds");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter os alunos");
            }

            return Ok(_alunos);
        }

        /// <summary>
        /// Retorna um aluno pelo Ir
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Não encontrado.</returns>
        [HttpGet("{id}")]
        public ActionResult<Aluno> Get(int id)
        {
            AlunoRepository rep = new AlunoRepository();
            rep.ObterPorId(1);

            var aluno = _alunos.FirstOrDefault(a => a.Id == id);

            if (aluno == null)
                return NotFound("Não encontrado");
            else return Ok(aluno);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Aluno aluno) {

            
            aluno.Id = _alunos.Count + 1;
            _alunos.Add(aluno);

            return Created("/" + aluno.Id, aluno);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Aluno alunoAlterado)
        {
            var aluno = _alunos.FirstOrDefault(a => a.Id == id);

            if (aluno == null)
                return NotFound("Não encontrado");

            aluno.Nome = alunoAlterado.Nome;
            aluno.Idade = alunoAlterado.Idade;

            return Ok(aluno);

        }

        [HttpPatch("{id}")]
        public ActionResult Patch(int id, [FromBody] JsonElement update)
        {
            var aluno = _alunos.FirstOrDefault(a => a.Id == id);

            if (aluno == null)
                return NotFound("Não encontrado");


            if (update.TryGetProperty("Nome", out var nome))
                aluno.Nome = nome.GetString();


            if (update.TryGetProperty("Idade", out var idade))
                aluno.Idade = idade.GetInt32();


            return Ok(aluno);

        }

        [HttpDelete]
        public ActionResult Delete(int id) {

            var aluno = _alunos.FirstOrDefault(a => a.Id == id);

            if (aluno == null)
                return NotFound("Não encontrado");

            _alunos.Remove(aluno);

            return Ok();

        }

        [HttpGet("{id}/ano-nascimento/")]
        public ActionResult<int> GetAnoNascimento(int id)
        {
            var aluno = _alunos.FirstOrDefault(a => a.Id == id);

            if (aluno == null)
                return NotFound("Não encontrado");

            int anoAtual = DateTime.Now.Year;

            int anoNascimento = anoAtual - aluno.Idade;

            return Ok(anoNascimento);
        }


        [HttpGet("pesquisar")]
        public ActionResult<IEnumerable<Aluno>> Pesquisar([FromQuery] string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("O nome é inválido");

            var alunos = _alunos.Where(a => a.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));

            if (!alunos.Any())
                return NotFound("Nada encontrado.");

            return Ok(alunos);
        }


        [HttpPost("{id}/foto")]
        public async Task<ActionResult> UploadFoto(int id, IFormFile foto)
        {
            var aluno = _alunos.FirstOrDefault(a => a.Id == id);

            if (aluno == null)
                return NotFound("Não encontrado");

            try
            {
                if (foto == null || foto.Length == 0)
                {
                    return BadRequest("Nenhuma foto enviada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
            var extensao = Path.GetExtension(foto.FileName);

            if (!extensoesPermitidas.Contains(extensao))
                return BadRequest("Formato inválido.");

            //????/foto   \foto
            var caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"fotos");
            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            //???/foto/1.jpg
            var caminhoArquivo = Path.Combine(caminho, $"{id}{extensao}");

            using(var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
                //foto.CopyTo(stream);
                
             

            }

            return Ok("Foto enviada com sucesso.");

        }

    }
}
