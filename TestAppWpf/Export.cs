﻿using System;
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

        private static System.Data.DataTable ConvertToDataTable<T>(ObservableCollection<T> users)

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

        public static void AsXls<T>(ObservableCollection<T> users, string saveFilePath)
        {
            System.Web.UI.WebControls.DataGrid grid = new System.Web.UI.WebControls.DataGrid();
            grid.HeaderStyle.Font.Bold = true;
            grid.DataSource = ConvertToDataTable(users);

            grid.DataBind();

            using (StreamWriter sw = new StreamWriter(saveFilePath))
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    grid.RenderControl(hw);
                }
            }
        }

        
    }
}
