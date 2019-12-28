using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Xml.Serialization;
using Microsoft.Office.Interop.Excel;


namespace TestAppWpf
{
    static class Export
    {
        



        public static void AsXml<T> (ObservableCollection<T> users, string saveFilePath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<User>));
            using (StreamWriter wr = new StreamWriter(saveFilePath))
            {
                xs.Serialize(wr, users);
            }
        }

        private static System.Data.DataTable ConvertToDataTable<T>(IEnumerable<T> users)

        {
            PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = new System.Data.DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
                
            foreach (T item in users)

            {

                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                    
                table.Rows.Add(row);

            }

            return table;

        }

        public static void AsXls(Dictionary<IEnumerable<User>, string> usersList, string saveFilePath)
        {
            Application excelApp = new Application();
            excelApp.Workbooks.Add();

            int counter = 0;

            foreach (KeyValuePair<IEnumerable<User>,string> u in usersList)
            {
                System.Data.DataTable table = ConvertToDataTable(u.Key);
                Worksheet workSheet = excelApp.ActiveSheet;
                workSheet.Name = u.Value;

                for(int i=0; i < table.Columns.Count; i++)
                {
                    workSheet.Cells[1, i + 1] = table.Columns[i].ColumnName;
                }

                for(int i=0;i < table.Rows.Count;i++)
                {
                    for(int j=0; j < table.Columns.Count;j++)
                    {
                        workSheet.Cells[i + 2, j + 1] = table.Rows[i][j];
                    }
                }
                
                workSheet.SaveAs(saveFilePath);
                counter++;

                if(counter<usersList.Count)
                {
                    excelApp.Worksheets.Add();
                }
                

            }

            excelApp.Quit();
        }

        
    }
}
