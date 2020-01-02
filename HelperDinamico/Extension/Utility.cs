using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace HelperDinamico.Extension
{
    public class Utility
    {
        internal static DataTable ExportListToDataTable<T>(List<T> objects)
        {
            DataTable table = new DataTable();
            try
            {
                if (objects.Count > 0)
                {
                    var myCastedObject = Convert.ChangeType(objects[0], typeof(T));
                    //getting collumns
                    foreach (var prop in myCastedObject.GetType().GetProperties())
                    {
                        if (myCastedObject.GetType().GetProperty(prop.Name).PropertyType.FullName.StartsWith("System."))
                        {
                            DataColumn col = new DataColumn();
                            col.ColumnName = GetPropertyLabel(prop.Name.Clone().ToString());
                            table.Columns.Add(col);
                        }
                    }
                    foreach (T obj in objects)
                    {
                        myCastedObject = Convert.ChangeType(obj, typeof(T));
                        DataRow row = table.NewRow();
                        foreach (var prop in myCastedObject.GetType().GetProperties())
                        {
                            if (myCastedObject.GetType().GetProperty(prop.Name).PropertyType.FullName.StartsWith("System."))
                            {
                                var propertyValue = myCastedObject.GetType().GetProperty(prop.Name).GetValue(myCastedObject, null);
                                row[GetPropertyLabel(prop.Name.Clone().ToString())] = propertyValue != null ? propertyValue.ToString() : "";
                            }
                        }
                        table.Rows.Add(row);
                    }
                }
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.Message);
                DebugLog.Logar(e.StackTrace);
            }
            return table;
        }
        private static string GetPropertyLabel(string propertyName)
        {
            if (propertyName.Count() > 2)
            {
                propertyName = propertyName.Replace("Id", "");
                var propertySize = propertyName.Count();
                for (int i = 1; i < propertySize; i++)
                {
                    if (Char.IsUpper(propertyName.ElementAt(i)))
                    {
                        propertyName = propertyName.Insert(i, " ");
                        i++;
                        propertySize++;
                    }
                }
            }
            return propertyName;
        }

    }
}