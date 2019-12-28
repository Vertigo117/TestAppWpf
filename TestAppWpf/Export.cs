using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Office.Interop.Excel;


namespace TestAppWpf
{
    static class Export
    {
        public static void AsXml (ObservableCollection<User> users, string saveFilePath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<User>));
            using (StreamWriter wr = new StreamWriter(saveFilePath))
            {
                xs.Serialize(wr, users);
            }
        }
    }
}
