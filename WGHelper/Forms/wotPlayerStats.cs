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

        public class Fallout
        {
            public int spotted { get; set; }
            public double avg_damage_assisted_track { get; set; }
            public int max_xp { get; set; }
            public double avg_damage_blocked { get; set; }
            public int direct_hits_received { get; set; }
            public int explosion_hits { get; set; }
            public int piercings_received { get; set; }
            public int flag_capture_solo { get; set; }
            public int piercings { get; set; }
            public object max_damage_tank_id { get; set; }
            public int xp { get; set; }
            public int survived_battles { get; set; }
            public int dropped_capture_points { get; set; }
            public int max_frags_with_avatar { get; set; }
            public int hits_percents { get; set; }
            public int draws { get; set; }
            public int death_count { get; set; }
            public object max_xp_tank_id { get; set; }
            public int battles { get; set; }
            public int damage_received { get; set; }
            public double avg_damage_assisted { get; set; }
            public object max_frags_tank_id { get; set; }
            public int frags { get; set; }
            public double avg_damage_assisted_radio { get; set; }
            public int capture_points { get; set; }
            public int max_win_points { get; set; }
            public int avatar_frags { get; set; }
            public int max_damage { get; set; }
            public int resource_absorbed { get; set; }
            public int hits { get; set; }
            public int battle_avg_xp { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int damage_dealt { get; set; }
            public int avatar_damage_dealt { get; set; }
            public int win_points { get; set; }
            public int no_damage_direct_hits_received { get; set; }
            public int max_frags { get; set; }
            public int shots { get; set; }
            public int explosion_hits_received { get; set; }
            public int flag_capture { get; set; }
            public double tanking_factor { get; set; }
            public int max_damage_with_avatar { get; set; }
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

        public class GlobalmapChampion
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

        public class GlobalmapMiddle
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

        public class GlobalmapAbsolute
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

        public class Statistics
        {
            public Clan clan { get; set; }
            public All all { get; set; }
            public RegularTeam regular_team { get; set; }
            public Fallout fallout { get; set; }
            public int trees_cut { get; set; }
            public Company company { get; set; }
            public GlobalmapChampion globalmap_champion { get; set; }
            public StrongholdSkirmish stronghold_skirmish { get; set; }
            public GlobalmapMiddle globalmap_middle { get; set; }
            public StrongholdDefense stronghold_defense { get; set; }
            public Historical historical { get; set; }
            public Team team { get; set; }
            public List<Frag> frags { get; set; }
            public GlobalmapAbsolute globalmap_absolute { get; set; }
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
            string parameters = "&extra=statistics.globalmap_absolute%2Cstatistics.globalmap_champion%2Cstatistics.globalmap_middle%2Cstatistics.fallout";

            WebRequest requestPlayersStats = WebRequest.Create(urlRequestPlayersStats + "application_id=" + applicationID + "&account_id=" + accountID + "&access_token=" + accessToken + parameters);
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
            answer = answer.Replace("}},\"g", "}],\"g");

            //-------------------------------------------------
            
            PlayerStatsRootObject playerStats = JsonConvert.DeserializeObject<PlayerStatsRootObject>(answer);

            labelNickname.Text = "Nickname: "+playerStats.data.player.nickname;
            labelClientLanguage.Text = "Client language: " + playerStats.data.player.client_language;
            labelCreatedAt.Text = "Account created: " + playerStats.data.player.created_at.ToString();
            labelGlobalRating.Text = "Global rating: " + playerStats.data.player.global_rating.ToString();
            labelLogoutAt.Text = "Logout at: " + playerStats.data.player.logout_at.ToString();
            labelUpdatedAt.Text = "Last update: " + playerStats.data.player.updated_at.ToString();
            //**************************************************
            labelBattleLifeTime.Text = "Battle life time: " + playerStats.data.player.@private.battle_life_time.ToString();
            labelCredits.Text = "Credits: " + playerStats.data.player.@private.credits.ToString();
            labelFreeXP.Text = "Free XP:" + playerStats.data.player.@private.free_xp.ToString();
            labelGold.Text = "Gold: " + playerStats.data.player.@private.gold.ToString();
            labelIsBoundToPhone.Text = "Bounded to phone: " + playerStats.data.player.@private.is_bound_to_phone.ToString();
            labelIsPremium.Text = "Premium: " + playerStats.data.player.@private.is_premium.ToString();
            labelPremiumExperiesAt.Text = "Experies at: " + playerStats.data.player.@private.premium_expires_at.ToString();
            //**************************************************
            //labelRestrictionsChatBanTime.Text = playerStats.data.player.@private.restrictions.chat_ban_time.ToString();
            //labelRestrictionsClanTime.Text = playerStats.data.player.@private.restrictions.clan_time.ToString();
            labelStatisticsFrags.Text = "Frags: " + playerStats.data.player.statistics.frags.Count.ToString();
            labelStatisticsTreesCut.Text = "Trees cut: " + playerStats.data.player.statistics.trees_cut.ToString();
            //**************************************************
            labelStatisticsAllAvgDamageAssisted.Text = "Average damage assisted: " + playerStats.data.player.statistics.all.avg_damage_assisted.ToString();
            labelStatisticsAllAvgDamageAssistedRadio.Text = "Average damage assisted radio:" + playerStats.data.player.statistics.all.avg_damage_assisted_radio.ToString();
            labelStatisticsAllAvgDamageAssistedTrack.Text = "Average damage assisted track:" + playerStats.data.player.statistics.all.avg_damage_assisted_track.ToString();
            labelStatisticsAllAvgDamageBlocked.Text = "Average damage blocked: " + playerStats.data.player.statistics.all.avg_damage_blocked.ToString();
            labelStatisticsAllBattleAvgXp.Text = "Average battle XP: " + playerStats.data.player.statistics.all.battle_avg_xp.ToString();
            labelStatisticsAllBattles.Text = "Battles: " + playerStats.data.player.statistics.all.battles.ToString();
            labelStatisticsAllCapturePoints.Text = "Capture points: " + playerStats.data.player.statistics.all.capture_points.ToString();
            labelStatisticsAllDamageDealt.Text = "Damage dealt: " + playerStats.data.player.statistics.all.damage_dealt.ToString();
            labelStatisticsAllDamageReceived.Text = "Damage received: " + playerStats.data.player.statistics.all.damage_received.ToString();
            labelStatisticsAllDirectHitsReceived.Text = "Direct hits received: " + playerStats.data.player.statistics.all.direct_hits_received.ToString();
            labelStatisticsAllDraws.Text = "Draws: " + playerStats.data.player.statistics.all.draws.ToString();
            labelStatisticsAllDroppedCapturePoints.Text = "Dropped capture points: " + playerStats.data.player.statistics.all.dropped_capture_points.ToString();
            labelStatisticsAllExplosionHits.Text = "Explosion hits" + playerStats.data.player.statistics.all.explosion_hits.ToString();
            labelStatisticsAllExplosionHitsReceived.Text = "Explosion hits received: " + playerStats.data.player.statistics.all.explosion_hits_received.ToString();
            labelStatisticsAllFrags.Text = "Frags: " + playerStats.data.player.statistics.all.frags.ToString();
            labelStatisticsAllHits.Text = "Hits: " + playerStats.data.player.statistics.all.hits.ToString();
            labelStatisticsAllHitsPercents.Text = "Hits percents: " + playerStats.data.player.statistics.all.hits_percents.ToString();
            labelStatisticsAllLosses.Text = "Losses: " + playerStats.data.player.statistics.all.losses.ToString();
            labelStatisticsAllMaxDamage.Text = "Max damage: " + playerStats.data.player.statistics.all.max_damage.ToString();
            labelStatisticsAllMaxDamageTankId.Text = "Max damage tank ID: " + playerStats.data.player.statistics.all.max_damage_tank_id.ToString();
            labelStatisticsAllMaxFrags.Text = "Max frags: " + playerStats.data.player.statistics.all.max_frags.ToString();
            labelStatisticsAllMaxFragsTankId.Text = "Max frags tank ID: " + playerStats.data.player.statistics.all.max_frags_tank_id.ToString();
            labelStatisticsAllMaxXp.Text = "Max XP: " + playerStats.data.player.statistics.all.max_xp.ToString();
            labelStatisticsAllMaxXpTankId.Text = "Max XP tank ID: " + playerStats.data.player.statistics.all.max_xp_tank_id.ToString();
            labelStatisticsAllNoDamageDirectHitsReceived.Text = "No damage direct hits received: " + playerStats.data.player.statistics.all.no_damage_direct_hits_received.ToString();
            labelStatisticsAllPiercing.Text = "Piercing: " + playerStats.data.player.statistics.all.piercings.ToString();
            labelStatisticsAllPiercingReceived.Text = "Piercing received: " + playerStats.data.player.statistics.all.piercings_received.ToString();
            labelStatisticsAllShots.Text = "Shots: " + playerStats.data.player.statistics.all.shots.ToString();
            labelStatisticsAllSpotted.Text = "Spotted: " + playerStats.data.player.statistics.all.spotted.ToString();
            labelStatisticsAllSurvivedBattles.Text = "Syrvived battles: " + playerStats.data.player.statistics.all.survived_battles.ToString();
            labelStatisticsAllTankingFactor.Text = "Tanking factor: " + playerStats.data.player.statistics.all.tanking_factor.ToString();
            labelStatisticsAllWins.Text = "Wins: " + playerStats.data.player.statistics.all.wins.ToString();
            labelStatisticsAllXp.Text = "XP: " + playerStats.data.player.statistics.all.xp.ToString();
            //*****************************************
            labelStatisticsClanAvgDamageAssisted.Text = "Average damage assisted: " + playerStats.data.player.statistics.clan.avg_damage_assisted.ToString();
            labelStatisticsClanAvgDamageAssistedRadio.Text = "Average damage assisted radio:" + playerStats.data.player.statistics.clan.avg_damage_assisted_radio.ToString();
            labelStatisticsClanAvgDamageAssistedTrack.Text = "Average damage assisted track:" + playerStats.data.player.statistics.clan.avg_damage_assisted_track.ToString();
            labelStatisticsClanAvgDamageBlocked.Text = "Average damage blocked: " + playerStats.data.player.statistics.clan.avg_damage_blocked.ToString();
            labelStatisticsClanBattleAvgXp.Text = "Average battle XP: " + playerStats.data.player.statistics.clan.battle_avg_xp.ToString();
            labelStatisticsClanBattles.Text = "Battles: " + playerStats.data.player.statistics.clan.battles.ToString();
            labelStatisticsClanCapturePoints.Text = "Capture points: " + playerStats.data.player.statistics.clan.capture_points.ToString();
            labelStatisticsClanDamageDealt.Text = "Damage dealt: " + playerStats.data.player.statistics.clan.damage_dealt.ToString();
            labelStatisticsClanDamageReceived.Text = "Damage received: " + playerStats.data.player.statistics.clan.damage_received.ToString();
            labelStatisticsClanDirectHitsReceived.Text = "Direct hits received: " + playerStats.data.player.statistics.clan.direct_hits_received.ToString();
            labelStatisticsClanDraws.Text = "Draws: " + playerStats.data.player.statistics.clan.draws.ToString();
            labelStatisticsClanDroppedCapturePoints.Text = "Dropped capture points: " + playerStats.data.player.statistics.clan.capture_points.ToString();
            labelStatisticsClanExplosionHits.Text = "Explosion hits" + playerStats.data.player.statistics.clan.explosion_hits.ToString();
            labelStatisticsClanExplosionHitsReceived.Text = "Explosion hits received: " + playerStats.data.player.statistics.clan.explosion_hits_received.ToString();
            labelStatisticsClanFrags.Text = "Frags: " + playerStats.data.player.statistics.clan.frags.ToString();
            labelStatisticsClanHits.Text = "Hits: " + playerStats.data.player.statistics.clan.hits.ToString();
            labelStatisticsClanHitsPercentage.Text = "Hits percents: " + playerStats.data.player.statistics.clan.hits_percents.ToString();
            labelStatisticsClanLosses.Text = "Losses: " + playerStats.data.player.statistics.clan.losses.ToString();
            labelStatisticsClanNoDamageDirectHitsReceived.Text = "No damage direct hits received: " + playerStats.data.player.statistics.clan.no_damage_direct_hits_received.ToString();
            labelStatisticsClanPiercings.Text = "Piercing: " + playerStats.data.player.statistics.clan.piercings.ToString();
            labelStatisticsClanPiercingsReceived.Text = "Piercing received: " + playerStats.data.player.statistics.clan.piercings_received.ToString();
            labelStatisticsClanShots.Text = "Shots: " + playerStats.data.player.statistics.clan.shots.ToString();
            labelStatisticsClanSpotted.Text = "Spotted: " + playerStats.data.player.statistics.clan.spotted.ToString();
            labelStatisticsClanSurvivedBattles.Text = "Syrvived battles: " + playerStats.data.player.statistics.clan.survived_battles.ToString();
            labelStatisticsClanTankingFactor.Text = "Tanking factor: " + playerStats.data.player.statistics.clan.tanking_factor.ToString();
            labelStatisticsClanWins.Text = "Wins: " + playerStats.data.player.statistics.clan.wins.ToString();
            labelStatisticsClanXp.Text = "XP: " + playerStats.data.player.statistics.clan.xp.ToString();
            //****************************************
            labelStatisticsCompanyAvgDamageAssisted.Text = "Average damage assisted: " + playerStats.data.player.statistics.company.avg_damage_assisted.ToString();
            labelStatisticsCompanyAvgDamageAssistedRadio.Text = "Average damage assisted radio:" + playerStats.data.player.statistics.company.avg_damage_assisted_radio.ToString();
            labelStatisticsCompanyAvgDamageAssistedTrack.Text = "Average damage assisted track:" + playerStats.data.player.statistics.company.avg_damage_assisted_track.ToString();
            labelStatisticsCompanyAvgDamageBlocked.Text = "Average damage blocked: " + playerStats.data.player.statistics.company.avg_damage_blocked.ToString();
            labelStatisticsCompanyBattleAvgXp.Text = "Average battle XP: " + playerStats.data.player.statistics.company.battle_avg_xp.ToString();
            labelStatisticsCompanyBattles.Text = "Battles: " + playerStats.data.player.statistics.company.battles.ToString();
            labelStatisticsCompanyCapturePoints.Text = "Capture points: " + playerStats.data.player.statistics.company.capture_points.ToString();
            labelStatisticsCompanyDamageDealt.Text = "Damage dealt: " + playerStats.data.player.statistics.company.damage_dealt.ToString();
            labelStatisticsCompanyDamageReceived.Text = "Damage received: " + playerStats.data.player.statistics.company.damage_received.ToString();
            labelStatisticsCompanyDirectHitsReceived.Text = "Direct hits received: " + playerStats.data.player.statistics.company.direct_hits_received.ToString();
            labelStatisticsCompanyDraws.Text = "Draws: " + playerStats.data.player.statistics.company.draws.ToString();
            labelStatisticsCompanyDroppedCapturePoints.Text = "Dropped capture points: " + playerStats.data.player.statistics.company.dropped_capture_points.ToString();
            labelStatisticsCompanyExplosionHits.Text = "Explosion hits" + playerStats.data.player.statistics.company.explosion_hits.ToString();
            labelStatisticsCompanyExplosionHitsReceived.Text = "Explosion hits received: " + playerStats.data.player.statistics.company.explosion_hits_received.ToString();
            labelStatisticsCompanyFrags.Text = "Frags: " + playerStats.data.player.statistics.company.frags.ToString();
            labelStatisticsCompanyHits.Text = "Hits: " + playerStats.data.player.statistics.company.hits.ToString();
            labelStatisticsCompanyHitsPercents.Text = "Hits percents: " + playerStats.data.player.statistics.company.hits_percents.ToString();
            labelStatisticsCompanyLosses.Text = "Losses: " + playerStats.data.player.statistics.company.losses.ToString();
            labelStatisticsCompanyNoDamageDirectReceived.Text = "No damage direct hits received: " + playerStats.data.player.statistics.company.no_damage_direct_hits_received.ToString();
            labelStatisticsCompanyPiercing.Text = "Piercing: " + playerStats.data.player.statistics.company.piercings.ToString();
            labelStatisticsCompanyPiercingsReceived.Text = "Piercing received: " + playerStats.data.player.statistics.company.piercings_received.ToString();
            labelStatisticsCompanyShots.Text = "Shots: " + playerStats.data.player.statistics.company.shots.ToString();
            labelStatisticsCompanySpotted.Text = "Spotted: " + playerStats.data.player.statistics.company.spotted.ToString();
            labelStatisticsCompanySurvivedBattles.Text = "Syrvived battles: " + playerStats.data.player.statistics.company.survived_battles.ToString();
            labelStatisticsCompanyTankingFactor.Text = "Tanking factor: " + playerStats.data.player.statistics.company.tanking_factor.ToString();
            labelStatisticsCompanyWins.Text = "Wins: " + playerStats.data.player.statistics.company.wins.ToString();
            labelStatisticsCompanyXp.Text = "XP: " + playerStats.data.player.statistics.company.xp.ToString();
            //****************************************
            labelStatisticsFalloutAvatarDamageDealt.Text = "Avatar damage dealt: " + playerStats.data.player.statistics.fallout.avatar_damage_dealt.ToString();
            labelStatisticsFalloutAvatarFrags.Text = "Avatar frags: " + playerStats.data.player.statistics.fallout.avatar_frags.ToString();
            labelStatisticsFalloutAvgDamageAssisted.Text = "Average damage assisted: " + playerStats.data.player.statistics.fallout.avg_damage_assisted.ToString();
            labelStatisticsFalloutAvgDamageAssistedRadio.Text = "Average damage assisted radio: " + playerStats.data.player.statistics.fallout.avg_damage_assisted_radio.ToString();
            labelStatisticsFalloutAvgDamageAssistedTrack.Text = "Average damage assisted track: " + playerStats.data.player.statistics.fallout.avg_damage_assisted_track.ToString();
            labelStatisticsFalloutAvgDamageBlocked.Text = "Average damage blocked: " + playerStats.data.player.statistics.fallout.avg_damage_blocked.ToString();
            labelStatisticsFalloutAvgXP.Text = "Average XP: " + playerStats.data.player.statistics.fallout.battle_avg_xp.ToString();
            labelStatisticsFalloutBattles.Text = "Battles: " + playerStats.data.player.statistics.fallout.battles.ToString();
            labelStatisticsFalloutCapturePoints.Text = "Capture points:" + playerStats.data.player.statistics.fallout.capture_points.ToString();
            labelStatisticsFalloutDamageDealt.Text = "Damage dealt: " + playerStats.data.player.statistics.fallout.damage_dealt.ToString();
            labelStatisticsFalloutDamageReceived.Text = "Damage received: " + playerStats.data.player.statistics.fallout.damage_received.ToString();
            labelStatisticsFalloutDeathCount.Text = "Death count: " + playerStats.data.player.statistics.fallout.death_count.ToString();
            labelStatisticsFalloutDirectHitsReceived.Text = "Direct hits received: " + playerStats.data.player.statistics.fallout.direct_hits_received.ToString();
            labelStatisticsFalloutDraws.Text = "Draws: " + playerStats.data.player.statistics.fallout.draws.ToString();
            labelStatisticsFalloutDroppedCapturePoints.Text = "Dropped capture points: " + playerStats.data.player.statistics.fallout.dropped_capture_points.ToString();
            labelStatisticsFalloutExplosionHits.Text = "Explosion hits: " + playerStats.data.player.statistics.fallout.explosion_hits.ToString();
            labelStatisticsFalloutExplosionHitsReceived.Text = "Explosion hits received: " + playerStats.data.player.statistics.fallout.explosion_hits_received.ToString();
            labelStatisticsFalloutFlagCapture.Text = "Flag capture: " + playerStats.data.player.statistics.fallout.flag_capture.ToString();
            labelStatisticsFalloutFlagCaptureSolo.Text = "Flag capture solo: " + playerStats.data.player.statistics.fallout.flag_capture_solo.ToString();
            labelStatisticsFalloutFrags.Text = "Frags: " + playerStats.data.player.statistics.fallout.frags.ToString();
            labelStatisticsFalloutHits.Text = "Hits: " + playerStats.data.player.statistics.fallout.hits.ToString();
            labelStatisticsFalloutHitsPercents.Text = "Hits percents: " + playerStats.data.player.statistics.fallout.hits_percents.ToString();
            labelStatisticsFalloutLosses.Text = "Losses: " + playerStats.data.player.statistics.fallout.losses.ToString();
            labelStatisticsFalloutMaxDamage.Text = "Max damage: " + playerStats.data.player.statistics.fallout.max_damage.ToString();
            //labelStatisticsFalloutMaxDamageTankId.Text = "Max damage tank ID" + playerStats.data.player.statistics.fallout.max_damage_tank_id.ToString();
            labelStatisticsFalloutMaxDamageWithAvatar.Text = "Max damage with avatar: " + playerStats.data.player.statistics.fallout.max_damage_with_avatar.ToString();
            labelStatisticsFalloutMaxFrags.Text = "Max frags: " + playerStats.data.player.statistics.fallout.max_frags.ToString();
            labelStatisticsFalloutMaxWinPoints.Text = "Max win points: " + playerStats.data.player.statistics.fallout.max_win_points.ToString();
            labelStatisticsFalloutMaxXp.Text = "Max XP: " + playerStats.data.player.statistics.fallout.max_xp.ToString();
            //labelStatisticsFalloutMaxXpTankId.Text = "Max XP tank ID:" + playerStats.data.player.statistics.fallout.max_xp_tank_id.ToString();
            labelStatisticsFalloutNoDamageDirectHitsReceived.Text = "No damage direct hits received: " + playerStats.data.player.statistics.fallout.no_damage_direct_hits_received.ToString();
            labelStatisticsFalloutPiercing.Text = "Piercing:" + playerStats.data.player.statistics.fallout.piercings.ToString();
            labelStatisticsFalloutPiercingReceived.Text = "Piercing received: " + playerStats.data.player.statistics.fallout.piercings.ToString();
            labelStatisticsFalloutResourceAbsorbed.Text = "Resource absorbed: " + playerStats.data.player.statistics.fallout.resource_absorbed.ToString();
            labelStatisticsFalloutShots.Text = "Shots: " + playerStats.data.player.statistics.fallout.shots.ToString();
            labelStatisticsFalloutSpotted.Text = "Spotted: " + playerStats.data.player.statistics.fallout.spotted.ToString();
            labelStatisticsFalloutSurvivedBattles.Text = "Survived battles: " + playerStats.data.player.statistics.fallout.survived_battles.ToString();
            labelStatisticsFalloutTankingFactor.Text = "Tanking factor: " + playerStats.data.player.statistics.fallout.tanking_factor.ToString();
            labelStatisticsFalloutWinPoints.Text = "Win points: " + playerStats.data.player.statistics.fallout.win_points.ToString();
            labelStatisticsFalloutWins.Text = "Wins: " + playerStats.data.player.statistics.fallout.wins.ToString();
            labelStatisticsFalloutXp.Text = "XP: " + playerStats.data.player.statistics.fallout.xp.ToString();
            //****************************************
            labelStatisticsGAbsoluteAvgDamageAssisted.Text = "Average damage assisted: " + playerStats.data.player.statistics.globalmap_absolute.avg_damage_assisted.ToString();
            labelStatisticsGAbsoluteAvgDamageAssistedRadio.Text = "Average damage assisded radio: " + playerStats.data.player.statistics.globalmap_absolute.avg_damage_assisted_radio.ToString();
            labelStatisticsGAbsoluteAvgDamageAssistedTrack.Text = "Average Damage Assisted track: " + playerStats.data.player.statistics.globalmap_absolute.avg_damage_assisted_track.ToString();
            labelStatisticsGAbsoluteAvgDamageBlocked.Text = "Average damage blocked: " + playerStats.data.player.statistics.globalmap_absolute.avg_damage_blocked.ToString();
            labelStatisticsGAbsoluteBattleAvgXp.Text = "Battle average XP: " + playerStats.data.player.statistics.globalmap_absolute.battle_avg_xp.ToString();
            labelStatisticsGAbsoluteBattles.Text = "Battles: " + playerStats.data.player.statistics.globalmap_absolute.battles.ToString();
            labelStatisticsGAbsoluteCapturePoints.Text = "Capture points: " + playerStats.data.player.statistics.globalmap_absolute.capture_points.ToString();
            labelStatisticsGAbsoluteDamageDealt.Text = "Damage dealt: " + playerStats.data.player.statistics.globalmap_absolute.damage_dealt.ToString();
            labelStatisticsGAbsoluteDamageReceived.Text = "Damage received: " + playerStats.data.player.statistics.globalmap_absolute.damage_received.ToString();
            labelStatisticsGAbsoluteDirectHitsReceived.Text = "Direct hits received: " + playerStats.data.player.statistics.globalmap_absolute.direct_hits_received.ToString();
            labelStatisticsGAbsoluteDraws.Text = "Draws: " + playerStats.data.player.statistics.globalmap_absolute.draws.ToString();
            labelStatisticsGAbsoluteDroppedCapturePoints.Text = "Dropped capture points: " + playerStats.data.player.statistics.globalmap_absolute.dropped_capture_points.ToString();
            labelStatisticsGAbsoluteExplosionHits.Text = "Explosion hits: " + playerStats.data.player.statistics.globalmap_absolute.explosion_hits.ToString();
            labelStatisticsGAbsoluteExplosionHitsReceived.Text = "Explosion hits received: " + playerStats.data.player.statistics.globalmap_absolute.explosion_hits_received.ToString();
            labelStatisticsGAbsoluteFrags.Text = "Frags: " + playerStats.data.player.statistics.globalmap_absolute.frags.ToString();
            labelStatisticsGAbsoluteHits.Text = "Hits: " + playerStats.data.player.statistics.globalmap_absolute.hits.ToString();
            labelStatisticsGAbsoluteHitsPercents.Text = "Hits percents: " + playerStats.data.player.statistics.globalmap_absolute.hits_percents.ToString();
            labelStatisticsGAbsoluteLosses.Text = "Losses: " + playerStats.data.player.statistics.globalmap_absolute.losses.ToString();
            labelStatisticsGAbsoluteNoDamageHitsReceived.Text = "No damage hits received: " + playerStats.data.player.statistics.globalmap_absolute.no_damage_direct_hits_received.ToString();
            labelStatisticsGAbsolutePiercing.Text = "Piercing: " + playerStats.data.player.statistics.globalmap_absolute.piercings.ToString();
            labelStatisticsGAbsolutePiercingReceived.Text = "Piercing received: " + playerStats.data.player.statistics.globalmap_absolute.piercings_received.ToString();
            labelStatisticsGAbsoluteShots.Text = "Shots: " + playerStats.data.player.statistics.globalmap_absolute.shots.ToString();
            labelStatisticsGAbsoluteSpotted.Text = "Spotted: " + playerStats.data.player.statistics.globalmap_absolute.spotted.ToString();
            labelStatisticsGAbsoluteSurvivedBattles.Text = "Survived battles: " + playerStats.data.player.statistics.globalmap_absolute.survived_battles.ToString();
            labelStatisticsGAbsoluteTankingFactor.Text = "Tanking factor: " + playerStats.data.player.statistics.globalmap_absolute.tanking_factor.ToString();
            labelStatisticsGAbsoluteWins.Text = "Wins: " + playerStats.data.player.statistics.globalmap_absolute.wins.ToString();
            labelStatisticsGAbsoluteXp.Text = "XP: " + playerStats.data.player.statistics.globalmap_absolute.xp.ToString();
            //****************************************
            labelStatisticsGChampionAvgDamageAssisted.Text = "Average damage assisted: " + playerStats.data.player.statistics.globalmap_champion.avg_damage_assisted.ToString();
            labelStatisticsGChampionAvgDamageAssistedRadio.Text = "Average damage assisded radio: " + playerStats.data.player.statistics.globalmap_champion.avg_damage_assisted_radio.ToString();
            labelStatisticsGChampionAvgDamageAssistedTrack.Text = "Average Damage Assisted track: " + playerStats.data.player.statistics.globalmap_champion.avg_damage_assisted_track.ToString();
            labelStatisticsGChampionAvgDamageBlocked.Text = "Average damage blocked: " + playerStats.data.player.statistics.globalmap_champion.avg_damage_blocked.ToString();
            labelStatisticsGChampionBattleAvgXp.Text = "Battle average XP: " + playerStats.data.player.statistics.globalmap_champion.battle_avg_xp.ToString();
            labelStatisticsGChampionBattles.Text = "Battles: " + playerStats.data.player.statistics.globalmap_champion.battles.ToString();
            labelStatisticsGChampionCapturePoints.Text = "Capture points: " + playerStats.data.player.statistics.globalmap_champion.capture_points.ToString();
            labelStatisticsGChampionDamageDealt.Text = "Damage dealt: " + playerStats.data.player.statistics.globalmap_champion.damage_dealt.ToString();
            labelStatisticsGChampionDamageReceived.Text = "Damage received: " + playerStats.data.player.statistics.globalmap_champion.damage_received.ToString();
            labelStatisticsGChampionDirectHitsReceived.Text = "Direct hits received: " + playerStats.data.player.statistics.globalmap_champion.direct_hits_received.ToString();
            labelStatisticsGChampionDraws.Text = "Draws: " + playerStats.data.player.statistics.globalmap_champion.draws.ToString();
            labelStatisticsGChampionDroppedCapturePoints.Text = "Dropped capture points: " + playerStats.data.player.statistics.globalmap_champion.dropped_capture_points.ToString();
            labelStatisticsGChampionExplosionHits.Text = "Explosion hits: " + playerStats.data.player.statistics.globalmap_champion.explosion_hits.ToString();
            labelStatisticsGChampionExplosionHitsReceived.Text = "Explosion hits received: " + playerStats.data.player.statistics.globalmap_champion.explosion_hits_received.ToString();
            labelStatisticsGChampionFrags.Text = "Frags: " + playerStats.data.player.statistics.globalmap_champion.frags.ToString();
            labelStatisticsGChampionHits.Text = "Hits: " + playerStats.data.player.statistics.globalmap_champion.hits.ToString();
            labelStatisticsGChampionHitsPercents.Text = "Hits percents: " + playerStats.data.player.statistics.globalmap_champion.hits_percents.ToString();
            labelStatisticsGChampionLosses.Text = "Losses: " + playerStats.data.player.statistics.globalmap_champion.losses.ToString();
            labelStatisticsGChampionNoDamageHitsReceived.Text = "No damage hits received: " + playerStats.data.player.statistics.globalmap_champion.no_damage_direct_hits_received.ToString();
            labelStatisticsGChampionPiercing.Text = "Piercing: " + playerStats.data.player.statistics.globalmap_champion.piercings.ToString();
            labelStatisticsGChampionPiercingReceived.Text = "Piercing received: " + playerStats.data.player.statistics.globalmap_champion.piercings_received.ToString();
            labelStatisticsGChampionShots.Text = "Shots: " + playerStats.data.player.statistics.globalmap_champion.shots.ToString();
            labelStatisticsGChampionSpotted.Text = "Spotted: " + playerStats.data.player.statistics.globalmap_champion.spotted.ToString();
            labelStatisticsGChampionSurvivedBattles.Text = "Survived battles: " + playerStats.data.player.statistics.globalmap_champion.survived_battles.ToString();
            labelStatisticsGChampionTankingFactor.Text = "Tanking factor: " + playerStats.data.player.statistics.globalmap_champion.tanking_factor.ToString();
            labelStatisticsGChampionWins.Text = "Wins: " + playerStats.data.player.statistics.globalmap_champion.wins.ToString();
            labelStatisticsGChampionXp.Text = "XP: " + playerStats.data.player.statistics.globalmap_champion.xp.ToString();
            //****************************************
            labelStatisticsGMiddleAvgDamageAssisted.Text = "Average damage assisted: " + playerStats.data.player.statistics.globalmap_middle.avg_damage_assisted.ToString();
            labelStatisticsGMiddleAvgDamageAssistedRadio.Text = "Average damage assisded radio: " + playerStats.data.player.statistics.globalmap_middle.avg_damage_assisted_radio.ToString();
            labelStatisticsGMiddleAvgDamageAssistedTrack.Text = "Average Damage Assisted track: " + playerStats.data.player.statistics.globalmap_middle.avg_damage_assisted_track.ToString();
            labelStatisticsGMiddleAvgDamageBlocked.Text = "Average damage blocked: " + playerStats.data.player.statistics.globalmap_middle.avg_damage_blocked.ToString();
            labelStatisticsGMiddleBattleAvgXp.Text = "Battle average XP: " + playerStats.data.player.statistics.globalmap_middle.battle_avg_xp.ToString();
            labelStatisticsGMiddleBattles.Text = "Battles: " + playerStats.data.player.statistics.globalmap_middle.battles.ToString();
            labelStatisticsGMiddleCapturePoints.Text = "Capture points: " + playerStats.data.player.statistics.globalmap_middle.capture_points.ToString();
            labelStatisticsGMiddleDamageDealt.Text = "Damage dealt: " + playerStats.data.player.statistics.globalmap_middle.damage_dealt.ToString();
            labelStatisticsGMiddleDamageReceived.Text = "Damage received: " + playerStats.data.player.statistics.globalmap_middle.damage_received.ToString();
            labelStatisticsGMiddleDirectHitsReceived.Text = "Direct hits received: " + playerStats.data.player.statistics.globalmap_middle.direct_hits_received.ToString();
            labelStatisticsGMiddleDraws.Text = "Draws: " + playerStats.data.player.statistics.globalmap_middle.draws.ToString();
            labelStatisticsGMiddleDroppedCapturePoints.Text = "Dropped capture points: " + playerStats.data.player.statistics.globalmap_middle.dropped_capture_points.ToString();
            labelStatisticsGMiddleExplosionHits.Text = "Explosion hits: " + playerStats.data.player.statistics.globalmap_middle.explosion_hits.ToString();
            labelStatisticsGMiddleExplosionHitsReceived.Text = "Explosion hits received: " + playerStats.data.player.statistics.globalmap_middle.explosion_hits_received.ToString();
            labelStatisticsGMiddleFrags.Text = "Frags: " + playerStats.data.player.statistics.globalmap_middle.frags.ToString();
            labelStatisticsGMiddleHits.Text = "Hits: " + playerStats.data.player.statistics.globalmap_middle.hits.ToString();
            labelStatisticsGMiddleHitsPercents.Text = "Hits percents: " + playerStats.data.player.statistics.globalmap_middle.hits_percents.ToString();
            labelStatisticsGMiddleLosses.Text = "Losses: " + playerStats.data.player.statistics.globalmap_middle.losses.ToString();
            labelStatisticsGMiddleNoDamageHitsReceived.Text = "No damage hits received: " + playerStats.data.player.statistics.globalmap_middle.no_damage_direct_hits_received.ToString();
            labelStatisticsGMiddlePiercing.Text = "Piercing: " + playerStats.data.player.statistics.globalmap_middle.piercings.ToString();
            labelStatisticsGMiddlePiercingReceived.Text = "Piercing received: " + playerStats.data.player.statistics.globalmap_middle.piercings_received.ToString();
            labelStatisticsGMiddleShots.Text = "Shots: " + playerStats.data.player.statistics.globalmap_middle.shots.ToString();
            labelStatisticsGMiddleSpotted.Text = "Spotted: " + playerStats.data.player.statistics.globalmap_middle.spotted.ToString();
            labelStatisticsGMiddleSurvivedBattles.Text = "Survived battles: " + playerStats.data.player.statistics.globalmap_middle.survived_battles.ToString();
            labelStatisticsGMiddleTankingFactor.Text = "Tanking factor: " + playerStats.data.player.statistics.globalmap_middle.tanking_factor.ToString();
            labelStatisticsGMiddleWins.Text = "Wins: " + playerStats.data.player.statistics.globalmap_middle.wins.ToString();
            labelStatisticsGMiddleXp.Text = "XP: " + playerStats.data.player.statistics.globalmap_middle.xp.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
