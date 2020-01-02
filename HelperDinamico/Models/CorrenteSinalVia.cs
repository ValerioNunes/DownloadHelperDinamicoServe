using HelperDinamico.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelperDinamico.Models
{
    public class CorrenteSinalVia
    {
        public String log { get; set; }
        public String trig { get; set; }
        public String freq { get; set; }
        public int KM { get; set; }
        public double corrente { get; set; }
        public double Period { get; set; }
        public double valorFrequencia { get; set; }
        public int linha { get; set; }
        public String Data { get; set; }
        public DateTime data { get; set; }

        public CorrenteSinalVia(String log, int linha, String part1, String part2)
        {
            this.log = log;
            this.linha = linha;
            Trig(part1);
            Km(part1);
            DataTempo(part1);


            Freq(part2);
            Power(part2);
            Periodo(part2);

        }

        public void Periodo(String texto)
        {
            try
            {
                texto = texto.Split(',')[2].Replace(" ", "").Replace(" ", "").Replace("ms", "").Replace(">", "");
                texto = texto.Split(':')[1];
                this.Period = Double.Parse(texto) / 1000;
                this.valorFrequencia = 1 / this.Period;
            }
            catch (Exception nfe)
            {
                this.Period = -1.0;
            }
        }
        void Power(String texto)
        {
            try
            {
                texto = texto.Split(',')[1].Replace(" ", "").Replace(" ", "").Replace("%", "").Replace("<", "");
                texto = texto.Split(':')[1];
                this.corrente = Double.Parse(texto) / 100;

            }
            catch (Exception nfe)
            {
                this.corrente = -1.0;
                DebugLog.Logar(nfe.Message);
                DebugLog.Logar(nfe.StackTrace);
            }
        }
        private void DataTempo(String texto)
        {

            try
            {
                this.Data = texto.Split(' ')[0];
            }
            catch (Exception nfe)
            {
                DebugLog.Logar(nfe.Message);
                DebugLog.Logar(nfe.StackTrace);
            }
        }

        void Freq(String texto)
        {
            try
            {
                this.freq = texto.Split(',')[0].Replace(" ", "").Split(':')[1];

            }
            catch (Exception nfe)
            {
                DebugLog.Logar(nfe.Message);
            }
        }

        void Trig(String texto)
        {
            try
            {
                this.trig = texto.Split(':')[1].Replace(")", "").Replace(" ", "");
            }
            catch (Exception nfe)
            {
                DebugLog.Logar(nfe.Message);
                DebugLog.Logar(nfe.StackTrace);
            }
        }

        private void Km(String texto)
        {

            try
            {
                String km = texto.Split(' ')[1];
                this.KM = Int32.Parse(km);
                //System.out.println("Km: "+d);
            }
            catch (Exception nfe)
            {
                DebugLog.Logar(nfe.Message);
            }
        }
    }
}