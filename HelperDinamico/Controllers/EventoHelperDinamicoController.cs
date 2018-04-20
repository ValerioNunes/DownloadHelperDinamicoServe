using HelperDinamico.DAL;
using HelperDinamico.Extension;
using HelperDinamico.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelperDinamico.Controllers
{
    public class EventoHelperDinamicoController : Controller
    {
        private RacContext racdb = new RacContext();
        private IpHelperDinamicoContext locodb = new IpHelperDinamicoContext();

        // GET: EventoHelperDinamico
        public ActionResult Index()
        {
            List<Evento> Eventos = racdb.Eventos.Where(x => x.SistemaId == 6).OrderByDescending(data => data.InicioAvaria).ToList();
            List<Locomotiva> locomotivas = locodb.Locomotiva.ToList();

            List<EventoViewModel> eventoviewmodels = new List<EventoViewModel>();

            foreach (var loco in locomotivas)
            {
                var eventoviewmodel = new EventoViewModel();
                eventoviewmodel.Nome = loco.Nome;
                var EventosDessaLoco = Eventos.Where(x => x.CampoOrdenacao != null &&
                x.CampoOrdenacao.Contains(loco.Id.ToString())).ToList();
                eventoviewmodel.Eventos = EventosDessaLoco;

                eventoviewmodels.Add(eventoviewmodel);
            }

            return View(eventoviewmodels);
        }

        [HttpPost]
        public ActionResult GetEventosHelper()
        {
            DateTime now = DateTime.Now;
            int month = now.Month;
            int[] falhas = new int[month];
            int[] defeitos = new int[month];

            for (int i = 0; i < month; i++)
            {
                int mes = i + 1;
                var eventosDoMes = racdb.Eventos.Where(x => x.SistemaId == 6 &&
                x.InicioAvaria.Month == mes && x.InicioAvaria.Year == now.Year).ToList();

                falhas[i] = eventosDoMes.Where(x => x.Sala.Contains("FAL")).Count();
                defeitos[i] = eventosDoMes.Where(x => x.Sala.Contains("DEF")).Count();
            }

            return Json(new object[] { falhas, defeitos });
        }

        [HttpPost]
        public ActionResult GetEventosLocomotiva()
        {
            DateTime now = DateTime.Now;
            int month = now.Month;
            
            List<LocomotivaGrafEvento> locomotivaGrafEventos = new List<LocomotivaGrafEvento>();

            foreach (var loco in locodb.Locomotiva.ToList())
            {
                LocomotivaGrafEvento locomotivaGrafEvento = new LocomotivaGrafEvento();
                locomotivaGrafEvento.Nome = loco.Nome;
                int[] eventos = new int[month];

                for (int i = 0; i < month; i++)
                    {
                        int mes = i + 1;
                        
                        var eventosDoMes = racdb.Eventos.Where(x => x.SistemaId == 6 &&
                        x.InicioAvaria.Month == mes && x.InicioAvaria.Year == now.Year && x.CampoOrdenacao.Contains(loco.Nome)).ToList();

                        eventos[i] = eventosDoMes.Count();
                    }
                locomotivaGrafEvento.eventos = eventos;

                locomotivaGrafEventos.Add(locomotivaGrafEvento);
            }
            
            return Json(locomotivaGrafEventos);
        }

        // GET: EventoHelperDinamico/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EventoHelperDinamico/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventoHelperDinamico/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EventoHelperDinamico/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EventoHelperDinamico/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EventoHelperDinamico/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EventoHelperDinamico/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                locodb.Dispose();
                racdb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
