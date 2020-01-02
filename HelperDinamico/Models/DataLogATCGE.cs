using HelperDinamico.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelperDinamico.Models
{
    public class DataLogATCGE
    {
       
        Dictionary<String, Object> Log = new Dictionary<String, Object>();

        public void Add(String log, int linha, String texto)
        {
            
            String data = texto.Split(' ')[5];

            if (Log[log] != null)
            {

                Dictionary<Int32, String> D = (Dictionary<Int32, String>)Log[log];
                D.Add(linha, data);
                Log.Add(log, D);

            }
            else
            {

                Dictionary<Int32,String> D = new Dictionary<Int32, String>();
                D.Add(linha,data);
                Log.Add(log, D);
            }
            
        }

        public DateTime DataLogATC(String strData)
        {
            DateTime myDate = new DateTime();
            try
            {
               
                myDate = DateTime.ParseExact(strData, "HH:mm:ss.SS MM/dd/yy", System.Globalization.CultureInfo.InvariantCulture);
              
            }
            catch (Exception e)
            {
                DebugLog.Logar(strData + " : " + e.Message);
            }

            return myDate;
        }


        public void setDataCorrenteSinalVia(CorrenteSinalVia correnteSinalVia) {
            
            Dictionary<Int32, String> Datas = (Dictionary<Int32, String>)Log[correnteSinalVia.log];

            foreach (var pair in Datas)
            {
                if(pair.Key > correnteSinalVia.linha)
                {
                    String strData = correnteSinalVia.data + " " + pair.Value;
                    if (!pair.Value.Equals("00/00/00"))
                    {
                        correnteSinalVia.data = DataLogATC(strData);
                        break;
                    }
                   
                }
            }
        }
    }
}