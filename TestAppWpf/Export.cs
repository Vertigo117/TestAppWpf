using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using OfficeOpenXml;


namespace TestAppWpf
{
    static class Export
    {
        public static void AsXml<T> (ArrayList usersList, string saveFilePath)
        {
            int stringLengtgh = saveFilePath.Length - 4;
            saveFilePath = saveFilePath.Substring(0, stringLengtgh);

            foreach (IDictionary<IEnumerable<T>, string> dict in usersList)
            {
                foreach(KeyValuePair<IEnumerable<T>, string> keyValuePair in dict)
                {
                    string filename = keyValuePair.Value;
                    var users = keyValuePair.Key.ToList();

                        XmlSerializer xmlSerializer = new XmlSerializer(users.GetType(), new Type[] { typeof(User), typeof(UserOrganization), typeof(UserTimeOnline), typeof(UserConnection)});
                        using (StreamWriter writer = new StreamWriter(string.Format("{0}_{1}.xml", saveFilePath, filename)))
                        {
                            xmlSerializer.Serialize(writer, users);
                        }
                }
            }
        }

        

        public static void AsXls<T>(ArrayList usersList, string saveFilePath)
        {
            FileInfo fileInfo = new FileInfo(saveFilePath);

            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                foreach (IDictionary<IEnumerable<T>, string> dictrionary in usersList)
                {
                    foreach (KeyValuePair<IEnumerable<T>, string> pair in dictrionary)
                    {
                        
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add(pair.Value);

                        Type type = pair.Key.First().GetType();
                        PropertyInfo[] properties = type.GetProperties();

                        for (int i = 0; i < properties.Length; i++)
                        {
                            workSheet.Cells[1, i + 1].Value = properties[i].Name;
                            workSheet.Cells[1, i + 1].Style.Font.Bold = true;
                        }

                        for (int i = 0; i < pair.Key.Count(); i++)
                        {
                            for (int j = 0; j < properties.Length; j++)
                            {
                                var propertyValue = properties[j].GetValue(pair.Key.ElementAt(i));
                                workSheet.Cells[i + 2, j + 1].Value = propertyValue;

                                if (propertyValue is DateTime)
                                {
                                    workSheet.Cells[i + 2, j + 1].Style.Numberformat.Format = "dd.mm.yyyy hh:mm:ss";
                                }
                                
                            }
                        }

                        workSheet.Cells.AutoFitColumns();
                        workSheet.View.FreezePanes(2, 1);
                    }
                }

                excelPackage.Save();
            }
        }  
    }
}
