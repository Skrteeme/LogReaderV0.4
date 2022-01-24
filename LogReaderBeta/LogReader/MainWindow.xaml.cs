using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace LogReader
{
    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TelegramClient telegramClient = new TelegramClient();

        [Obsolete]
        public MainWindow()
        {
            InitializeComponent();
            telegramClient.TelegramBot();
        }

        Repository Rep = new Repository();
        List<SGTIN> BadSGTINLIST = new List<SGTIN>();
        List<SRID> GoodSRIDLIST = new List<SRID>();
        bool RVFlag = false;

        /// <summary>
        /// Вывод информации по конкретному SRID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Solo_Button_Click (object sender, RoutedEventArgs e)
        {
            if (RVFlag)
            {
                InfoWindow infoWindow = new InfoWindow();
                infoWindow.Show();

                string SenderString = sender.ToString();
                foreach (var item in GoodSRIDLIST)
                {

                    if (SenderString.Contains(item.SRIDNumber))
                    {
                        infoWindow.InfoBox.Text += Rep.OutputInfoAboutSRID(item);
                    }
                }
            }
            
        }

        /// <summary>
        /// Поиск по SGTIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (RVFlag)
            {
                ResultBox.Text = Rep.SearchSGTINInBD(Rep.LoadSGTINList(EnterBox.Text), GoodSRIDLIST, BadSGTINLIST);
                Listobox.ItemsSource = GoodSRIDLIST;
                Listobox_Bad.ItemsSource = BadSGTINLIST;
                Listobox.Items.Refresh();
                Listobox_Bad.Items.Refresh();
            }
            
        }

        /// <summary>
        /// Поиск по SRID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SRIDButton_Click(object sender, RoutedEventArgs e)
        {
            if (RVFlag)
            {
                ResultBox.Text = Rep.SearchSRIDInBD(Rep.LoadSGTINList(EnterBox.Text), GoodSRIDLIST, BadSGTINLIST);
                Listobox.ItemsSource = GoodSRIDLIST;
                Listobox_Bad.ItemsSource = BadSGTINLIST;
                Listobox.Items.Refresh();
                Listobox_Bad.Items.Refresh();
            }
            
        }

        /// <summary>
        /// Загрузка файлов с логами, распаковка и загрузка в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Upload_Drop(object sender, DragEventArgs e)
        {
            
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                Rep.DeleteRV();
                RVFlag = false;
                GoodSRIDLIST.Clear();
                BadSGTINLIST.Clear();
                Rep.ClearTempFolder();
                string[] filePaths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                string Path = @".\Temp\Local\" + filePaths[0].Substring(filePaths[0].Length - 5);
                File.Copy(filePaths[0], Path);
                Rep.UnzipRARand7z();
                Rep.CreateRV();
                RVFlag = true;
                Rep.SearchInFiles();
                Rep.ClearTempFolder();
                ResultBox.Text += "Загрузка данных завершена!\n";
            }

        }
        /// <summary>
        /// Сохранение Анализа логов в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RVFlag)
            {
                Rep.SaveDataToFile(Rep.InfoAboutSRIDonTelegramMass(GoodSRIDLIST, BadSGTINLIST));
            }
        }

        /// <summary>
        /// Вывести пароль тех. поддержки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TechPassButton_Click(object sender, RoutedEventArgs e)
        {
            string Num = "";
            if (EnterBox.Text.Length == 16) Num = $"{EnterBox.Text[14] }{EnterBox.Text[15]}";
            if (EnterBox.Text.Length == 2) Num = EnterBox.Text;
            if (EnterBox.Text.Length == 16 || EnterBox.Text.Length == 2)
            {
                DateTime date = DateTime.Now.ToUniversalTime();
                string Hoursstring = $"{date:HH}";
                if (Hoursstring.Length < 2) Hoursstring = $"0{Hoursstring}";
                string Daysstring = $"{date:dd}";
                if (Hoursstring.Length < 2) Daysstring = $"0{Daysstring}";
                string PredPass = $"{Hoursstring}{Num}{Daysstring}";
                int PredPassInt = Convert.ToInt32(PredPass);
                string Pass = Convert.ToString(PredPassInt, 8);
                if (Pass.Length < 6) Pass = $"0{Pass}";
                ResultBox.Text = $"Пароль тех. поддержки = {Pass}";
            }
        }

        private void AKB_Click(object sender, RoutedEventArgs e)
        {
            if (RVFlag)
            {
                InfoWindow infoWindow = new InfoWindow();
                infoWindow.Show();
                infoWindow.InfoBox.Text += Rep.DeviceInfoHandler();
            }
            
        }

        private void MB_Click(object sender, RoutedEventArgs e)
        {
            if (RVFlag)
            {
                InfoWindow infoWindow = new InfoWindow();
                infoWindow.Show();
                infoWindow.InfoBox.Text += Rep.MBErrorHandler();
            }
            
        }

        private void MK_Click(object sender, RoutedEventArgs e)
        {
            if (RVFlag)
            {
                InfoWindow infoWindow = new InfoWindow();
                infoWindow.Show();
                infoWindow.InfoBox.Text += Rep.MKErrorHandler();
            }
            
        }

        private void Sattelite_Click(object sender, RoutedEventArgs e)
        {
            if (RVFlag)
            {
                InfoWindow infoWindow = new InfoWindow();
                infoWindow.Show();
                infoWindow.InfoBox.Text += Rep.SatteliteHandler();
            }
            
        }
    }
}
