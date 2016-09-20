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
        }

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
            WebRequest requestServerOnline = WebRequest.Create(urlServerOnline + "?" + urlServerOnlineRequest);             //Инициализация запроса
            WebResponse responseServerOnline = requestServerOnline.GetResponse();                                           //Выполнение запроса и получение ответа
            Stream answerStream = responseServerOnline.GetResponseStream();                                                 //Конвертирование ответа в Stream
            StreamReader srAnswer = new StreamReader(answerStream);                                                         //Конвертирование Stream в StreamReader
            jsonAnswer = srAnswer.ReadToEnd();                                                                              //Конвертирование StreamReader в String
            wotOnlineRootObject wotOnline = JsonConvert.DeserializeObject<wotOnlineRootObject>(jsonAnswer);                 //Десериализация JSON и запись данных в объект класса

            for (int i = 0; i < wotOnline.data.wot.Count; i++)                                                              //Выведение данных в label по серверам
            {
                switch (wotOnline.data.wot[i].server)
                {
                    case "RU1":
                        {
                            label_ru1Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                    case "RU2":
                        {
                            label_ru2Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                    case "RU3":
                        {
                            label_ru3Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                    case "RU4":
                        {
                            label_ru4Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                    case "RU5":
                        {
                            label_ru5Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                    case "RU6":
                        {
                            label_ru6Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                    case "RU7":
                        {
                            label_ru7Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                    case "RU8":
                        {
                            label_ru8Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                    case "RU9":
                        {
                            label_ru9Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                    case "RU10":
                        {
                            label_ru10Online.Text = wotOnline.data.wot[i].players_online.ToString();
                            break;
                        }
                }
            }

            int wotTotalOnline = 0;                                                                     //Подсчет общего онлайна на серверах WoT
            for (int i = 0; i < wotOnline.data.wot.Count; i++)                                          
            {
                wotTotalOnline += Convert.ToInt32(wotOnline.data.wot[i].players_online);
            }
            label_totalOnline.BeginInvoke((MethodInvoker)(delegate { label_totalOnline.Text = wotTotalOnline.ToString(); }));   //Вывод общего онлайна в label
            label_updatingInfo.Visible = false;                                                                                 //Информатор загрузки уходит в инвиз
            pictureBox1.Image = Properties.Resources.tick;                                                                      //Смена изображения-информатора
        }

        private void button_GetWoTPlayersOnline_Click(object sender, EventArgs e)                                               //Функция кнопки
        {
            button_GetWoTPlayersOnline.Enabled = false;                                                                         //Временное отключение кнопки
            timer1.Start();                                                                                                     //Запуск таймера на 5 секунд
            Thread requestWoTOnline;                                                                                            //Инициализация потока
            requestWoTOnline = new Thread(request);                                                                             //Привязка функции к потоку
            label_updatingInfo.Visible = true;                                                                                  //Отображение информатора загрузки
            pictureBox1.Image = Properties.Resources.loading_sh;                                                                //Смена изображения информатора
            requestWoTOnline.IsBackground = true;                                                                               //Поток после полного выполнения самоуничтожается
            requestWoTOnline.Start();                                                                                           //Запуск потока, реквест-респонс
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)                                                                    //Функция таймера
        {
            button_GetWoTPlayersOnline.Enabled = true;                                                                          //Включение кнопки
            timer1.Stop();                                                                                                      //Остановка таймера
        }
    }
}
