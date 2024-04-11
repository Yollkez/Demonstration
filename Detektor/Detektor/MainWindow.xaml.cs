using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Management;
using System.Threading;
using System.ComponentModel;

namespace Detektor
{
    
    public partial class MainWindow : Window
    {
        Thread thread;
        Action action;

        public MainWindow()
        {
            InitializeComponent();
            action = new Action(() => { pbStatus.Value += 1; });
            List<string> result = new List<string>();

            ManagementObjectSearcher CPU_Searcher = new ManagementObjectSearcher("SELECT * FROM " + "Win32_Processor");

            foreach (ManagementObject obj in CPU_Searcher.Get())
            {
                result.Add("Процессор:                  " + obj["Name"].ToString().Trim());
                result.Add("Производитель:          " + obj["Manufacturer"].ToString().Trim());
                result.Add("Описание:                   " + obj["Description"].ToString().Trim());

            }

            ManagementObjectSearcher VidCardSearcher = new ManagementObjectSearcher("SELECT * FROM " + "Win32_VideoController");

            foreach (ManagementObject obj in VidCardSearcher.Get())
            {
                result.Add("Видеокарта:              " + obj["Name"].ToString().Trim());
                result.Add("Видеопроцессор:          " + obj["VideoProcessor"].ToString().Trim());
                result.Add("Версия драйвера:         " + obj["DriverVersion"].ToString().Trim());
                result.Add("Объем памяти (в байтах): " + obj["AdapterRAM"].ToString().Trim());

            }

            InfoList.ItemsSource = result;

            
            
        }


        private string CheckMemoryUsage()
        {

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                var hddUsage = searcher.Get().Cast<ManagementObject>().Select(m => new
                {
                    TotalVisibleMemorySize = m["TotalVisibleMemorySize"],
                    FreeMemory = m["FreePhysicalMemory"]
                }).FirstOrDefault();
                if (hddUsage == null) throw new Exception("Can't recive informacion about memory usage.");
                var freeRam = (ulong)hddUsage.FreeMemory;
                var totalRam = (ulong)hddUsage.TotalVisibleMemorySize;
                var RAM = (totalRam - freeRam) / (float)totalRam * 100;

                return RAM.ToString();
            }
            catch (Exception exception)
            {
                return null;
            }
        }
        
        private static string CurrentCPUusage
        {

            get
            {
                System.Management.ManagementObjectSearcher CpuLoad = new 
                    System.Management.ManagementObjectSearcher("SELECT LoadPercentage  FROM Win32_Processor");
                foreach (System.Management.ManagementObject obj in CpuLoad.Get())
                {
                    return obj["LoadPercentage"].ToString();
                }
                return null;
            }
            
        }

        private string CPUusageCounter()
        {
            int i = 0;
            int sum = 0;
            string CPUUsage = CurrentCPUusage;
            pbStatus.Value = 0;
            while (i != 5){
                sum = sum + int.Parse(CPUUsage);
                Thread.Sleep(100);
                i++;
            }
            sum = sum / 5;
            CPUusage.Content = sum.ToString()+"%";
            if (sum > 80){
                return "1";
            }
            else if(sum <= 80 & sum > 60)
            {
                return "2";
            }
            else if (sum <= 60 & sum > 40)
            {
                return "3";
            }
            else if (sum <= 40 & sum > 20)
            {
                return "4";
            }
            else
            {
                return "5";
            }
        }

        private string RAMusageCounter()
        {
            int i = 0;
            double sum = 0;

            

            string RAMUsage = CheckMemoryUsage();
            while (i != 5)
            {
                sum = sum + double.Parse(RAMUsage);
                Thread.Sleep(1000);
                i++;
            }
            Math.Round(sum);
            sum = sum / 5;
            RAMusage.Content = Math.Round(sum).ToString()+"%";
            if (sum > 90)
            {
                return "1";
            }
            
            else if (sum <= 90 & sum >= 40)
            {
                return "2";
            }
            
            else
            {
                return "3";
            }
        }
        
        private void wdaw_Click(object sender, RoutedEventArgs e)
        {

            thread = new Thread(DoWork);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
            Thread.Sleep(1000);

            switch (RAMusageCounter())
            {
                
                case "1":
                    {
                        RAMcolor.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        break;
                    }
                case "2":
                    {
                        RAMcolor.Background = new SolidColorBrush(Color.FromRgb(235, 247, 6));
                        break;
                    }
                case "3":
                    {
                        RAMcolor.Background = new SolidColorBrush(Color.FromRgb(6, 247, 81));
                        break;
                    }
                
            }

            switch (CPUusageCounter())
            {
                case "1":
                    {
                        CPUcolor.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        break;
                    }
                case "2":
                    {
                        CPUcolor.Background = new SolidColorBrush(Color.FromRgb(247, 119, 6));
                        break;
                    }
                case "3":
                    {
                        CPUcolor.Background = new SolidColorBrush(Color.FromRgb(235, 247, 6));
                        break;
                    }
                case "4":
                    {
                        CPUcolor.Background = new SolidColorBrush(Color.FromRgb(157, 247, 6));
                        break;
                    }
                case "5":
                    {
                        CPUcolor.Background = new SolidColorBrush(Color.FromRgb(6, 247, 81));
                        break;
                    }
            }
           



        }

        private void DoWork()
        {
            for (int i = 0; i <= 100; i++)
            {
                Progress();
                Thread.Sleep(100);
            }
        }
        private void Progress()
        {
            pbStatus.Dispatcher.BeginInvoke(action);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Уровень загрузки ниже среднего \nМощности ваших комплектующих все еще достаточно");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Уровень загрузки низкий \nМощности ваших комплектующих достаточно");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Уровень загрузки средний \nСтоит задуматься о обновлении комплектующих");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Уровень загрузки выше среднего \nМощности ваших комплектующих уже не достаточно");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Уровень загрузки высокий \nРекомендуется обновиться комплектующие, чтобы повысить производительность");
        }

        private async void pbStatus_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }
    }
}
