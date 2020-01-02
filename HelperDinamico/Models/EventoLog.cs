using HelperDinamico.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Query.Dynamic;

namespace HelperDinamico.Models
{
    public class EventoLog
    {
        public DateTime time { get; set; }
        public String PONTO_DE_ACELERACAO { get; set; }
        public String INDICACAO_MAQUINISTA { get; set; }
        public int POSICAOHELPER_GPS { get; set; }
        public int VELOCREALGPSHELPER { get; set; }
        public double DIST_RELAT_LE { get; set; }
        public double DIST_RELAT_LD { get; set; }
        public double VEL_RELAT_LE { get; set; }
        public double VEL_RELAT_LD { get; set; }
        public int PRESSAOFREIO { get; set; }
        public int PRESSAO_CILINDRO_PRINCIPAL { get; set; }
        public int BAT_EOT { get; set; }
        public int ID_EOT { get; set; }
        public int PRESSÃO_EG_EOT { get; set; }
        public int VELOC_REAL_GPS_EOT { get; set; }

        public int CILINDRO_RETORNO_E { get; set; }
        public int CILINDRO_AVANCO_E { get; set; }
        public int CILINDRO_TELEMETRO { get; set; }
        public int CHAVE_INTERLOCK { get; set; }
        public int TECLA_TELA { get; set; }
        public int TECLA_BZRESET { get; set; }
        public int TECLA_LIP_PINO_ENGATE { get; set; }
        public int TECLA_OP_ALINHAR { get; set; }

        public int E { get; set; }
        public int D { get; set; }
        public int C { get; set; }
        public int B { get; set; }
        public int A { get; set; }

        public int CILINDRO_PINO_ENGATE { get; set; }
        public int CILINDRO_RETORNO_D { get; set; }
        public int CILINDRO_AVANCO_D { get; set; }
        public int CORTE_TRACAO { get; set; }
        public int EMERGENCIA { get; set; }
        public int SIRENE { get; set; }
        public int VALVULA_TELEMETRO { get; set; }
        public int VALVULA_RETORNO_ALINHAMENTO { get; set; }
        public int VALVULA_AVANÇO_ALINHAMENTO { get; set; }
        public int VALVULA_PINO_ENGATE { get; set; }
        public int VELOC_RELATIVA_GPS_EOT_HELPER { get; set; }
        public int DIST_RELATIVA_GPS_EOT_HELPER { get; set; }
        public int POSICAO_EOT_GPS { get; set; }
        public int ESTADO_GPS_EOT { get; set; }
        public int CONT_GPS_EOT_VIVO { get; set; }
        public int ESTADO_GPS_HELPER { get; set; }
        public int CONT_GPS_HELPER_VIVO { get; set; }
        public int ESTADO_LASER_END2 { get; set; }
        public int CON_LASER_END2 { get; set; }
        public int ESTADO_LASER_END3 { get; set; }
        public int CONT_LASER_END3 { get; set; }
        public int TEMP_LASER_END2 { get; set; }
        public int TEMP_LASER_END3 { get; set; }
        public String ALARME1 { get; set; }
        public String ALARME2 { get; set; }
        public String ALARME3 { get; set; }
        public String LOCALIZACAO_OK_HELPER { get; set; }
        
        public EventoLog(String ev)
        {
            try
            {
                Time(ev);
                GPSHELPER(ev);
                DIST_VEL_RELAT_Laser(ev);
                EOT(ev);
                Binario(ev);
                Indicacao_Maquinista(ev);
                Alarmes(ev);
                Pressao(ev);
            }
            catch (Exception e)
            {
                //JOptionPane.showMessageDialog(null, "Arquivo TXT corrompido !!!");
                DebugLog.Logar("EventoLog " + e.Message);
                DebugLog.Logar(e.StackTrace);
            }

        }

