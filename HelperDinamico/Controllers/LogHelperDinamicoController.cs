using HelperDinamico.Dal;
using HelperDinamico.DAL;
using HelperDinamico.Extension;
using HelperDinamico.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace HelperDinamico.Controllers
{
    public class LogHelperDinamicoController : Controller
    {
        private RacContext racdb = new RacContext();
        private IpHelperDinamicoContext db = new IpHelperDinamicoContext();

        public ActionResult Index()
        {
            List<LocomotivaViewModel> locomotivasviewmodel = new List<LocomotivaViewModel>();

            try
            {
                List<Locomotiva> locomotivas = db.Locomotiva.ToList();

                foreach (var loco in locomotivas)
                {
                    var locoviewmodel = new LocomotivaViewModel();
                    locoviewmodel.Nome = loco.Nome;
                    DebugLog.Logar(loco.Nome);
                    locoviewmodel.Arquivos = new DirectoryInfo(Path.Combine(Server.MapPath("~/Arquivos"))).
                        GetFiles().Where(x => x.Name.Contains(loco.Id.ToString())).ToList();

                    locomotivasviewmodel.Add(locoviewmodel);
                }

                
                    ViewBag.Message = TempData["Message"].ToString();
            }
            catch(Exception e)
            {
                DebugLog.Logar(e.StackTrace);
            }

            return View(locomotivasviewmodel);
        }

        public ActionResult Analise()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SalvarLog()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString.ToString()))
                {
                    String Locomotiva = Request.QueryString["loco"].ToString();
                    String arq = ".7z";

                    FileStream output = null;
                    StreamReader inputStream;
                    Stream rawStream;

                    try
                    {
                        if (Request.Files.Count > 0)
                        {
                            string raiz = (Path.Combine(Server.MapPath("~/Arquivos")));
                            string path = raiz + "/" + Locomotiva + DateTime.Now.ToString(" dd-MM-yyyy HH-mm-ss") + arq;
                            Directory.CreateDirectory(raiz);
                            output = new FileStream(path, FileMode.Create);

                            Request.InputStream.Position = 0;
                            inputStream = new StreamReader(Request.Files[0].InputStream);
                            rawStream = inputStream.BaseStream;
                            rawStream.Seek(0, SeekOrigin.Begin);
                            rawStream.CopyTo(output);

                            rawStream.Close();
                            inputStream.Close();
                            output.Close();

                            DateTime today = DateTime.Now;
                            DateTime answer = today.AddDays(-7);

                            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("~/Arquivos")));
                            List<FileInfo> fileInfo = dir.GetFiles().ToList();

                            fileInfo.ForEach(x =>
                            {
                                if (x.CreationTimeUtc < answer)
                                {
                                    x.Delete();
                                }
                            });

                            IpHelperDinamico ipHelperDinamico = new IpHelperDinamico();
                            ipHelperDinamico.Data = DateTime.Now;
                            ipHelperDinamico.LocomotivaId = Int32.Parse(Locomotiva);
                            ipHelperDinamico.Ip = Request.UserHostAddress.ToString();
                            db.IpHelperDinamico.Add(ipHelperDinamico);
                            db.SaveChanges();
                            SendMessages(Locomotiva, DateTime.Now);

                            return Json("Sucessagem!");
                        }
                        else
                        {
                            return Json("Sem Arquivo valido");
                        }
                    }
                    catch (Exception e)
                    {
                        DebugLog.Logar(e.Message);
                        DebugLog.Logar(e.StackTrace);
                        return Json(e.Message + " : " + e.StackTrace);
                    }
                }
                else
                {
                    DebugLog.Logar("LOGHELPER => NULL");
                    return Json("LOGHELPER => NULL");
                }
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.Message);
                DebugLog.Logar(e.StackTrace);
                return Json(e.Message + " : " + e.StackTrace);
            }
        }

        private void SendMessages(string loco, DateTime date)
        {
            try
            {
                var dbSms = new UserContext();
                var receivers = new string[] { "valeriobsno@gmail.com",
                                               "valerio.nunes@vale.com",
                                               "lusami.barbosa@vale.com" };

                //                               "helcio.mangueira@vale.com",
                //                               "cassio.miranda@vale.com",
                //                               "deyvison.araujo@vale.com",
                //                               "jose.maria.cordeiro@vale.com",
                //                               "josemar.monteiro@vale.com",
                //                               "jocimar.oliveira@vale.com",
                //                               "saulo.santos@vale.com"
                //};

                foreach (var user in receivers)
                {
                    SmsQueue smsQueue = new SmsQueue();
                    smsQueue.Hist = DateTime.Now;
                    smsQueue.Message = string.Format("Log da Locomotiva Helper {0} recebido no sistema! http://172.20.15.22/hd/LogHelperDinamico ", loco);
                    smsQueue.Sender = "pcm@vale.com";
                    smsQueue.Receiver = user;
                    smsQueue.Subject = string.Format("Download Remoto do Helper Dinâmico - Log da {0} recebido com sucesso!", loco);
                    smsQueue.AppInfo = "5500";
                    dbSms.SmsQueue.Add(smsQueue);
                }
                dbSms.SaveChanges();
                dbSms.Dispose();
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.Message);
                DebugLog.Logar(e.StackTrace);
            }
        }

        public ActionResult Analisar(String fileDir, String filename)
        {
            try
            {
                AnaliseLog analiseLog = new AnaliseLog();
                List<Evento> Eventos = racdb.Eventos.Where(x => x.SistemaId == 6).OrderBy(data => data.InicioAvaria).ToList();
                var infoLogs = analiseLog.Iniciar(fileDir, filename);

                if (infoLogs.FirstOrDefault() != null)
                {
                    ViewBag.Infolog = infoLogs;

                    var nomeLog = infoLogs.FirstOrDefault().Log;
                    var loco_data_hora = nomeLog.Split(' ');
                    var loco = loco_data_hora[0];

                    var dateEventos = infoLogs.OrderBy(x => x.Data).
                    GroupBy(x => x.Data.Day).
                    Select(x => x.First()).ToList();

                    dateEventos.ForEach(x =>
                    {
                        DebugLog.Logar(x.Data.ToString());
                    });

                    Eventos = Eventos.Where(x => x.CampoOrdenacao != null &&
                                                 x.CampoOrdenacao.Contains(loco) &&
                                                 x.InicioAvaria.Date >= dateEventos.FirstOrDefault().Data.Date &&
                                                 x.InicioAvaria.Date <= dateEventos.LastOrDefault().Data.Date).ToList();
                    ViewBag.Eventos = Eventos;

                    if (Eventos != null)
                    {
                        Eventos.ForEach(x =>
                        {
                            DebugLog.Logar(x.Descricao + "  " + x.InicioAvaria);
                        });
                    }

                    if (infoLogs.Count > 0){
                        TempData["Message"] = null;
                        return View("Analise");
                        
                    }
                    TempData["Message"] = "Atenção - Arquivo de Log está Vazio !!!";
                }
                else
                {
                    TempData["Message"] = "Atenção - Arquivo de Log está Vazio !!!";
                }
            }
            catch (Exception e)
            {
                TempData["Message"] = "Atenção - Erro na Leiruta do Log !!!";
                DebugLog.Logar(e.Message);
                DebugLog.Logar(e.StackTrace);
            }

            return RedirectToAction("Index");
        }

        public FileResult Baixar(String fileInfo)
        {
            if (fileInfo != null)
            {
                return File(Server.MapPath("~/Arquivos") + "/" + fileInfo, "application/7z", fileInfo);
            }
            return null;
        }

        public FileResult BaixarXls(String fileDir, String fileName)
        {
            if (!string.IsNullOrEmpty(fileDir) && !string.IsNullOrEmpty(fileName))
            {
                AnaliseLog analiseLog = new AnaliseLog();
                var gridView = analiseLog.GetXls(fileDir, fileName);
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName.Split('.')[0] + ".xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                gridView.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            return null;
        }

        public FileResult AnalisadorHD()
        {
            string arquivo = Server.MapPath("~/Extension") + "/Helper_Dinâmico.jar";
            if (System.IO.File.Exists(arquivo))
            {
                return File(arquivo, "application/jar", "Helper_Dinâmico - ValerianoSystem.jar");
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                racdb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}