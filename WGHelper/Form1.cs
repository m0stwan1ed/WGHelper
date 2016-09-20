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
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WGHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Wot
        {
            public int players_online { get; set; }
            public string server { get; set; }
        }

        public class Data
        {
            public List<Wot> wot { get; set; }
        }

        public class wotOnlineRootObject
        {
            public string status { get; set; }
            public Data data { get; set; }
        }

        public static string urlServerOnline = "https://api.worldoftanks.ru/wgn/servers/info/";
        public static string urlServerOnlineRequest = "application_id=demo&game=wot";
        public static string jsonAnswer;
        public static wotOnlineRootObject wotOnline;

        private void button_GetWoTPlayersOnline_Click(object sender, EventArgs e)
        {
            WebRequest requestServerOnline = WebRequest.Create(urlServerOnline + "?" + urlServerOnlineRequest);
            WebResponse responseServerOnline =  requestServerOnline.GetResponse();
            Stream answerStream = responseServerOnline.GetResponseStream();
            StreamReader srAnswer = new StreamReader(answerStream);
            jsonAnswer = srAnswer.ReadToEnd();
            wotOnlineRootObject wotOnline = JsonConvert.DeserializeObject<wotOnlineRootObject>(jsonAnswer);

            for (int i=0; i<wotOnline.data.wot.Count; i++)
            {
                switch(wotOnline.data.wot[i].server)
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

            int wotTotalOnline = 0;
            for (int i = 0; i < wotOnline.data.wot.Count; i++)
            {
                wotTotalOnline += Convert.ToInt32(wotOnline.data.wot[i].players_online);
            }
            label_totalOnline.Text = wotTotalOnline.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
