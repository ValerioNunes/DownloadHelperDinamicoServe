using HelperDinamico.DAL;
using HelperDinamico.Extension;
using HelperDinamico.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelperDinamico.Controllers
{
    public class LogHelperDinamicoController : Controller
    {
        private IpHelperDinamicoContext db = new IpHelperDinamicoContext();

        public ActionResult Index()
        {
            List<Locomotiva> locomotivas = db.Locomotiva.ToList();
            List<LocomotivaViewModel> locomotivasviewmodel = new List<LocomotivaViewModel>();

            foreach (var loco in locomotivas)
            {
                var locoviewmodel = new LocomotivaViewModel();
                locoviewmodel.Nome = loco.Nome;
                locoviewmodel.Arquivos = new DirectoryInfo(Path.Combine(Server.MapPath("~/Arquivos"))).
                    GetFiles().Where(x => x.Name.Contains(loco.Id.ToString())).ToList();               

                locomotivasviewmodel.Add(locoviewmodel);
            }

            return View(locomotivasviewmodel);
        }

        [HttpPost]
        public ActionResult SalvarLog()
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

        public FileResult Baixar(String fileInfo)
        {
            if (fileInfo != null)
            {
                return File(Server.MapPath("~/Arquivos") + "/" + fileInfo, "application/7z", fileInfo);
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
            }
            base.Dispose(disposing);
        }
    }
}