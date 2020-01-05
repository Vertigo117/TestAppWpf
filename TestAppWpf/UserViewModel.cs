using Microsoft.Win32;
using System;
using System.Collections;
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
        private Command openExportWindowCommand;
        private Command saveFileCommand;
        private bool calendarVisible;
        private bool state;
        private bool checkBoxPeriodState;
        private bool checkBoxUsersFromOrganizations;
        private bool checkBoxOrgs;
        private bool checkBoxConnections;
        private bool checkBox24h;
        private DateTime dateFrom;
        private DateTime dateTo;
        SaveFileDialog dialog;

        public bool CheckBox24h 
        { 
            get
            {
                return checkBox24h;
            }
            set
            {
                checkBox24h = value;
                OnPropertyChanged("CheckBox24h");
            }
        }

        public bool CheckBoxConnections 
        { 
            get
            {
                return checkBoxConnections;
            }
            set
            {
                checkBoxConnections = value;
                OnPropertyChanged("CheckBoxConnections");
            }
        }

        public bool CheckBoxOrgs
        {
            get { return checkBoxOrgs; }
            set
            {
                checkBoxOrgs = value;
                OnPropertyChanged("CheckBoxOrgs");
            }
        }

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

        public bool CheckBoxUsersFromOrganizations
        {
            get { return checkBoxUsersFromOrganizations; }
            set
            {
                checkBoxUsersFromOrganizations = value;
                OnPropertyChanged("CheckBoxUsersFromOrganizations");
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
                        dialog.Filter = "Excel Worksheets|*.xlsx|XML Files|*.xml";
                        if (dialog.ShowDialog()==true)
                        {
                            try
                            {
                                FileSave();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
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
                    (openExportWindowCommand = new Command(o => 
                    {
                        ExportWindow exportWindow = new ExportWindow();
                        exportWindow.ShowDialog();
                    })) ;
            }
        }



        public UserViewModel()
        {
            calendarVisible = false;
            Users = new ObservableCollection<User>();
            File.WriteAllText(errorPath, string.Empty);
            ParseLOG();
            dateFrom = DateTime.Parse("20.06.2017");
            dateTo = DateTime.Now;
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
                            LogoutTime = Convert.ToDateTime(values[6]),
                            EndCode = Convert.ToInt32(values[7])
                        };

                        if (RegexCheck(user) && user.LoginTime<user.LogoutTime) //У некоторых пользователей время конца сессии раньше, чем начала. Этого не должно быть
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
            ArrayList filteredUsers = new ArrayList();
            

            if(checkBoxPeriodState)
            {
                var filteredByPeriod = Users.Where(u => u.LoginTime >= dateFrom && u.LoginTime <= dateTo && u.EndCode==1);
                Dictionary<IEnumerable<object>, string> keyValuePairs = new Dictionary<IEnumerable<object>, string>() { { filteredByPeriod, "Ошибки за период" } };

                if(filteredByPeriod != null)
                {
                    filteredUsers.Add(keyValuePairs);
                }
                
            }

            if (checkBoxUsersFromOrganizations)
            {
                var usersFromOrganizations = Users.Where(u => u.LoginTime >= dateFrom && u.LoginTime <= dateTo).GroupBy(i => i.Organization).Select(o => new UserOrganization { OrganizationName = o.Key, NumberOfUsers = o.Count() });
                Dictionary<IEnumerable<object>, string> keyValuePairs = new Dictionary<IEnumerable<object>, string>() { { usersFromOrganizations, "Пользователи от организации" } };

                if (usersFromOrganizations != null)
                {
                    filteredUsers.Add(keyValuePairs);
                }
            }

            if(checkBoxOrgs)
            {
                var orgs = Users.Select(s => new { s.UserName, s.Ip, TimeSubtr = (s.LogoutTime - s.LoginTime).TotalMinutes }).GroupBy(g => new { g.UserName, g.Ip }).Select(s => new UserTimeOnline { Total = s.Count(), UserName= s.Key.UserName, Ip= s.Key.Ip, TimeOnline = s.Sum(sum => sum.TimeSubtr) });
                Dictionary<IEnumerable<object>, string> keyValuePairs = new Dictionary<IEnumerable<object>, string>() { { orgs, "Отчёт по организациям" } };

                if (orgs != null)
                {
                    filteredUsers.Add(keyValuePairs);
                }
            }

            if(checkBoxConnections)
            {
                var connections = Users.Where(u => u.LoginTime >= dateFrom && u.LoginTime <= dateTo).GroupBy(g => new { g.UserName, g.Ip }).Select(s => new UserConnection { UserName = s.Key.UserName, Ip = s.Key.Ip, NumberOfConnections = s.Count() });
                Dictionary<IEnumerable<object>, string> keyValuePairs = new Dictionary<IEnumerable<object>, string>() { { connections, "Количество подключений" } };

                if (connections != null)
                {
                    filteredUsers.Add(keyValuePairs);
                }
            }

            if(CheckBox24h)
            {
                var day = Users.Where(d => DateTime.Now.Subtract(d.LoginTime).Days <= 1);
                Dictionary<IEnumerable<object>, string> keyValuePairs = new Dictionary<IEnumerable<object>, string>() { { day, "Отчёт за сутки" } };

                if (day != null)
                {
                    filteredUsers.Add(keyValuePairs);
                }
            }

            if (dialog.FilterIndex==1)
            {
                Export.AsXls<object>(filteredUsers, dialog.FileName);
            }

            if (dialog.FilterIndex == 2)
            {
                Export.AsXml<object>(filteredUsers, dialog.FileName);
            }
        }

    }
    
}
