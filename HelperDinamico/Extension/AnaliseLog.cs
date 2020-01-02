using HelperDinamico.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HelperDinamico.Extension
{
    public class AnaliseLog
    {
        static String raiz = "C:\\inetpub\\wwwroot\\hd\\Extension\\LogsTemp";
        static String destino = "";
        static String nomeLog = null;
        static String Locomotiva = "locomotiva";
        static String EOT = "eot";
        static int minBAT_EOT = 50;

        Dictionary<string, string> Alarme1 = new Dictionary<string, string>();
        Dictionary<string, string> Alarme2 = new Dictionary<string, string>();
        Dictionary<string, string> Alarme3 = new Dictionary<string, string>();

        public List<InfoLog> Iniciar(String origem, String nomeArquivo)
        {

            List<EventoLog> eventoLogs = null;
            List<InfoLog> infoLogs = null;
            nomeLog = nomeArquivo;

            Criarpasta();
            DebugLog.Logar("Arquivo Zipado de  " + origem + " para " + destino);

            try
            {
                if (ExtrairArquivoZip(origem + "\\" + nomeArquivo, destino))
                {

                    eventoLogs = LerArquivos();
                    carregarAlarmes();
                    infoLogs = AnalisarEventos(eventoLogs);
                    infoLogs = infoLogs.OrderBy(x => x.Data).
                               GroupBy(x => x.Descricao).
                               Select(x => x.First()).ToList();
                }
            }
            catch (Exception e)
            {
                //JOptionPane.showMessageDialog(null, "Arquivo TXT corrompido !!!");
                DebugLog.Logar("  DeletarArquivos() " + e.Message);
                DebugLog.Logar(e.StackTrace);
            }
            finally
            {
                DeletarArquivos();
            }
            return infoLogs;
        }

        Boolean ExtrairArquivoZip(string localizacaoArquivoZip, string destino)
        {
            if (File.Exists(localizacaoArquivoZip))
            {
                //recebe a localização do arquivo zip
                using (ZipFile zip = new ZipFile(localizacaoArquivoZip))
                {
                    //verifica se o destino existe
                    if (Directory.Exists(destino))
                    {
                        try
                        {
                            //extrai o arquivo zip para o destino
                            zip.ExtractAll(destino);
                        }
                        catch
                        {
                            DebugLog.Logar(" zip.ExtractAll(destino); - erro ao extrai o arquivo zip para o destino");
                            return false;
                        }
                    }
                    else
                    {
                        //lança uma exceção se o destino não existe
                        DebugLog.Logar("O arquivo destino não foi localizado");
                        return false;
                    }
                }
            }
            else
            {
                //lança uma exceção se a origem não existe
                DebugLog.Logar("O Arquivo Zip não foi localizado");
                return false;
            }

            return true;
        }

        List<EventoLog> LerArquivos()
        {
            List<EventoLog> eventoLogs = new List<EventoLog>();
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(destino));
            List<FileInfo> fileInfo = dir.GetFiles().ToList();

            fileInfo.ForEach(x =>
            {
                if (x.Extension.ToUpper().Contains("TXT"))
                {
                    using (StreamReader sr = x.OpenText())
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            if (s != null)
                            {
                                if (s.Count() == 157 && s.Substring(0, 1).Contains("#"))
                                {
                                    EventoLog eventoLog = new EventoLog(s);
                                    eventoLogs.Add(eventoLog);
                                }
                            }

                        }
                    }
                }
            });

            return eventoLogs;
        }

        void Criarpasta()
        {
            String fileName = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Millisecond + "_" + DateTime.Now.Millisecond;
            destino = raiz + "\\" + fileName;
            bool exists = System.IO.Directory.Exists(destino);
            if (!exists)
                System.IO.Directory.CreateDirectory(destino);
        }

        void DeletarArquivos()
        {
            DebugLog.Logar("Deletando Arquivos");
            try
            {
                //  DirectoryInfo dir = new DirectoryInfo(Path.Combine(raiz));
                //   List<FileInfo> fileInfo = dir.GetFiles().ToList();
                //   fileInfo.ForEach(x =>
                //   {
                //        x.Delete();
                //   });

                DirectoryInfo directoryInfo = new DirectoryInfo(destino);
                directoryInfo.Delete(true);
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.StackTrace);
            }
        }

        private List<InfoLog> AnalisarEventos(List<EventoLog> eventoLogs)
        {
            List<InfoLog> infoLogs = new List<InfoLog>();
            InfoLog infoLog = null;

            if (eventoLogs != null)
            {
                EventoLog eventoAnterior = null;
                eventoLogs.ForEach(x =>
                {
                    infoLog = getAlarmes(x);
                    if (infoLog != null)
                        infoLogs.Add(infoLog);

                    infoLog = getEOT(x, eventoAnterior);
                    if (infoLog != null)
                        infoLogs.Add(infoLog);

                    infoLog = getCorteTracaoEmergencia(x, eventoAnterior);
                    if (infoLog != null)
                        infoLogs.Add(infoLog);

                    eventoAnterior = x;
                }
            );
            }

            return infoLogs;
        }

        void carregarAlarmes()
        {
            Alarme1.Add("000", "");
            Alarme1.Add("002", "DATALOGGER LIG");
            Alarme1.Add("004", "DATALOGGER LIVRE");
            Alarme1.Add("008", "GPS LOC. OK");
            Alarme1.Add("016", "AR CILINTRO PRINC");
            Alarme1.Add("032", "GPS ATIVO");
            Alarme1.Add("064", "APLICACAO FREIO GRAD.");
            Alarme1.Add("128", "-----");

            Alarme2.Add("000", "");
            Alarme2.Add("001", "EOT BAT.");
            Alarme2.Add("002", "TELEMETRO ATIVO");
            Alarme2.Add("004", "CIL.ALIN.E.AV.");
            Alarme2.Add("008", "CIL.ALIN.E.REC.");
            Alarme2.Add("016", "CIL.ALIN.D.AV");
            Alarme2.Add("032", "CIL.ALIN.D.REC");
            Alarme2.Add("056", "ERRO NA INDICAÇÃO NOS CILINDROS DE ALINHAMENTO");
            Alarme2.Add("064", "PINO ENGATE");
            Alarme2.Add("128", "FREI HELPER (CF)");

            Alarme3.Add("000", "");
            Alarme3.Add("001", "DISTANCIA RELATIVA");
            Alarme3.Add("002", "VELOCIDADE RELATIVA");
            Alarme3.Add("004", "DEFEITO LASER E");
            Alarme3.Add("008", "DEFEITO LASER D");
            Alarme3.Add("016", "VELOCIDADE HELPER");
            Alarme3.Add("032", "FREIO TREM (EG)");
            Alarme3.Add("064", "EOT ATIVO");
            Alarme3.Add("128", "EOT ID");
        }

        private InfoLog getAlarmes(EventoLog x)
        {
            InfoLog infoLog = new InfoLog();
            infoLog.Ativo = Locomotiva;
            infoLog.Nome = "Alarmes";
            infoLog.Descricao = "";
            infoLog.Data = x.time;
            infoLog.Log = nomeLog;

            String Alarme = null;
            string lvAlarme = "";

            Alarme = x.ALARME1;
            if (!Alarme.Equals("000"))
            {
                if (Alarme1.TryGetValue(Alarme, out lvAlarme))
                {
                    bool flag = infoLog.Descricao != null && !infoLog.Descricao.Contains("");
                    infoLog.Descricao = (flag) ? infoLog.Descricao + "|" + lvAlarme : lvAlarme;
                }
                else
                {
                    infoLog.Descricao = Alarme + " Não Homologado";
                }
            }


            Alarme = x.ALARME2;
            if (!Alarme.Equals("000"))
            {
                if (Alarme2.TryGetValue(Alarme, out lvAlarme))
                {
                    bool flag = infoLog.Descricao != null && !infoLog.Descricao.Contains("");
                    infoLog.Descricao = (flag) ? infoLog.Descricao + "|" + lvAlarme : lvAlarme;
                }
                else
                {
                    infoLog.Descricao = Alarme + " Não Homologado";
                }
            }



            Alarme = x.ALARME3;
            if (!Alarme.Equals("000"))
            {
                if (Alarme3.TryGetValue(Alarme, out lvAlarme))
                {
                    bool flag = infoLog.Descricao != null && !infoLog.Descricao.Contains("");
                    infoLog.Descricao = (flag) ? infoLog.Descricao + "|" + lvAlarme : lvAlarme;
                }
                else
                {
                    infoLog.Descricao = Alarme + " Não Homologado";
                }
            }

            if (infoLog.Descricao.Equals(""))
            {
                return null;
            }
            else
            {
                return infoLog;
            }
        }

        private InfoLog getEOT(EventoLog x, EventoLog eventoAnterior)
        {
            InfoLog infoLog = new InfoLog();
            infoLog.Ativo = EOT;
            infoLog.Nome = "EOT";
            infoLog.Descricao = "";
            infoLog.Data = x.time;
            infoLog.Log = nomeLog;

            if (x.BAT_EOT < minBAT_EOT && x.ID_EOT > 0)
            {

                infoLog.Descricao = "EOT com Id: " + x.ID_EOT + " Apresentou " + x.BAT_EOT + " %  de Bateria";
            }

            if (eventoAnterior != null)
            {
                if (x.ESTADO_GPS_EOT == 0 && x.ID_EOT > 0)
                {

                    infoLog.Descricao = infoLog.Descricao + "| EOT com Id: " + x.ID_EOT + " Perdeu comunicação com GPS ";
                    infoLog.Descricao = infoLog.Descricao + "|[ posição Locomotiva Helper : KM" + posicaoHelper_KM(x) + ", posição do EOT : KM" + posicaoTrem_KM(eventoAnterior) + " ]";
                }

                if (eventoAnterior.ID_EOT == 0 && x.ID_EOT > 0)
                {
                    infoLog.Descricao = infoLog.Descricao + "| EOT com Id: " + x.ID_EOT + " Iniciou  comunicação com o Helper ~" + x.DIST_RELATIVA_GPS_EOT_HELPER + " metros ";
                    infoLog.Descricao = infoLog.Descricao + "|[ posição Locomotiva Helper : KM" + posicaoHelper_KM(x) + ", posição do EOT : KM" + posicaoTrem_KM(x) + " ]";
                }

                if (eventoAnterior.ID_EOT > 0 && x.ID_EOT == 0)
                {
                    infoLog.Descricao = infoLog.Descricao + "| EOT com Id: " + eventoAnterior.ID_EOT + " Perdeu comunicação com o Helper ~" + eventoAnterior.DIST_RELATIVA_GPS_EOT_HELPER + " metros ";
                    infoLog.Descricao = infoLog.Descricao + "|[ posição Locomotiva Helper : KM" + posicaoHelper_KM(x) + ", posição do EOT : KM" + posicaoTrem_KM(eventoAnterior) + " ]";
                }
            }

            if (infoLog.Descricao.Equals(""))
            {
                return null;
            }
            else
            {

                return infoLog;
            }
        }

        private InfoLog getCorteTracaoEmergencia(EventoLog x, EventoLog eventoAnterior)
        {
            InfoLog infoLog = new InfoLog();
            infoLog.Ativo = Locomotiva;
            infoLog.Nome = "CorteTracaoEmergencia";
            infoLog.Descricao = "";
            infoLog.Data = x.time;
            infoLog.Log = nomeLog;


            if (eventoAnterior != null)
            {


                if (x.CORTE_TRACAO == 1 && eventoAnterior.CORTE_TRACAO == 0)
                {
                    infoLog.Descricao = "|Locomotiva Helper apresentou *CORTE DE TRAÇÃO  ";
                }

                if (x.EMERGENCIA == 1 && eventoAnterior.EMERGENCIA == 0)
                {
                    infoLog.Descricao = infoLog.Descricao + "|Locomotiva Helper apresentou *EMERGÊNCIA";
                }

                if ((x.EMERGENCIA == 1 && eventoAnterior.EMERGENCIA == 0) || (x.CORTE_TRACAO == 1 && eventoAnterior.CORTE_TRACAO == 0))
                {

                    infoLog.Descricao = infoLog.Descricao + "|[ posição Locomotiva Helper : KM" + posicaoHelper_KM(x) + ", posição do EOT : KM" + posicaoTrem_KM(eventoAnterior) + " ]";

                    InfoLog i = getAlarmes(x);
                    if (i != null)
                    {
                        if (!i.Descricao.Equals(""))
                            infoLog.Descricao = infoLog.Descricao + "|Motivo Aparente => " + i.Descricao;
                    }
                }

                if (x.CILINDRO_PINO_ENGATE == 1 && eventoAnterior.CILINDRO_PINO_ENGATE == 0)
                {
                    infoLog.Descricao = infoLog.Descricao + "|Pino de Engate abaixado";
                }


                if (x.CILINDRO_PINO_ENGATE == 0 && eventoAnterior.CILINDRO_PINO_ENGATE == 1)
                {
                    infoLog.Descricao = infoLog.Descricao + "|Pino de Engate Levantado";
                }


            }

            if (infoLog.Descricao.Equals(""))
            {
                return null;
            }
            else
            {
                infoLog.Descricao += (x.CHAVE_INTERLOCK == 1) ? "|Modo de Operação" : "|Modo de Manutenção";
                return infoLog;
            }
        }

        public GridView GetXls(String origem, String nomeArquivo)
        {
            nomeLog = nomeArquivo;
            List<EventoLog> eventoLogs = null;
            DebugLog.Logar("Arquivo Zipado " + origem);
            Criarpasta();

            if (ExtrairArquivoZip(origem + "\\" + nomeArquivo, destino))
            {
                try
                {
                    eventoLogs = LerArquivos();
                    DeletarArquivos();
                }
                catch (Exception e)
                {
                    DebugLog.Logar("  DeletarArquivos() " + e.Message);
                    DebugLog.Logar(e.StackTrace);
                }
            }

            var gridView = new GridView();
            DataTable dt = Utility.ExportListToDataTable(eventoLogs);

            if (dt != null)
            {
                gridView.DataSource = dt;
                gridView.DataBind();
            }

            return gridView;
        }

        public int posicaoTrem_KM(EventoLog ev)
        {
            if (ev.POSICAOHELPER_GPS < 0)
                return (617000 + Math.Abs(ev.POSICAO_EOT_GPS));
            else
                return (617000 - Math.Abs(ev.POSICAO_EOT_GPS));
        }

        public int posicaoHelper_KM(EventoLog ev)
        {
            return (617000 - ev.POSICAOHELPER_GPS);
        }

    }
}


