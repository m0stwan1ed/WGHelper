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

namespace WGHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;    //Отключение отслеживания пересекания потоков
            startUpdating();                                                                                           //Запуск потока, реквест-респонс
        }

        void startUpdating()
        {
            button_Retry.Visible = false;
            updateWoTServersStats_timer.Start();                                     //Запуск таймера
            Thread requestWoTOnline;                                                                                            //Инициализация потока
            requestWoTOnline = new Thread(request);                                                                             //Привязка функции к потоку
            label_updatingInfo.Text = "Updating...";
            label_updatingInfo.Visible = true;                                                                                  //Отображение информатора загрузки
            pictureBox1.Image = Properties.Resources.loading_sh;                                                                //Смена изображения информатора
            requestWoTOnline.IsBackground = true;                                                                               //Поток после полного выполнения самоуничтожается
            requestWoTOnline.Start();
        }

        bool requestWoTThreadState = false;                     //Переменная-маркер для проверки активности потока

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

        public void request()                                                                               //Функция, что выполняет запрос в отдельном потоке
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

                //------------Если сервер в сети информатор станет зелёного цвета. Если не в сети - красным. Вместо онлайна будет написано, что сервер офлайн
                if (serverStatusWoT[0] == true) ru1_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru1_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru1Online.Text = "Offline";
                }
                if (serverStatusWoT[1] == true) ru2_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru2_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru2Online.Text = "Offline";
                }
                if (serverStatusWoT[2] == true) ru3_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru3_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru3Online.Text = "Offline";
                }
                if (serverStatusWoT[3] == true) ru4_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru4_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru4Online.Text = "Offline";
                }
                if (serverStatusWoT[4] == true) ru5_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru5_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru5Online.Text = "Offline";
                }
                if (serverStatusWoT[5] == true) ru6_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru6_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru6Online.Text = "Offline";
                }
                if (serverStatusWoT[6] == true) ru7_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru7_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru7Online.Text = "Offline";
                }
                if (serverStatusWoT[7] == true) ru8_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru8_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru8Online.Text = "Offline";
                }
                if (serverStatusWoT[8] == true) ru9_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru9_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru9Online.Text = "Offline";
                }
                if (serverStatusWoT[9] == true) ru10_status_pictureBox.Image = Properties.Resources.rsz_green_light;
                else
                {
                    ru10_status_pictureBox.Image = Properties.Resources.rsz_red_light;
                    label_ru10Online.Text = "Offline";
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

                MessageBox.Show("  Unable to connect to servers\nCheck your network connection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);               //Вывод на экран сообщения об ошибке

            }

        }

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

        private void button_Retry_Click(object sender, EventArgs e)
        {
            startUpdating();
        }
    }
}
