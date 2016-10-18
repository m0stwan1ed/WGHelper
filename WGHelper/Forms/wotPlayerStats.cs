using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;                                         //Подключение многопоточности
using Newtonsoft.Json;                                          //Библиотека для работы с JSON
using System.Xml.Linq;                                          //Библиотека XML
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WGHelper.Forms
{
    public partial class wotPlayerStats : Form
    {
        public wotPlayerStats()
        {
            InitializeComponent();
            settings = XDocument.Load("settings.xml");
            Thread getPlayersStatsThread;
            getPlayersStatsThread = new Thread(getPlayersStats);
            getPlayersStatsThread.IsBackground = true;
            getPlayersStatsThread.Start();
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        XDocument settings;
        //------------------------------------------------
        public class Meta
        {
            public int count { get; set; }
        }

        public class Datum
        {
            public int tank_id { get; set; }
        }

        public class TanksListRootObject
        {
            public string status { get; set; }
            public Meta meta { get; set; }
            public List<Datum> data { get; set; }
        }
        //-----------------------------------------------

        public class Meta2
        {
            public int count { get; set; }
        }

        public class Restrictions
        {
            public object chat_ban_time { get; set; }
            public object clan_time { get; set; }
        }

        public class Private
        {
            public Restrictions restrictions { get; set; }
            public int gold { get; set; }
            public int free_xp { get; set; }
            public bool is_bound_to_phone { get; set; }
            public bool is_premium { get; set; }
            public int credits { get; set; }
            public int premium_expires_at { get; set; }
            public List<object> friends { get; set; }
            public int battle_life_time { get; set; }
        }

        public class Clan
        {
            public int spotted { get; set; }
            public double avg_damage_assisted_track { get; set; }
            public double avg_damage_blocked { get; set; }
            public int direct_hits_received { get; set; }
            public int explosion_hits { get; set; }
            public int piercings_received { get; set; }
            public int piercings { get; set; }
            public int xp { get; set; }
            public int survived_battles { get; set; }
            public int dropped_capture_points { get; set; }
            public int hits_percents { get; set; }
            public int draws { get; set; }
            public int battles { get; set; }
            public int damage_received { get; set; }
            public double avg_damage_assisted { get; set; }
            public int frags { get; set; }
            public double avg_damage_assisted_radio { get; set; }
            public int capture_points { get; set; }
            public int hits { get; set; }
            public int battle_avg_xp { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int damage_dealt { get; set; }
            public int no_damage_direct_hits_received { get; set; }
            public int shots { get; set; }
            public int explosion_hits_received { get; set; }
            public double tanking_factor { get; set; }
        }

        public class All
        {
            public int spotted { get; set; }
            public double avg_damage_assisted_track { get; set; }
            public int max_xp { get; set; }
            public double avg_damage_blocked { get; set; }
            public int direct_hits_received { get; set; }
            public int explosion_hits { get; set; }
            public int piercings_received { get; set; }
            public int piercings { get; set; }
            public int max_damage_tank_id { get; set; }
            public int xp { get; set; }
            public int survived_battles { get; set; }
            public int dropped_capture_points { get; set; }
            public int hits_percents { get; set; }
            public int draws { get; set; }
            public int max_xp_tank_id { get; set; }
            public int battles { get; set; }
            public int damage_received { get; set; }
            public double avg_damage_assisted { get; set; }
            public int max_frags_tank_id { get; set; }
            public int frags { get; set; }
            public double avg_damage_assisted_radio { get; set; }
            public int capture_points { get; set; }
            public int max_damage { get; set; }
            public int hits { get; set; }
            public int battle_avg_xp { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int damage_dealt { get; set; }
            public int no_damage_direct_hits_received { get; set; }
            public int max_frags { get; set; }
            public int shots { get; set; }
            public int explosion_hits_received { get; set; }
            public double tanking_factor { get; set; }
        }

        public class RegularTeam
        {
            public int spotted { get; set; }
            public double avg_damage_assisted_track { get; set; }
            public int max_xp { get; set; }
            public double avg_damage_blocked { get; set; }
            public int direct_hits_received { get; set; }
            public int explosion_hits { get; set; }
            public int piercings_received { get; set; }
            public int piercings { get; set; }
            public object max_damage_tank_id { get; set; }
            public int xp { get; set; }
            public int survived_battles { get; set; }
            public int dropped_capture_points { get; set; }
            public int hits_percents { get; set; }
            public int draws { get; set; }
            public object max_xp_tank_id { get; set; }
            public int battles { get; set; }
            public int damage_received { get; set; }
            public double avg_damage_assisted { get; set; }
            public object max_frags_tank_id { get; set; }
            public int frags { get; set; }
            public double avg_damage_assisted_radio { get; set; }
            public int capture_points { get; set; }
            public int max_damage { get; set; }
            public int hits { get; set; }
            public int battle_avg_xp { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int damage_dealt { get; set; }
            public int no_damage_direct_hits_received { get; set; }
            public int max_frags { get; set; }
            public int shots { get; set; }
            public int explosion_hits_received { get; set; }
            public double tanking_factor { get; set; }
        }

        public class Company
        {
            public int spotted { get; set; }
            public double avg_damage_assisted_track { get; set; }
            public double avg_damage_blocked { get; set; }
            public int direct_hits_received { get; set; }
            public int explosion_hits { get; set; }
            public int piercings_received { get; set; }
            public int piercings { get; set; }
            public int xp { get; set; }
            public int survived_battles { get; set; }
            public int dropped_capture_points { get; set; }
            public int hits_percents { get; set; }
            public int draws { get; set; }
            public int battles { get; set; }
            public int damage_received { get; set; }
            public double avg_damage_assisted { get; set; }
            public int frags { get; set; }
            public double avg_damage_assisted_radio { get; set; }
            public int capture_points { get; set; }
            public int hits { get; set; }
            public int battle_avg_xp { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int damage_dealt { get; set; }
            public int no_damage_direct_hits_received { get; set; }
            public int shots { get; set; }
            public int explosion_hits_received { get; set; }
            public double tanking_factor { get; set; }
        }

        public class StrongholdSkirmish
        {
            public int spotted { get; set; }
            public int max_frags_tank_id { get; set; }
            public int max_xp { get; set; }
            public int direct_hits_received { get; set; }
            public int explosion_hits { get; set; }
            public int piercings_received { get; set; }
            public int piercings { get; set; }
            public int xp { get; set; }
            public int survived_battles { get; set; }
            public int dropped_capture_points { get; set; }
            public int hits_percents { get; set; }
            public int draws { get; set; }
            public int max_xp_tank_id { get; set; }
            public int battles { get; set; }
            public int damage_received { get; set; }
            public int frags { get; set; }
            public int capture_points { get; set; }
            public int max_damage_tank_id { get; set; }
            public int max_damage { get; set; }
            public int hits { get; set; }
            public int battle_avg_xp { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int damage_dealt { get; set; }
            public int no_damage_direct_hits_received { get; set; }
            public int max_frags { get; set; }
            public int shots { get; set; }
            public int explosion_hits_received { get; set; }
            public double tanking_factor { get; set; }
        }

        public class StrongholdDefense
        {
            public int spotted { get; set; }
            public object max_frags_tank_id { get; set; }
            public int max_xp { get; set; }
            public int direct_hits_received { get; set; }
            public int explosion_hits { get; set; }
            public int piercings_received { get; set; }
            public int piercings { get; set; }
            public int xp { get; set; }
            public int survived_battles { get; set; }
            public int dropped_capture_points { get; set; }
            public int hits_percents { get; set; }
            public int draws { get; set; }
            public object max_xp_tank_id { get; set; }
            public int battles { get; set; }
            public int damage_received { get; set; }
            public int frags { get; set; }
            public int capture_points { get; set; }
            public object max_damage_tank_id { get; set; }
            public int max_damage { get; set; }
            public int hits { get; set; }
            public int battle_avg_xp { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int damage_dealt { get; set; }
            public int no_damage_direct_hits_received { get; set; }
            public int max_frags { get; set; }
            public int shots { get; set; }
            public int explosion_hits_received { get; set; }
            public double tanking_factor { get; set; }
        }

        public class Historical
        {
            public int spotted { get; set; }
            public double avg_damage_assisted_track { get; set; }
            public int max_xp { get; set; }
            public double avg_damage_blocked { get; set; }
            public int direct_hits_received { get; set; }
            public int explosion_hits { get; set; }
            public int piercings_received { get; set; }
            public int piercings { get; set; }
            public object max_damage_tank_id { get; set; }
            public int xp { get; set; }
            public int survived_battles { get; set; }
            public int dropped_capture_points { get; set; }
            public int hits_percents { get; set; }
            public int draws { get; set; }
            public object max_xp_tank_id { get; set; }
            public int battles { get; set; }
            public int damage_received { get; set; }
            public double avg_damage_assisted { get; set; }
            public object max_frags_tank_id { get; set; }
            public int frags { get; set; }
            public double avg_damage_assisted_radio { get; set; }
            public int capture_points { get; set; }
            public int max_damage { get; set; }
            public int hits { get; set; }
            public int battle_avg_xp { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int damage_dealt { get; set; }
            public int no_damage_direct_hits_received { get; set; }
            public int max_frags { get; set; }
            public int shots { get; set; }
            public int explosion_hits_received { get; set; }
            public double tanking_factor { get; set; }
        }

        public class Team
        {
            public int spotted { get; set; }
            public double avg_damage_assisted_track { get; set; }
            public int max_xp { get; set; }
            public double avg_damage_blocked { get; set; }
            public int direct_hits_received { get; set; }
            public int explosion_hits { get; set; }
            public int piercings_received { get; set; }
            public int piercings { get; set; }
            public int max_damage_tank_id { get; set; }
            public int xp { get; set; }
            public int survived_battles { get; set; }
            public int dropped_capture_points { get; set; }
            public int hits_percents { get; set; }
            public int draws { get; set; }
            public int max_xp_tank_id { get; set; }
            public int battles { get; set; }
            public int damage_received { get; set; }
            public double avg_damage_assisted { get; set; }
            public int max_frags_tank_id { get; set; }
            public int frags { get; set; }
            public double avg_damage_assisted_radio { get; set; }
            public int capture_points { get; set; }
            public int max_damage { get; set; }
            public int hits { get; set; }
            public int battle_avg_xp { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int damage_dealt { get; set; }
            public int no_damage_direct_hits_received { get; set; }
            public int max_frags { get; set; }
            public int shots { get; set; }
            public int explosion_hits_received { get; set; }
            public double tanking_factor { get; set; }
        }

        public class Frag
        {
            public int tank_id { get; set; }
            public int count { get; set; }
        }

        public class Statistics
        {
            public Clan clan { get; set; }
            public All all { get; set; }
            public RegularTeam regular_team { get; set; }
            public int trees_cut { get; set; }
            public Company company { get; set; }
            public StrongholdSkirmish stronghold_skirmish { get; set; }
            public StrongholdDefense stronghold_defense { get; set; }
            public Historical historical { get; set; }
            public Team team { get; set; }
            public List<Frag> frags { get; set; }
        }

        public class Player
        {
            public string client_language { get; set; }
            public int last_battle_time { get; set; }
            public int account_id { get; set; }
            public int created_at { get; set; }
            public int updated_at { get; set; }
            public Private @private { get; set; }
            public object ban_time { get; set; }
            public int global_rating { get; set; }
            public int clan_id { get; set; }
            public Statistics statistics { get; set; }
            public string nickname { get; set; }
            public object ban_info { get; set; }
            public int logout_at { get; set; }
        }

        public class Data
        {
            public Player player { get; set; }
        }

        public class PlayerStatsRootObject
        {
            public string status { get; set; }
            public Meta2 meta { get; set; }
            public Data data { get; set; }
        }

        string urlRequestPlayersStats = "https://api.worldoftanks.ru/wot/account/info/?";
        string applicationID = "146bc6b8d619f5030ed02cdb5ce759b4";

        void getPlayersStats()
        {

            WebRequest requestTanksList = WebRequest.Create("https://api.worldoftanks.ru/wot/encyclopedia/vehicles/?application_id=146bc6b8d619f5030ed02cdb5ce759b4&fields=tank_id");
            WebResponse responseTanksList = requestTanksList.GetResponse();
            Stream answerStream = responseTanksList.GetResponseStream();
            StreamReader srAnswer = new StreamReader(answerStream);
            string strTanksList = srAnswer.ReadToEnd();

            strTanksList = strTanksList.Replace("\"data\":{", "\"data\":[");
            strTanksList = strTanksList.Replace("}}}", "}]}");
            strTanksList = Regex.Replace(strTanksList, "\"[0-9]*\":", "");

            TanksListRootObject tanksList = JsonConvert.DeserializeObject<TanksListRootObject>(strTanksList);
            
            settings = XDocument.Load("settings.xml");

            string accountID = settings.Element("settings").Element("wg_open_id").Element("account_id").Value;
            string accessToken = settings.Element("settings").Element("wg_open_id").Element("access_token").Value;

            WebRequest requestPlayersStats = WebRequest.Create(urlRequestPlayersStats + "application_id=" + applicationID + "&account_id=" + accountID + "&access_token=" + accessToken);
            WebResponse responsePlayersStats = requestPlayersStats.GetResponse();
            answerStream = responsePlayersStats.GetResponseStream();
            srAnswer = new StreamReader(answerStream);
            string answer = srAnswer.ReadToEnd();

            //-------------------------------------------------

            answer = answer.Replace("{\""+accountID, "{\"player");
            for(int i = 0; i<tanksList.data.Count; i++)
            {
                string destroyed = "";
                destroyed = getBetween(answer, "\"" + tanksList.data[i].tank_id.ToString() + "\":", ",");
                if (destroyed!="")
                {
                    answer = answer.Replace("\"" + tanksList.data[i].tank_id.ToString() + "\":"+destroyed, "{\"tank_id\":" + tanksList.data[i].tank_id.ToString() + ", \"count\":" + destroyed  + "}");
                }
            }

            answer = answer.Replace("\"frags\":{", "\"frags\":[");
            answer = answer.Replace("}}},", "}]},");

            //-------------------------------------------------

            PlayerStatsRootObject playerStats = JsonConvert.DeserializeObject<PlayerStatsRootObject>(answer);

            labelNickname.Text = playerStats.data.player.nickname;
            labelClientLanguage.Text = playerStats.data.player.client_language;
            labelCreatedAt.Text = playerStats.data.player.created_at.ToString();
            labelGlobalRating.Text = playerStats.data.player.global_rating.ToString();
            labelLogoutAt.Text = playerStats.data.player.logout_at.ToString();
            labelUpdatedAt.Text = playerStats.data.player.updated_at.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
