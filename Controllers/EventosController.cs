using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API_Projeto_Casa.Data;
using API_Projeto_Casa.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Projeto_Casa.Controllers {
    [Route ("api/v1/eventos")]
    [ApiController]
    public class EventosController : ControllerBase {

        private readonly ApplicationDbContext database;


        public EventosController (ApplicationDbContext database) {

            this.database = database;
        }

        [HttpGet]
        public IActionResult Get () {
            var eventos = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).ToList ();
            return Ok (eventos); //Status code = 200 && Dados 
        }

        [HttpGet ("{id}")]
        public IActionResult Get (int id) {
            try {
                Evento evento = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (e => e.Status == true).First (p => p.Id == id);
                return Ok (evento);
            } catch (Exception) {

                Response.StatusCode = 404;
                return new ObjectResult (new { msg = "Id inválido" });
            }

        }

        [HttpPost]
        public IActionResult Post ([FromBody] EventoTemp eTemp) {
            //Validação

            Evento e = new Evento ();
            e.Nome = eTemp.Nome;
            e.Capacidade = eTemp.Capacidade;
            e.Data = eTemp.Data;
            e.Preco = eTemp.Preco;
            if (database.Local.First (c => c.Id == eTemp.CasaDeShowID) == null || database.Local.Where (c => c.Status == true).First (c => c.Id == eTemp.CasaDeShowID) == null) {
                return new ObjectResult (new { msg = "O local do Evento não existe!" });
            } else {
                e.CasaDeShow = database.Local.First (c => c.Id == eTemp.CasaDeShowID);
            }
            e.Estilo = eTemp.Estilo.ToString ();
            e.Imagem = eTemp.Imagem;
            e.Status = eTemp.Status;
            database.Evento.Add (e);
            database.SaveChanges ();

            //Set do Status Code
            Response.StatusCode = 201;
            return new ObjectResult (new { msg = "Evento criado com sucesso!" });
        }
        [HttpPatch]
        public IActionResult Patch ([FromBody] Evento evento) {
            if (evento.Id > 0) {

                try {
                    var ptemp = database.Evento.First (p => p.Id == evento.Id);
                    if (ptemp != null) {

                        //Editar 
                        // Obs: Não é possivel alterar Local do evento
                        ptemp.Nome = evento.Nome != null ? evento.Nome : ptemp.Nome;
                        ptemp.Capacidade = evento.Capacidade != 0 ? evento.Capacidade : ptemp.Capacidade;
                        ptemp.Data = evento.Data != null ? evento.Data : ptemp.Data;
                        ptemp.Preco = evento.Preco != 0 ? evento.Preco : ptemp.Preco;
                        ptemp.Estilo = evento.Estilo != null ? evento.Estilo : ptemp.Estilo;
                        ptemp.Imagem = evento.Imagem != null ? evento.Imagem : ptemp.Imagem;
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
                var evento = database.Evento.First (p => p.Id == id);
                evento.Status = false;
                database.SaveChanges ();
                Response.StatusCode = 201;
                return new ObjectResult (new { msg = "Evento excluido!" });
            } catch (Exception) {
                Response.StatusCode = 404;
                return new ObjectResult (new { msg = "Id inválido" });
            }
        }


        public class EventoTemp {
            public int Id { get; set; }

            [Required (ErrorMessage = "O Nome é obrigatório.")]
            [StringLength (150, ErrorMessage = "Nome grande demais.")]
            [MinLength (2, ErrorMessage = "Nome curto demais.")]
            public string Nome { get; set; }

            [Range (10, 100000, ErrorMessage = "Capacidade Inválida.")]
            public int Capacidade { get; set; }

            [Required (ErrorMessage = "A data é obrigatória.")]
            public System.DateTime Data { get; set; }

            [Range (1, 100000, ErrorMessage = "Preço inválido.")]
            public double Preco { get; set; }

            [Required]
            public int CasaDeShowID { get; set; }

            [Range (1, 8, ErrorMessage = "Estilo inválido.")]
            public int Estilo { get; set; }

            [Required (ErrorMessage = "A URL da imagem é obrigatória.")]
            [MinLength (2, ErrorMessage = "Url curta demais.")]
            [StringLength (1024, ErrorMessage = "Url grande demais.")]
            public string Imagem { get; set; }

            public bool Status { get; set; }
        }
    }
}