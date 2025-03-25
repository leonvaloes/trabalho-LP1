using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using IntroAPI.DTOs;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace IntroAPI.Controllers
{

    [Authorize("APIAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class MatriculasController : ControllerBase
    {
        private readonly ILogger<MatriculasController> _logger;

        private static List<Aluno> _alunos = new()
        {
          new Aluno { Id = 1, Nome = "João" },
          new Aluno { Id = 2, Nome = "Maria" }
        };

        private static List<Disciplina> _disciplinas = new()
        {
          new Disciplina { Id = 1, Nome = "Matemática" },
          new Disciplina { Id = 2, Nome = "História" }
        };

        private static List<Matricula> _matriculas = new();




        public MatriculasController(ILogger<MatriculasController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult Post([FromBody] MatriculaCriarDTO matriculaDTO)
        {
            var aluno = _alunos.FirstOrDefault(a => a.Id == matriculaDTO.AlunoId);
            if (aluno == null) return NotFound("Aluno não encontrado.");

            var disciplina = _disciplinas.FirstOrDefault(a => a.Id == matriculaDTO.DisciplinaId);
            if (disciplina == null) return NotFound("Disciplina não encontrada.");

            // Verificar se já está matriculado
            if (_matriculas.Any(m => m.Aluno.Id == matriculaDTO.AlunoId && m.Disciplina.Id == matriculaDTO.DisciplinaId))
                return BadRequest("Aluno já está matriculado nesta disciplina.");

            //Matricula matricula = new();
            //matricula.Id = _matriculas.Count + 1;
            //matricula.Aluno = new Aluno();
            //matricula.Aluno.Id = matriculaDTO.AlunoId;
            //matricula.Disciplina.Id = matriculaDTO.DisciplinaId;

            Matricula matricula = new()
            {
                Id = _matriculas.Count + 1,
                Aluno = aluno,
                Disciplina = new Disciplina()
                {
                    Id = matriculaDTO.DisciplinaId
                }
            };


            _matriculas.Add(matricula);
            return Created("/" + matricula.Id, matricula);
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult<IEnumerable<MatriculaDTO>> ObterTodas()
        {
            List<MatriculaDTO> matriculaDTOs = new();

            foreach (var item in _matriculas)
            {
                matriculaDTOs.Add(new MatriculaDTO()
                {
                    Id = item.Id,
                    DisciplinaId = item.Id,
                    AlunoId = item.Aluno.Id,
                });
            }

            return Ok(matriculaDTOs);
        }


        [HttpGet("aluno/{id}")]
        public ActionResult<IEnumerable<Matricula>> ObterMatriculasAluno(int id)
        {
            var aluno = _alunos.FirstOrDefault(a => a.Id == id);
            if (aluno == null)
                return NotFound("Aluno não encontrado");

            var matriculas = _matriculas.Where(m => m.Aluno.Id == id).ToList();

            return Ok(matriculas);
        }


        [HttpGet("disciplina/{id}")]
        public ActionResult<IEnumerable<Aluno>> ObterAlunosMatriculados(int id)
        {

            var matriculas = _matriculas.Where(m => m.Disciplina.Id == id).ToList();

            //List<Aluno> alunos = new List<Aluno>();

            //foreach (var matricula in matriculas)
            //{
            //    alunos.Add(matricula.Aluno);
            //}

            //return Ok(alunos);

            return Ok(matriculas.Select(m => m.Aluno));
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var matricula = _matriculas.FirstOrDefault(m => m.Id == id);

            if (matricula == null)
                return NotFound("Matrícula não encontrada");

            _matriculas.RemoveAll(m => m.Id == id);
            return Ok();
        }


    }
}
