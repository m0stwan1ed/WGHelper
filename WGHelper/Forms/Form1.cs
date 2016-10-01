using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;                                         //Подключение многопоточности
using System.Windows.Forms;
using Newtonsoft.Json;                                          //Библиотека для работы с JSON
using System.Net.NetworkInformation;                            //Библиотека для работы функции проверки доступности серверов
using System.Xml.Linq;                                          //Библиотека XML
using System.Diagnostics;                                       //Библиотека для работы с процессами

namespace WGHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();                      //Инициализация компонентов формы. Должна быть первой всегда
            //------------------------------------------------------------------------------------------------
            checkAuthorization();
            Control.CheckForIllegalCrossThreadCalls = false;    //Отключение отслеживания пересекания потоков
            updateWoTClientVersion();                           //Вывод данных о версии клиента игры и версии приложения
            startUpdating();                                    //Запуск потока, реквест-респонс
            pingTestWoT();                                      //Запуск потока проверки доступности серверов
            pingWoTServers_timer.Start();                       //Запуск таймера, который через промежутки времени проводит тест задержек доступа к серверам
        }

        XDocument settings;                                     //Объект для хранения XML-файла

        //----------------------Запуск потока, реквест-респонс--------------------------
        void startUpdating()
        {
            button_Retry.Visible = false;
            updateWoTServersStats_timer.Start();                                     //Запуск таймера
            Thread requestWoTOnline;                                                 //Инициализация потока
            requestWoTOnline = new Thread(request);                                  //Привязка функции к потоку
            label_updatingInfo.Text = "Updating...";
            label_updatingInfo.Visible = true;                                       //Отображение информатора загрузки
            pictureBox1.Image = Properties.Resources.loading_sh;                     //Смена изображения информатора
            requestWoTOnline.IsBackground = true;                                    //Поток после полного выполнения самоуничтожается
            requestWoTOnline.Start();
        }
        //-------------------------------------------------------------------------------

        void checkAuthorization()
        {
            settings = XDocument.Load("settings.xml");
            if (settings.Element("settings").Element("wg_open_id").Element("authorized").Value == "no")
            {
                authorizationToolStripMenuItem.Enabled = true;
                logoutToolStripMenuItem.Enabled = false;
            }
            else
            {
                int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                label1.Text = unixTime.ToString();
                if (unixTime > Convert.ToInt32( settings.Element("settings").Element("wg_open_id").Element("experies_at").Value))
                {
                    WebRequest logoutRequest = WebRequest.Create("https://api.worldoftanks.ru/wot/auth/logout/?application_id=146bc6b8d619f5030ed02cdb5ce759b4&access_token=" + settings.Element("settings").Element("wg_open_id").Element("access_token").Value);
                    WebResponse logoutResponse = logoutRequest.GetResponse();
                    settings.Element("settings").Element("wg_open_id").Element("access_token").Value = "";
                    settings.Element("settings").Element("wg_open_id").Element("nickname").Value = "";
                    settings.Element("settings").Element("wg_open_id").Element("account_id").Value = "";
                    settings.Element("settings").Element("wg_open_id").Element("experies_at").Value = "";
                    settings.Element("settings").Element("wg_open_id").Element("authorized").Value = "no";
                    settings.Save("settings.xml");
                    checkAuthorization();
                }
                else
                {
                    authorizationToolStripMenuItem.Enabled = false;
                    logoutToolStripMenuItem.Enabled = true;
                }
            }
        }

        bool requestWoTThreadState = false;                     //Переменная-маркер для проверки активности потока
        bool pingWoTThreadState = false;                        //Переменная-маркер для проверки потока Ping

        public class wotOnlineRootObject                        //Основной класс для десериализации JSON-ответа от сервера
        {
            public string status { get; set; }                  //Статус ответа
            public Data data { get; set; }                      //Данные о серверах
        }

        public class Data
        {
            public List<Wot> wot { get; set; }                  //Список серверов
        }

        public class Wot
        {
            public int players_online { get; set; }             //Количество игроков онлайн
            public string server { get; set; }                  //Номер сервера "RU-"
        }

        public static string urlServerOnline = "https://api.worldoftanks.ru/wgn/servers/info/";             //URI запроса
        public static string urlServerOnlineRequest = "application_id=demo&game=wot";                       //Запрос к серверу
        public static string jsonAnswer;                                                                    //Переменная ответа сервера
        public static wotOnlineRootObject wotOnline;                                                        //Объект класса для десериализации ответа сервера

        //---------------------------Функция, что выполняет запрос в отдельном потоке-------------------------------
        public void request()                                                                               
        {
            try     //Отлавливаем System.Net.WebException(отсутствие соединения с сервером)
            {
                requestWoTThreadState = true;                                                                                   //Маркер выполнения потока(выполняется)
                WebRequest requestServerOnline = WebRequest.Create(urlServerOnline + "?" + urlServerOnlineRequest);             //Инициализация запроса
                WebResponse responseServerOnline = requestServerOnline.GetResponse();                                           //Выполнение запроса и получение ответа
                Stream answerStream = responseServerOnline.GetResponseStream();                                                 //Конвертирование ответа в Stream
                StreamReader srAnswer = new StreamReader(answerStream);                                                         //Конвертирование Stream в StreamReader
                jsonAnswer = srAnswer.ReadToEnd();                                                                              //Конвертирование StreamReader в String
                wotOnlineRootObject wotOnline = JsonConvert.DeserializeObject<wotOnlineRootObject>(jsonAnswer);                 //Десериализация JSON и запись данных в объект класса
                bool[] serverStatusWoT = new bool[10];                                                                          //Переменная для проверки или сервер в сети

                for (int i=0; i<10; i++)                                                                                        //Обнуление переменных
                {
                    serverStatusWoT[i] = false;
                }

                //----------Создание коллекции объектов типа Label----------
                var labelsWoTOnline = new List<Label>();
                //--------------Заполнение коллекции объектами--------------
                labelsWoTOnline.Add(label_ru1Online);
                labelsWoTOnline.Add(label_ru2Online);
                labelsWoTOnline.Add(label_ru3Online);
                labelsWoTOnline.Add(label_ru4Online);
                labelsWoTOnline.Add(label_ru5Online);
                labelsWoTOnline.Add(label_ru6Online);
                labelsWoTOnline.Add(label_ru7Online);
                labelsWoTOnline.Add(label_ru8Online);
                labelsWoTOnline.Add(label_ru9Online);
                labelsWoTOnline.Add(label_ru10Online);
                //----------------------------------------------------------

                //----------------Выведение данных в label по серверам и определение, есть ли сервер в сети
                for (int i = 0; i < wotOnline.data.wot.Count; i++)                                                              
                {                                                                                                               
                    switch (wotOnline.data.wot[i].server)
                    {
                        case "RU1":
                            {
                                label_ru1Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[0] = true;
                                break;
                            }
                        case "RU2":
                            {
                                label_ru2Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[1] = true;
                                break;
                            }
                        case "RU3":
                            {
                                label_ru3Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[2] = true;
                                break;
                            }
                        case "RU4":
                            {
                                label_ru4Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[3] = true;
                                break;
                            }
                        case "RU5":
                            {
                                label_ru5Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[4] = true;
                                break;
                            }
                        case "RU6":
                            {
                                label_ru6Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[5] = true;
                                break;
                            }
                        case "RU7":
                            {
                                label_ru7Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[6] = true;
                                break;
                            }
                        case "RU8":
                            {
                                label_ru8Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[7] = true;
                                break;
                            }
                        case "RU9":
                            {
                                label_ru9Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[8] = true;
                                break;
                            }
                        case "RU10":
                            {
                                label_ru10Online.Text = wotOnline.data.wot[i].players_online.ToString();
                                serverStatusWoT[9] = true;
                                break;
                            }
                    }
                }
                //--------------------------------------------------------------------------------------------------

                //----------Создание коллекции объектов типа PictureBox----------
                var WoTStatusPictureBox = new List<PictureBox>();
                //--------------Заполнение коллекции объектами-------------------
                WoTStatusPictureBox.Add(ru1_status_pictureBox);
                WoTStatusPictureBox.Add(ru2_status_pictureBox);
                WoTStatusPictureBox.Add(ru3_status_pictureBox);
                WoTStatusPictureBox.Add(ru4_status_pictureBox);
                WoTStatusPictureBox.Add(ru5_status_pictureBox);
                WoTStatusPictureBox.Add(ru6_status_pictureBox);
                WoTStatusPictureBox.Add(ru7_status_pictureBox);
                WoTStatusPictureBox.Add(ru8_status_pictureBox);
                WoTStatusPictureBox.Add(ru9_status_pictureBox);
                WoTStatusPictureBox.Add(ru10_status_pictureBox);
                //---------------------------------------------------------------

                for (int i=0; i<10; i++)
                {
                    if (serverStatusWoT[i] == true) WoTStatusPictureBox[i].Image = Properties.Resources.rsz_green_light;
                    else
                    {
                        WoTStatusPictureBox[i].Image = Properties.Resources.rsz_red_light;
                        labelsWoTOnline[i].Text = "Offline";
                    }
                }
                //--------------------------------------------------------------------------------------------------
                
                int wotTotalOnline = 0;                                                                     //Подсчет общего онлайна на серверах WoT
                for (int i = 0; i < wotOnline.data.wot.Count; i++)
                {
                    wotTotalOnline += Convert.ToInt32(wotOnline.data.wot[i].players_online);
                }
                label_totalOnline.BeginInvoke((MethodInvoker)(delegate { label_totalOnline.Text = wotTotalOnline.ToString(); }));   //Вывод общего онлайна в label
                label_updatingInfo.Visible = false;                                                                                 //Информатор загрузки уходит в инвиз
                pictureBox1.Image = Properties.Resources.tick;                                                                      //Смена изображения-информатора
                requestWoTThreadState = false;                                                                                      //Маркер выполнения потока(не выполняется)
            }

            catch(System.Net.WebException)          //Обрабатываем исключение System.Net.WebException
            {
                button_Retry.Visible = true;                                                                                        //Вывод кнопки повторной попытки выполнения запроса

                requestWoTThreadState = false;                                                                                      //Маркер выполнения потока(не выполняется)
                label_updatingInfo.Text = "No connection";                                                                          //Указываем отсутствие соединения
                pictureBox1.Image = Properties.Resources.rsz_1no_icon;                                                              //Смена изображения-информатора

                //-----------------При этом статус серверов становится неизвестен
                ru1_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                ru2_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                ru3_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                ru4_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                ru5_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                ru6_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                ru7_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                ru8_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                ru9_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                ru10_status_pictureBox.Image = Properties.Resources.rsz_gray_light;
                label_ru1Online.Text = "Unknown";
                label_ru2Online.Text = "Unknown";
                label_ru3Online.Text = "Unknown";
                label_ru4Online.Text = "Unknown";
                label_ru5Online.Text = "Unknown";
                label_ru6Online.Text = "Unknown";
                label_ru7Online.Text = "Unknown";
                label_ru8Online.Text = "Unknown";
                label_ru9Online.Text = "Unknown";
                label_ru10Online.Text = "Unknown";
                //--------------------------------------------------------------------------------------------------

                updateWoTServersStats_timer.Stop();                                                                                 //Остановка таймера из-за отсутствия соединения с сервером

                MessageBox.Show("  Unable to connect to servers\nCheck your network connection", "Request error", MessageBoxButtons.OK, MessageBoxIcon.Error);               //Вывод на экран сообщения об ошибке
            }

        }
        //----------------------------------------------------------------------------------------------------------

        //-------------------Функция данных о версии клиента игры и версии приложения-------------------------------
        void updateWoTClientVersion()
        {
            settings = XDocument.Load("settings.xml");                                                              //Загружаем файл настроек
            label_appVersion.Text = settings.Element("settings").Attribute("version").Value;                        //Выводим текущую версию приложения
            if (settings.Element("settings").Element("worldoftanks").Element("installed").Value == "no")            //Проверяем, задан ли путь к клиенту игры
            {
                label_WoTClientVersion.Text = "Client not installed";                                               //Выводим сообщение "Клиент не установлен"
                button_RunClient.Visible = false;                                                                   //Убираем кнопки запуска игры и лаунчера
                button_RunUpdater.Visible = false;
            }
            else
            {
                label_WoTClientVersion.Text = "Client version: " + settings.Element("settings").Element("worldoftanks").Element("client_version").Value;//Выводим версию клиента
                button_RunClient.Visible = true;                                                                    //Отображаем кнопки запуска лаунчера и игры
                button_RunUpdater.Visible = true;
            }
        }
        //----------------------------------------------------------------------------------------------------------

        private void timer1_Tick(object sender, EventArgs e)                                                                    //Функция таймера
        {
            if (requestWoTThreadState == false)
            {
                Thread requestWoTOnline;                                                                                            //Инициализация потока
                requestWoTOnline = new Thread(request);                                                                             //Привязка функции к потоку
                label_updatingInfo.Text = "Updating...";
                label_updatingInfo.Visible = true;                                                                                  //Отображение информатора загрузки
                pictureBox1.Image = Properties.Resources.loading_sh;                                                                //Смена изображения информатора
                requestWoTOnline.IsBackground = true;                                                                               //Поток после полного выполнения самоуничтожается
                requestWoTOnline.Start();                                                                                           //Запуск потока, реквест-респонс                                                                                     
            }
        }

        private void button_Retry_Click(object sender, EventArgs e)                 //Функция повторной попытки запроса к серверам
        {
            startUpdating();
        }

        void pingTestWoT()                                                          //Функция запуска потока ping
        {
            Thread pingWoTServers_Thread;                                           //Обьявление переменной 
            pingWoTServers_Thread = new Thread(function_pingWoTServers_Thread);     //Привязка функции к потоку
            pingWoTServers_Thread.IsBackground = true;                              //Закрытие потока после окончания
            pingWoTServers_Thread.Start();                                          //Запуск потока
        }

        //----------------Функция, что выполняется в потоке для проверки доступности серверов----------------------
        void function_pingWoTServers_Thread()
        {
            try
            {
                label_pingInfo.Text = "Pinging...";
                button_RetryPing.Visible = false;
                //----------Создание коллекции объектов типа Label----------
                var labelsWoTPing = new List<Label>();
                //--------------Заполнение коллекции объектами--------------
                labelsWoTPing.Add(label_ru1Ping);
                labelsWoTPing.Add(label_ru2Ping);
                labelsWoTPing.Add(label_ru3Ping);
                labelsWoTPing.Add(label_ru4Ping);
                labelsWoTPing.Add(label_ru5Ping);
                labelsWoTPing.Add(label_ru6Ping);
                labelsWoTPing.Add(label_ru7Ping);
                labelsWoTPing.Add(label_ru8Ping);
                labelsWoTPing.Add(label_ru9Ping);
                labelsWoTPing.Add(label_ru10Ping);
                //----------------------------------------------------------

                pingWoTThreadState = true;
                pictureBox2.Image = Properties.Resources.loading_sh;
                label_pingInfo.Visible = true;
                Ping ping = new Ping();                                                 //Объект класса Ping для проверки скорости доступа к серверу

                PingReply[] WoTRu = new PingReply[10];                                  //Массив объектов класса PingReply для хранения параметров

                //-----------------Выполнение запросов на тест скорости доступа к серверам
                for (int i = 0; i < 10; i++)
                {
                    WoTRu[i] = ping.Send("login.p" + Convert.ToString(i + 1) + ".worldoftanks.net");
                }
                //-------------------------------------------------------------------------

                byte indexOfServerWoT = 0;                  //Ячейка для запоминания сервера с найменьшей задержкой доступа
                int pingMinimalWoT = 999999;                //Для проверки доступности серверов
                for (int i = 0; i < 10; i++)                     //Вычисление сервера с найменьшей задержкой
                {
                    if (WoTRu[i].RoundtripTime < pingMinimalWoT)
                    {
                        pingMinimalWoT = Convert.ToInt32(WoTRu[i].RoundtripTime);       //Запоминание задержки
                        indexOfServerWoT = Convert.ToByte(i);                           //Запоминание сервера
                    }
                }

                //-----------Вывод результатов на экран------------------------
                for (int i = 0; i < 10; i++)
                {
                    labelsWoTPing[i].Text = WoTRu[i].RoundtripTime.ToString() + " ms";
                }
                //--------------------------------------------------------------

                //--------Окрашивание показателей задержки в разные цвета
                for (int i = 0; i < 10; i++)
                {
                    if (WoTRu[i].RoundtripTime <= 50) labelsWoTPing[i].ForeColor = Color.DarkGreen;
                    else if (WoTRu[i].RoundtripTime <= 150) labelsWoTPing[i].ForeColor = Color.Goldenrod;
                    else labelsWoTPing[i].ForeColor = Color.Red;
                }
                //-------------------------------------------------------
                label_WoT_Recommend.Text = "Recommend WoT RU" + Convert.ToString(indexOfServerWoT + 1);         //Вывод рекомендуемого сервера с найменьшей задержкой
                pingWoTThreadState = false;
                label_pingInfo.Visible = false;
                pictureBox2.Image = Properties.Resources.tick;
            }
            catch(System.Net.NetworkInformation.PingException)
            {
                label_pingInfo.Text = "No connection";
                pictureBox2.Image = Properties.Resources.rsz_1no_icon;
                button_RetryPing.Visible = true;
                pingWoTServers_timer.Stop();
                MessageBox.Show("  Unable to connect to servers\nCheck your network connection", "Ping error", MessageBoxButtons.OK, MessageBoxIcon.Error);               //Вывод на экран сообщения об ошибке
            }
            
        }
        //---------------------------------------------------------------------------------------------------------

        private void pingWoTServers_timer_Tick(object sender, EventArgs e)                                  //Функция, что выполняется при каждом шаге таймера
        {
            if(pingWoTThreadState == false) pingTestWoT();                                                  //Функция запуска потока ping
        }

        private void button_RunUpdater_Click(object sender, EventArgs e)                                    //Нажатие на кнопку "Запустить апдейтер"
        {
            
            this.ShowInTaskbar = false;                                                                     //Переносим приложение из панели задач
            notifyIcon1.Visible = true;                                                                     //В системный трей
            this.Visible = false;                                                                           //Убираем главное окно
            Process.Start(settings.Element("settings").Element("worldoftanks").Element("client_path").Value + "\\WoTLauncher.exe");//Запускаем апдейтер
            updateWoTServersStats_timer.Stop();                                                             //Останавливаем обновление информации о серверах
            pingWoTServers_timer.Stop();                                                                    //Останавливаем обновление информации о задержках доступа к серверам
            checkRunningUpdaterWoT_timer.Start();                                                           //Запускаем проверку окончания работы апдейтера
        }

        private void checkRunningUpdaterWoT_timer_Tick(object sender, EventArgs e)                          //Функция таймера проверки окончания работы апдейтера
        {
            Process[] localByName = Process.GetProcessesByName("WoTLauncher");                              //Поиск процесса апдейтера
            if (localByName.Length == 0)                                                                    //Если его нет
            {
                this.ShowInTaskbar = true;                                                                  //Переносим приложение из системного трея
                notifyIcon1.Visible = false;                                                                //В панель задач
                this.Visible = true;                                                                        //Отображаем главное окно
                checkRunningUpdaterWoT_timer.Stop();                                                        //Останавливаем проверку окончания работы апдейтера
                pingWoTServers_timer.Start();                                                               //Запускаем проверку задержки доступа к серверам
                updateWoTServersStats_timer.Start();                                                        //Запускаем проверку онлайна серверов
            }
        }

        private void button_RunClient_Click(object sender, EventArgs e)                                     //Нажатие на кнопку "Запустить игру"
        {
            this.ShowInTaskbar = false;                                                                     //Переносим приложение из панели задач
            notifyIcon1.Visible = true;                                                                     //В системный трей
            this.Visible = false;                                                                           //Убираем главное окно
            Process.Start(settings.Element("settings").Element("worldoftanks").Element("client_path").Value + "\\WorldOfTanks.exe");//Запускаем игру
            updateWoTServersStats_timer.Stop();                                                             //Останавливаем обновление информации о серверах
            pingWoTServers_timer.Stop();                                                                    //Останавливаем обновление информации о задержках доступа к серверам
            checkRunningWoT_timer.Start();                                                                  //Запускаем проверку окончания работы апдейтера
        }

        private void checkRunningWoT_timer_Tick(object sender, EventArgs e)                                 //Функция таймера проверки окончания работы игры
        {
            Process[] localByName = Process.GetProcessesByName("WorldOfTanks");                             //Поиск процесса игры
            if (localByName.Length == 0)                                                                    //Если его нет
            {
                this.ShowInTaskbar = true;                                                                  //Переносим приложение из системного трея
                notifyIcon1.Visible = false;                                                                //В панель задач
                this.Visible = true;                                                                        //Отображаем главное окно
                checkRunningUpdaterWoT_timer.Stop();                                                        //Останавливаем проверку окончания работы апдейтера
                pingWoTServers_timer.Start();                                                               //Запускаем проверку задержки доступа к серверам
                updateWoTServersStats_timer.Start();                                                        //Запускаем проверку онлайна серверов
            }
        }

        private void button_RetryPing_Click(object sender, EventArgs e)
        {
            pingTestWoT();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettings settingsWindow = new FormSettings();                                               //Инициализация объекта окна настроек
            AddOwnedForm(settingsWindow);                                                                   //Присвоение окна настроек главному окну
            settingsWindow.ShowDialog();                                                                    //Отображение окна настроек как дочернее окно
            updateWoTClientVersion();                                                                       //Обновление данных о версии клиента
        }

        private void authorizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.AuthForm autherizationForm = new Forms.AuthForm();
            AddOwnedForm(autherizationForm);
            autherizationForm.ShowDialog();
            checkAuthorization();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebRequest logoutRequest = WebRequest.Create("https://api.worldoftanks.ru/wot/auth/logout/?application_id=146bc6b8d619f5030ed02cdb5ce759b4&access_token=" + settings.Element("settings").Element("wg_open_id").Element("access_token").Value);
            WebResponse logoutResponse = logoutRequest.GetResponse();
            settings.Element("settings").Element("wg_open_id").Element("access_token").Value = "";
            settings.Element("settings").Element("wg_open_id").Element("nickname").Value = "";
            settings.Element("settings").Element("wg_open_id").Element("account_id").Value = "";
            settings.Element("settings").Element("wg_open_id").Element("experies_at").Value = "";
            settings.Element("settings").Element("wg_open_id").Element("authorized").Value = "no";
            settings.Save("settings.xml");
            checkAuthorization();
        }
    }
}