        void Time(String ev)
        {
            try
            {
                String time_string = ev.Substring(1, 12);
                DateTime myDate = DateTime.ParseExact(time_string, "ddMMyyHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                time = myDate;
            }
            catch (ParseException e)
            {
                // TODO Auto-generated catch block
                //JOptionPane.showMessageDialog(null, "Arquivo TXT corrompido na Data e Hora!!!");
                DebugLog.Logar("EventoLog " + ev + " : " + e.Message);
            }
        }

        void GPSHELPER(String ev)
        {

            POSICAOHELPER_GPS = Int32.Parse(ev.Substring(13, 6).Replace("+", ""));
            VELOCREALGPSHELPER = Int32.Parse(ev.Substring(19, 2));
            VELOC_RELATIVA_GPS_EOT_HELPER = Int32.Parse(ev.Substring(73, 2));
            DIST_RELATIVA_GPS_EOT_HELPER = Int32.Parse(ev.Substring(76, 4));
            ESTADO_GPS_HELPER = Int32.Parse(ev.Substring(95, 1));
            CONT_GPS_HELPER_VIVO = Int32.Parse(ev.Substring(96, 3));
        }

        void DIST_VEL_RELAT_Laser(String ev)
        {
            DIST_RELAT_LE = Double.Parse(ev.Substring(21, 3)) / 10.0;
            VEL_RELAT_LE = Double.Parse(ev.Substring(25, 2));

            DIST_RELAT_LD = Double.Parse(ev.Substring(27, 3)) / 10.0;
            VEL_RELAT_LD = Double.Parse(ev.Substring(31, 2));

            ESTADO_LASER_END2 = Int32.Parse(ev.Substring(99, 1));
            CON_LASER_END2 = Int32.Parse(ev.Substring(100, 3));
            ESTADO_LASER_END3 = Int32.Parse(ev.Substring(103, 1));
            CONT_LASER_END3 = Int32.Parse(ev.Substring(104, 3));

            TEMP_LASER_END2 = Int32.Parse(ev.Substring(116, 2));
            TEMP_LASER_END3 = Int32.Parse(ev.Substring(118, 2));
        }

        void Pressao(String ev)
        {
            PRESSAOFREIO = Int32.Parse(ev.Substring(33, 3));
            PRESSAO_CILINDRO_PRINCIPAL = Int32.Parse(ev.Substring(36, 3));

        }

        void EOT(String ev)
        {
            BAT_EOT = Int32.Parse(ev.Substring(39, 3));
            ID_EOT = Int32.Parse(ev.Substring(42, 2));
            PRESSÃO_EG_EOT = Int32.Parse(ev.Substring(44, 2));
            VELOC_REAL_GPS_EOT = Int32.Parse(ev.Substring(46, 2));
            POSICAO_EOT_GPS = Int32.Parse(ev.Substring(81, 5));
            ESTADO_GPS_EOT = Int32.Parse(ev.Substring(91, 1));
            CONT_GPS_EOT_VIVO = Int32.Parse(ev.Substring(92, 3));
        }

        void Binario(String ev)
        {

            CILINDRO_RETORNO_E = Int32.Parse(ev.Substring(48, 1));
            CILINDRO_AVANCO_E = Int32.Parse(ev.Substring(49, 1));
            CILINDRO_TELEMETRO = Int32.Parse(ev.Substring(50, 1));
            CHAVE_INTERLOCK = Int32.Parse(ev.Substring(51, 1));
            TECLA_TELA = Int32.Parse(ev.Substring(52, 1));
            TECLA_BZRESET = Int32.Parse(ev.Substring(53, 1));
            TECLA_LIP_PINO_ENGATE = Int32.Parse(ev.Substring(54, 1));
            TECLA_OP_ALINHAR = Int32.Parse(ev.Substring(55, 1));

            E = Int32.Parse(ev.Substring(56, 1));
            D = Int32.Parse(ev.Substring(57, 1));
            C = Int32.Parse(ev.Substring(58, 1));
            B = Int32.Parse(ev.Substring(59, 1));
            A = Int32.Parse(ev.Substring(60, 1));

            CILINDRO_PINO_ENGATE = Int32.Parse(ev.Substring(61, 1));
            CILINDRO_RETORNO_D = Int32.Parse(ev.Substring(62, 1));
            CILINDRO_AVANCO_D = Int32.Parse(ev.Substring(63, 1));
            PONTO_DE_ACELERACAO = ev.Substring(64, 1);
            CORTE_TRACAO = Int32.Parse(ev.Substring(65, 1));
            EMERGENCIA = Int32.Parse(ev.Substring(66, 1));
            SIRENE = Int32.Parse(ev.Substring(67, 1));
            VALVULA_TELEMETRO = Int32.Parse(ev.Substring(68, 1));
            VALVULA_RETORNO_ALINHAMENTO = Int32.Parse(ev.Substring(69, 1));
            VALVULA_AVANÇO_ALINHAMENTO = Int32.Parse(ev.Substring(70, 1));
            VALVULA_PINO_ENGATE = Int32.Parse(ev.Substring(71, 1));

        }

        void Indicacao_Maquinista(String ev)
        {
            INDICACAO_MAQUINISTA = ev.Substring(86, 5);
        }


        void Alarmes(String ev)
        {
            ALARME1 = ev.Substring(107, 3);
            ALARME2 = ev.Substring(110, 3);
            ALARME3 = ev.Substring(113, 3);
            LOCALIZACAO_OK_HELPER = ev.Substring(116, 41);
        }
    }
}