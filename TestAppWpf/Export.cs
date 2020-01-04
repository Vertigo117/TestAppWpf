using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Xml.Serialization;
using Microsoft.Office.Interop.Excel;


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
                foreach(KeyValuePair<IEnumerable<T>,string> keyValuePair in dict)
                {
                    string filename = keyValuePair.Value;



                    //foreach (T user in keyValuePair.Key)
                    //{
                        XmlSerializer xs = new XmlSerializer(typeof(List<T>));
                        using (StreamWriter wr = new StreamWriter(string.Format("{0}_{1}.xml", saveFilePath, filename)))
                        {
                            xs.Serialize(wr, (List<T>)keyValuePair.Key.ToList());
                        }
                    //}
                }
            }
        }

        

        public static void AsXls<T>(ArrayList usersList, string saveFilePath)
        {
            Application excelApp = new Application();
            Range excelCellrange;
            excelApp.Workbooks.Add();
            Worksheet workSheet = excelApp.ActiveSheet;


            int counter = 0;

            foreach (IDictionary<IEnumerable<T>,string> u in usersList)
            {
                

                foreach (KeyValuePair<IEnumerable<T>,string> pair in u)
                {
                    workSheet = excelApp.ActiveSheet;
                    workSheet.Name = pair.Value;
                    Type t = pair.Key.First().GetType();
                    PropertyInfo[] properties = t.GetProperties();


                    for (int i = 0; i < properties.Length; i++)
                    {
                        workSheet.Cells[1, i + 1] = properties[i].Name;
                    }

                    for(int i = 0; i < pair.Key.Count(); i++)
                    {
                        for(int j =0;j<properties.Length;j++)
                        {
                            workSheet.Cells[i + 2, j + 1] = properties[j].GetValue(pair.Key.ElementAt(i));
                        }
                    }
                        

                    //TODO Проверить на пустоту, иногда критует
                    excelCellrange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[pair.Key.Count(), pair.Key.First().GetType().GetProperties().Length]];
                    excelCellrange.EntireColumn.AutoFit();
                    counter++;

                    if (counter < usersList.Count)
                    {
                        excelApp.Worksheets.Add();
                    }
                }


            }


            workSheet.SaveAs(saveFilePath);
            excelApp.Quit();
        }

        
    }
}
