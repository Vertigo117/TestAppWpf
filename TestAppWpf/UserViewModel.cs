﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Input;
using TestAppWpf.Properties;


namespace TestAppWpf.ViewModel
{
    class UserViewModel : INotifyPropertyChanged
    {
        private string path = @"..\..\TXT\LOG.txt";
        private string errorPath = @"..\..\TXT\ERROR.txt";
        private string ipPattern = @"^((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]?)$";
        private string dateTimePattern = @"^([1-9]|([012][0-9])|(3[01])).([0]{0,1}[1-9]|1[012]).\d\d\d\d (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$";
        public ObservableCollection<User> Users { get;}
        Dictionary<IEnumerable<User>, string> filteredUsers;
        private Command openExportWindowCommand;
        private Command saveFileCommand;
        private bool calendarVisible;
        private bool state;
        private bool checkBoxPeriodState;
        private DateTime dateFrom;
        private DateTime dateTo;
        SaveFileDialog dialog;

        public DateTime DateFrom
        {
            get { return dateFrom; }
            set
            {
                dateFrom = value;
                OnPropertyChanged("DateFrom");
            }
        }

        public DateTime DateTo
        {
            get { return dateTo; }
            set
            {
                dateTo = value;
                OnPropertyChanged("DateTo");
            }
        }

        public bool CheckBoxPeriodState
        {
            get { return checkBoxPeriodState; }
            set
            {
                checkBoxPeriodState = value;
                OnPropertyChanged("CheckBoxPeriodState");
            }
        }
        

        public Command SaveFileCommand
        {
            get
            {
                return saveFileCommand ??
                    (saveFileCommand = new Command(s =>
                    {
                        dialog = new SaveFileDialog();
                        dialog.Filter = "Excel Worksheets|*.xls|XML Files|*.xml";
                        if (dialog.ShowDialog()==true)
                        {
                            //try
                            //{
                                FileSave();
                            //}
                            //catch(Exception ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //}
                        }
                    }));
            }
        }

        public Visibility CalendarVisibility
        {
            get { return calendarVisible ? Visibility.Visible : Visibility.Hidden; }
        }

        public bool VisibilityCheckboxState
        {
            get { return state; }
            set
            {
                state = value;
                calendarVisible = value;
                OnPropertyChanged("VisibilityCheckboxState");
                OnPropertyChanged("CalendarVisibility");
            }
        }


        public Command OpenExportWindowCommand
        {
            get 
            {
                return openExportWindowCommand ??
                    (openExportWindowCommand = new Command(o => { ExportWindow exportWindow = new ExportWindow(); exportWindow.ShowDialog(); })) ;
            }
        }



        public UserViewModel()
        {
            calendarVisible = false;
            Users = new ObservableCollection<User>();
            File.WriteAllText(errorPath, string.Empty);
            ParseLOG();
            
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private bool RegexCheck(User user)
        {
            return (Regex.IsMatch(user.Ip, ipPattern) && Regex.IsMatch(user.LoginTime.ToString(),dateTimePattern) && Regex.IsMatch(user.LogoutTime.ToString(),dateTimePattern));
        }

        private async void ParseLOG()
        {
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    string[] values = line.Split(';');

                    try
                    {
                        User user = new User()
                        {
                            Index = Convert.ToInt32(values[0]),
                            UserName = values[1],
                            Organization = values[2],
                            Ip = values[3],
                            SessionId = values[4],
                            LoginTime = Convert.ToDateTime(values[5]),
                            LogoutTime = Convert.ToDateTime(values[6])
                        };

                        if (RegexCheck(user))
                        {
                            Users.Add(user);
                        }
                        else
                        {
                            WriteERROR(string.Format("{0} [IP adress and/or DateTime value is incorrect]", line));
                        }

                    }
                    catch (Exception ex)
                    {
                        WriteERROR(string.Format("{0} [ERROR] : [{1}]", line, ex.Message));
                        continue;
                    }


                }
            }
        }

        private async void WriteERROR(string line)
        {
            using (StreamWriter sw = new StreamWriter(errorPath, true, Encoding.Default))
            {
                await sw.WriteLineAsync(line);
            }
        }

        private void FileSave()
        {
            filteredUsers = new Dictionary<IEnumerable<User>, string>();

            if(checkBoxPeriodState)
            {
                var filteredByPeriod = Users.Where(u => u.LoginTime >= dateFrom && u.LoginTime <= dateTo);
                filteredUsers.Add(filteredByPeriod, "Отчёт по ошибкам за период");
            }

            if (dialog.FilterIndex==1)
            {
                Export.AsXls(filteredUsers, dialog.FileName);
            }
            
            if(dialog.FilterIndex==2)
            {
                Export.AsXml(Users, dialog.FileName);
            }
        }

    }
    
}
