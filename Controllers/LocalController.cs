using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API_Projeto_Casa.Data;
using API_Projeto_Casa.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_Projeto_Casa.Controllers {
    [Route ("api/v1/local")]
    [ApiController]

    public class ApiLocalController : ControllerBase {
        private readonly ApplicationDbContext database;

        public ApiLocalController (ApplicationDbContext database) {

            this.database = database;
        }

        [HttpGet]
        public IActionResult Get () {
            var local = database.Local.Where (p => p.Status == true).ToList ();
            return Ok (local);
            //Status code = 200 && Dados 
        }

        [HttpGet ("{id}")]
        public IActionResult Get (int id) {
            try {
                var local = database.Local.Where (l => l.Status == true).First (l => l.Id == id);
                return Ok (local);
            } catch (Exception) {

                Response.StatusCode = 404;
                return new ObjectResult (new { msg = "Id inválido" });
            }

        }

        [HttpPost]
        public IActionResult Post ([FromBody] LocalTemp lTemp) {
            Local l = new Local ();
            l.Nome = lTemp.Nome;
            l.Endereco = lTemp.Endereco;
            l.LinkEndereco = lTemp.LinkEndereco;
            l.Status = true;

            database.Local.Add (l);
            database.SaveChanges ();

            //Set do Status Code
            Response.StatusCode = 201;
            return new ObjectResult (new { msg = "Local criado com sucesso!" });
        }

        [HttpPatch]
        public IActionResult Patch ([FromBody] Local local) {
            if (local.Id > 0) {

                try {
                    var ltemp = database.Local.First (p => p.Id == local.Id);
                    if (ltemp != null) {

                        //Editar 
                        ltemp.Nome = local.Nome != null ? local.Nome : local.Nome;
                        ltemp.Endereco = local.Endereco != null ? local.Endereco : local.Endereco;
                        ltemp.LinkEndereco = ltemp.LinkEndereco != null ? ltemp.LinkEndereco : ltemp.LinkEndereco;
                        database.SaveChanges ();
                        return Ok ();
                    }
                } catch (System.Exception) {
                    Response.StatusCode = 404;
                    return new ObjectResult (new { msg = "Evento não encontrado" });
                }

            } else {
                Response.StatusCode = 404;
                return new ObjectResult (new { msg = "Id inválido" });
            }
            Response.StatusCode = 404;
            return new ObjectResult (new { msg = "Id inválido" });
        }

        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            try {
                var local = database.Local.First (p => p.Id == id);
                local.Status = false;
                database.SaveChanges ();
                Response.StatusCode = 201;
                return new ObjectResult (new { msg = "Local excluido!" });
            } catch (Exception) {

                Response.StatusCode = 404;
                return new ObjectResult (new { msg = "Id inválido" });
            }
        }
        public class LocalTemp {

            [Required]
            public int Id { get; set; }

            [Required (ErrorMessage = "O Nome é obrigatório.")]
            public string Nome { get; set; }

            [Required (ErrorMessage = "O Endereço é obrigatório.")]
            public string Endereco { get; set; }

            public string LinkEndereco { get; set; }

            public bool Status { get; set; }
        }
    }
}