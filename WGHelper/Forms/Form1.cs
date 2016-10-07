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
        public Form1()                                          //Конструктор формы
        {
            InitializeComponent();                              //Инициализация компонентов формы. Должна быть первой всегда
            //------------------------------------------------------------------------------------------------
            checkAuthorization();
            loadSettings();                                     //Загрузка настроек приложения
            Control.CheckForIllegalCrossThreadCalls = false;    //Отключение отслеживания пересекания потоков
            UpdateWoTOnline();                                  //Запуск потока, реквест-респонс
            pingTestWoT();                                      //Запуск потока проверки доступности серверов
            pingWoTServers_timer.Start();                       //Запуск таймера, который через промежутки времени проводит тест задержек доступа к серверам
            updateWoTServersStats_timer.Start();                //Запуск таймера, который через промежутки времени выполняет запросы об онлайне серверов
            setHints();
        }

        XDocument settings;                                     //Объект для хранения XML-файла

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

        static string WGApiAppID = "146bc6b8d619f5030ed02cdb5ce759b4";
        public static string urlServerOnline = "https://api.worldoftanks.ru/wgn/servers/info/";             //URI запроса
        public static string urlServerOnlineRequest = "application_id=" + WGApiAppID + "&game=wot";                       //Запрос к серверу
        public static string jsonAnswer;                                                                    //Переменная ответа сервера
        public static wotOnlineRootObject wotOnline;                                                        //Объект класса для десериализации ответа сервера

        //----------------------Запуск потока, реквест-респонс---------------------------
        void UpdateWoTOnline()
        {
            //onlineToolStripMenuItem.Enabled = false;                                    //Отключаем кнопку меню для ручной проверки онлайна
            updateWoTServersStats_timer.Start();                                        //Запуск таймера, который через промежутки времени выполняет запросы об онлайне серверов
            button_Retry.Visible = false;                                               //Убираем кнопку "Попробовать ещё раз"
            Thread requestWoTOnline;                                                    //Инициализация потока
            requestWoTOnline = new Thread(request);                                     //Привязка функции к потоку
            label_updatingInfo.Text = "Updating...";
            label_updatingInfo.Visible = true;                                          //Отображение информатора загрузки
            pictureBox1.Image = Properties.Resources.loading_sh;                        //Смена изображения информатора
            requestWoTOnline.IsBackground = true;                                       //Поток после полного выполнения самоуничтожается
            requestWoTOnline.Start();
        }
        //-------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------
        class Meta
        {
            public int count { get; set; }
        }

        class Private
        {
            public int credits { get; set; }
            public int free_xp { get; set; }
            public int gold { get; set; }
            public bool is_premium { get; set; }
        }

        class Player
        {
            public Private @private { get; set; }
        }

        class DataPlayerStats
        {
            public Player player { get; set; }
        }

        class playerStats
        {
            public string status { get; set; }
            public Meta meta { get; set; }
            public DataPlayerStats data { get; set; }
        }
        //-------------------------------------------------------------------------------

        void getAuthPlayerInfo()
        {
            try
            {

                label_nickname.Text = settings.Element("settings").Element("wg_open_id").Element("nickname").Value;
                label_gold.Text = "loading";
                label_silver.Text = "loading";
                label_free_XP.Text = "loading";

                string requestUrl = "https://api.worldoftanks.ru/wot/account/info/";
                string appID = "application_id=" + WGApiAppID;
                string accountID = "account_id=" + settings.Element("settings").Element("wg_open_id").Element("account_id").Value;
                string accessToken = "access_token=" + settings.Element("settings").Element("wg_open_id").Element("access_token").Value;
                string fields = "fields=private.gold%2C+private.credits%2C+private.free_xp%2C+private.is_premium";
                WebRequest requestAuthPlayerInfo = WebRequest.Create(requestUrl + "?" + appID + "&" + accountID + "&" + accessToken + "&" + fields);
                WebResponse responseAuthPlayerInfo = requestAuthPlayerInfo.GetResponse();
                Stream answerStream = responseAuthPlayerInfo.GetResponseStream();
                StreamReader srAnswer = new StreamReader(answerStream);
                string jsonAnswer = srAnswer.ReadToEnd();
                jsonAnswer = jsonAnswer.Replace("\"" + settings.Element("settings").Element("wg_open_id").Element("account_id").Value + "\"", "\"player\"");
                playerStats authPlayerStats = JsonConvert.DeserializeObject<playerStats>(jsonAnswer);

                label_gold.Text = authPlayerStats.data.player.@private.gold.ToString();
                label_silver.Text = authPlayerStats.data.player.@private.credits.ToString();
                label_free_XP.Text = authPlayerStats.data.player.@private.free_xp.ToString();
                if (authPlayerStats.data.player.@private.is_premium == true) pictureBox_Premium.Image = Properties.Resources.premium_icon;
                else pictureBox_Premium.Image = Properties.Resources.standard_icon;
            }
            catch(System.Net.WebException)
            {
                label_nickname.Text = "No connection";
                label_gold.Text = "---";
                label_silver.Text = "---";
                label_free_XP.Text = "---";
            }
        }

        //-------Функция, что проверяет, была ли ранее выполнена авторизация-------------
        void checkAuthorization()
        {
            settings = XDocument.Load("settings.xml");                                  //Загружаем файл настроек
            if (settings.Element("settings").Element("wg_open_id").Element("authorized").Value == "no")//Если авторизация не выполнялась
            {
                authorizationToolStripMenuItem.Enabled = true;                          //Активируем кнопку авторизации
                logoutToolStripMenuItem.Enabled = false;                                
            }
            else                                                                        //Проверяем, истек ли сеанс авторизации
            {
                int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;//Получаем текущее время в виде UNIX
                if (unixTime > Convert.ToInt32( settings.Element("settings").Element("wg_open_id").Element("experies_at").Value))//Если текущее время больше чем время действия
                {
                    WebRequest logoutRequest = WebRequest.Create("https://api.worldoftanks.ru/wot/auth/logout/?application_id=146bc6b8d619f5030ed02cdb5ce759b4&access_token=" + settings.Element("settings").Element("wg_open_id").Element("access_token").Value);
                    WebResponse logoutResponse = logoutRequest.GetResponse();//Запрос на уничтожение токена авторизации
                    //----------------------------------------------------------------------------------------------
                    settings.Element("settings").Element("wg_open_id").Element("access_token").Value = "";
                    settings.Element("settings").Element("wg_open_id").Element("nickname").Value = "";
                    settings.Element("settings").Element("wg_open_id").Element("account_id").Value = "";                //Удаляем информацию о авторизации
                    settings.Element("settings").Element("wg_open_id").Element("experies_at").Value = "";
                    settings.Element("settings").Element("wg_open_id").Element("authorized").Value = "no";
                    //----------------------------------------------------------------------------------------------

                    label_nickname.Text = "Not authorized";
                    label_gold.Text = "---";
                    label_silver.Text = "---";
                    label_free_XP.Text = "---";
                    pictureBox_Premium.Image = null;

                    settings.Save("settings.xml");                                      //Сохраняем файл настроек
                    checkAuthorization();                                               //Проверяем авторизацию
                }
                else
                {
                    authorizationToolStripMenuItem.Enabled = false;                 //Деактивируем кнопку авторизации
                    logoutToolStripMenuItem.Enabled = true;                         //Активируем кнопку деавторизации
                    Thread.Sleep(500);
                    Thread authPlayerInfo;
                    authPlayerInfo = new Thread(getAuthPlayerInfo);
                    authPlayerInfo.IsBackground = true;
                    authPlayerInfo.Start();
                }
            }
        }
        //-----------------------------------------------------------------

        //-------Функция, что выполняет первоначальную загрузку настроек---
        void loadSettings()
        {
            settings = XDocument.Load("settings.xml");                          //Загружаем файл настроек
            updateToolStripMenuItem.Visible = false;                            //Отключаем кнопки ручного обновления
            pingToolStripMenuItem.Enabled = false;
            onlineToolStripMenuItem.Enabled = false;
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

            bool ping, online;                                              //Переменные, в которых хранится настройка автообновлений и автозапросов
            ping = Convert.ToBoolean(settings.Element("settings").Element("autoping").Value);
            online = Convert.ToBoolean(settings.Element("settings").Element("autoupdate_servers_online").Value);
            if ((ping == false) || (online == false))                       //Если хоть один из запросов не автоматический
            {
                updateToolStripMenuItem.Visible = true;                     //Отобразить пункт меню Обновления
                if (ping == false) pingToolStripMenuItem.Enabled = true;    //Если проверка задержки доступа не автоматическая - включить эту кнопку
                if (online == false) onlineToolStripMenuItem.Enabled = true;//Если проверка онлайна сервера не автоматическая - включить эту кнопку
            }
                
        }

        void setHints()
        {
            ToolTip hint = new ToolTip();
            hint.SetToolTip(button_RunClient, "Запустить клиент ");
            hint.SetToolTip(button_RunUpdater, "Запустить апдейтер");
        }

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

        //-------Функция таймера проверки онлайна сервера-------
        private void timer1_Tick(object sender, EventArgs e)                                                                    //Функция таймера
        {
            if (requestWoTThreadState == false)
            {
                
                if (settings.Element("settings").Element("autoupdate_servers_online").Value == "false")
                {
                    onlineToolStripMenuItem.Enabled = true;
                    updateWoTServersStats_timer.Stop();
                }
                else
                {
                    UpdateWoTOnline();
                }
            }
        }
        //------------------------------------------------------

        private void button_Retry_Click(object sender, EventArgs e)                 //Функция повторной попытки запроса к серверам
        {
            UpdateWoTOnline();
            updateWoTServersStats_timer.Start();
        }

        void pingTestWoT()                                                          //Функция запуска потока ping
        {
            pingWoTServers_timer.Start();                                           //Запуск таймера проверки задержки доступа
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
            if (pingWoTThreadState == false)                                    //Если поток не завершен
            {
                if (settings.Element("settings").Element("autoping").Value == "false")//Если автопроверка отключена
                {
                    pingWoTServers_timer.Stop();                                //Остановка таймера
                    pingToolStripMenuItem.Enabled = true;                       //Включение кнопки ручной проверки
                }
                else
                {
                    pingTestWoT();                                                  //Функция запуска потока ping
                }
            }
        }

        private void button_RunUpdater_Click(object sender, EventArgs e)                                    //Нажатие на кнопку "Запустить апдейтер"
        {
            
            this.ShowInTaskbar = false;                                                                     //Переносим приложение из панели задач
            notifyIcon1.Visible = true;                                                                     //В системный трей
            this.Visible = false;                                                                           //Убираем главное окно

            var wotUpdareInfo = new ProcessStartInfo();
            wotUpdareInfo.WorkingDirectory = settings.Element("settings").Element("worldoftanks").Element("client_path").Value;
            wotUpdareInfo.FileName = settings.Element("settings").Element("worldoftanks").Element("client_path").Value + "\\WoTLauncher.exe";
            Process.Start(wotUpdareInfo);

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
            
            var wotGameInfo = new ProcessStartInfo();
            wotGameInfo.WorkingDirectory = settings.Element("settings").Element("worldoftanks").Element("client_path").Value;
            wotGameInfo.FileName = settings.Element("settings").Element("worldoftanks").Element("client_path").Value + "\\WorldOfTanks.exe";
            Process.Start(wotGameInfo);

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

        private void button_RetryPing_Click(object sender, EventArgs e)                                     //Повторная проверка выполнить запрос
        {
            pingTestWoT();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)                                //Выход из приложения
        {
            this.Dispose();
        }

        private void authorizationToolStripMenuItem_Click(object sender, EventArgs e)                       //Кнопка авторизации
        {
            Forms.AuthForm autherizationForm = new Forms.AuthForm();                                        //Инициализация формы авторизации
            AddOwnedForm(autherizationForm);                                                                //Присвоение дочерней формы
            autherizationForm.ShowDialog();                                                                 //Отобразить как диалоговое окно
            checkAuthorization();                                                                           //Проверить авторизацию
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)                              //Кнопка деавторизации
        {
            WebRequest logoutRequest = WebRequest.Create("https://api.worldoftanks.ru/wot/auth/logout/?application_id=146bc6b8d619f5030ed02cdb5ce759b4&access_token=" + settings.Element("settings").Element("wg_open_id").Element("access_token").Value);//Запрос деавторизации
            WebResponse logoutResponse = logoutRequest.GetResponse();//Выполнение запроса деавторизации и уничтожение токена доступа
            //-----Удаление всех данных авторизации-----
            settings.Element("settings").Element("wg_open_id").Element("access_token").Value = "";
            settings.Element("settings").Element("wg_open_id").Element("nickname").Value = "";
            settings.Element("settings").Element("wg_open_id").Element("account_id").Value = "";
            settings.Element("settings").Element("wg_open_id").Element("experies_at").Value = "";
            settings.Element("settings").Element("wg_open_id").Element("authorized").Value = "no";
            //------------------------------------------

            label_nickname.Text = "Not authorized";
            label_gold.Text = "---";
            label_silver.Text = "---";
            label_free_XP.Text = "---";
            pictureBox_Premium.Image = null;

            settings.Save("settings.xml");//Сохранение файла настроек
            checkAuthorization();//Проверка авторизации
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)                           //Кнопка открытия окна настроек
        {
            FormSettings settingsWindow = new FormSettings();                                               //Инициализация объекта окна настроек
            AddOwnedForm(settingsWindow);                                                                   //Присвоение окна настроек главному окну
            settingsWindow.ShowDialog();                                                                    //Отображение окна настроек как дочернее окно
            loadSettings();                                                                                 //Обновление данных о версии клиента
            if ((updateWoTServersStats_timer.Enabled == false)&&(settings.Element("settings").Element("autoupdate_servers_online").Value == "true")) UpdateWoTOnline();//Если автообновление включено, а таймер откючен то включить таймен
            if ((pingWoTServers_timer.Enabled == false)&&(settings.Element("settings").Element("autoping").Value == "true")) pingTestWoT();//Если автопроверка включена, а таймер откючен то включить таймен
        }

        private void onlineToolStripMenuItem_Click(object sender, EventArgs e)                              //Кнопка ручной проверки онлайна серверов
        {
            onlineToolStripMenuItem.Enabled = false;
            UpdateWoTOnline();                                    //Запуск потока, реквест-респонс
            manualUpdateWoTOnlineCooldown_timer.Start();
        }

        private void pingToolStripMenuItem_Click(object sender, EventArgs e)                                //Кнопка автоматической проверки задержки доступа к серверам
        {
            pingToolStripMenuItem.Enabled = false;
            pingTestWoT();                                      //Запуск потока проверки доступности серверов
            manualUpdateWoTPingCooldown_timer.Start();
        }

        private void manualUpdateWoTOnlineCooldown_timer_Tick(object sender, EventArgs e)
        {
            onlineToolStripMenuItem.Enabled = true;
            manualUpdateWoTOnlineCooldown_timer.Stop();
        }

        private void manualUpdateWoTPingCooldown_timer_Tick(object sender, EventArgs e)
        {
            pingToolStripMenuItem.Enabled = true;
            manualUpdateWoTPingCooldown_timer.Stop();
        }
    }
}