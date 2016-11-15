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
            //**************************************************
            labelBattleLifeTime.Text = playerStats.data.player.@private.battle_life_time.ToString();
            labelCredits.Text = playerStats.data.player.@private.credits.ToString();
            labelFreeXP.Text = playerStats.data.player.@private.free_xp.ToString();
            labelGold.Text = playerStats.data.player.@private.gold.ToString();
            labelIsBoundToPhone.Text = playerStats.data.player.@private.is_bound_to_phone.ToString();
            labelIsPremium.Text = playerStats.data.player.@private.is_premium.ToString();
            labelPremiumExperiesAt.Text = playerStats.data.player.@private.premium_expires_at.ToString();
            //**************************************************
            labelRestrictions.Text = playerStats.data.player.@private.restrictions.ToString();
            //labelRestrictionsChatBanTime.Text = playerStats.data.player.@private.restrictions.chat_ban_time.ToString();
            //labelRestrictionsClanTime.Text = playerStats.data.player.@private.restrictions.clan_time.ToString();
            labelStatisticsFrags.Text = playerStats.data.player.statistics.frags.ToString();
            labelStatisticsTreesCut.Text = playerStats.data.player.statistics.trees_cut.ToString();
            //**************************************************
            labelStatisticsAllAvgDamageAssisted.Text = playerStats.data.player.statistics.all.avg_damage_assisted.ToString();
            labelStatisticsAllAvgDamageAssistedRadio.Text = playerStats.data.player.statistics.all.avg_damage_assisted_radio.ToString();
            labelStatisticsAllAvgDamageAssistedTrack.Text = playerStats.data.player.statistics.all.avg_damage_assisted_track.ToString();
            labelStatisticsAllAvgDamageBlocked.Text = playerStats.data.player.statistics.all.avg_damage_blocked.ToString();
            labelStatisticsAllBattleAvgXp.Text = playerStats.data.player.statistics.all.battle_avg_xp.ToString();
            labelStatisticsAllBattles.Text = playerStats.data.player.statistics.all.battles.ToString();
            labelStatisticsAllCapturePoints.Text = playerStats.data.player.statistics.all.capture_points.ToString();
            labelStatisticsAllDamageDealt.Text = playerStats.data.player.statistics.all.damage_dealt.ToString();
            labelStatisticsAllDamageReceived.Text = playerStats.data.player.statistics.all.damage_received.ToString();
            labelStatisticsAllDirectHitsReceived.Text = playerStats.data.player.statistics.all.direct_hits_received.ToString();
            labelStatisticsAllDraws.Text = playerStats.data.player.statistics.all.draws.ToString();
            labelStatisticsAllDroppedCapturePoints.Text = playerStats.data.player.statistics.all.dropped_capture_points.ToString();
            labelStatisticsAllExplosionHits.Text = playerStats.data.player.statistics.all.explosion_hits.ToString();
            labelStatisticsAllExplosionHitsReceived.Text = playerStats.data.player.statistics.all.explosion_hits_received.ToString();
            labelStatisticsAllFrags.Text = playerStats.data.player.statistics.all.frags.ToString();
            labelStatisticsAllHits.Text = playerStats.data.player.statistics.all.hits.ToString();
            labelStatisticsAllLosses.Text = playerStats.data.player.statistics.all.losses.ToString();
            labelStatisticsAllMaxDamage.Text = playerStats.data.player.statistics.all.max_damage.ToString();
            labelStatisticsAllMaxDamageTankId.Text = playerStats.data.player.statistics.all.max_damage_tank_id.ToString();
            labelStatisticsAllMaxFrags.Text = playerStats.data.player.statistics.all.max_frags.ToString();
            labelStatisticsAllMaxFragsTankId.Text = playerStats.data.player.statistics.all.max_frags_tank_id.ToString();
            labelStatisticsAllMaxXp.Text = playerStats.data.player.statistics.all.max_xp.ToString();
            labelStatisticsAllMaxXpTankId.Text = playerStats.data.player.statistics.all.max_xp_tank_id.ToString();
            labelStatisticsAllNoDamageDirectHitsReceived.Text = playerStats.data.player.statistics.all.no_damage_direct_hits_received.ToString();
            labelStatisticsAllPiercing.Text = playerStats.data.player.statistics.all.piercings.ToString();
            labelStatisticsAllPiercingReceived.Text = playerStats.data.player.statistics.all.piercings_received.ToString();
            labelStatisticsAllShots.Text = playerStats.data.player.statistics.all.shots.ToString();
            labelStatisticsAllSpotted.Text = playerStats.data.player.statistics.all.spotted.ToString();
            labelStatisticsAllSurvivedBattles.Text = playerStats.data.player.statistics.all.survived_battles.ToString();
            labelStatisticsAllTankingFactor.Text = playerStats.data.player.statistics.all.tanking_factor.ToString();
            labelStatisticsAllWins.Text = playerStats.data.player.statistics.all.wins.ToString();
            labelStatisticsAllXp.Text = playerStats.data.player.statistics.all.xp.ToString();
            //*****************************************
            labelStatisticsClanAvgDamageAssisted.Text = playerStats.data.player.statistics.clan.avg_damage_assisted.ToString();
            labelStatisticsClanAvgDamageAssistedRadio.Text = playerStats.data.player.statistics.clan.avg_damage_assisted_radio.ToString();
            labelStatisticsClanAvgDamageAssistedTrack.Text = playerStats.data.player.statistics.clan.avg_damage_assisted_track.ToString();
            labelStatisticsClanAvgDamageBlocked.Text = playerStats.data.player.statistics.clan.avg_damage_blocked.ToString();
            labelStatisticsClanBattleAvgXp.Text = playerStats.data.player.statistics.clan.battle_avg_xp.ToString();
            labelStatisticsClanBattles.Text = playerStats.data.player.statistics.clan.battles.ToString();
            labelStatisticsClanCapturePoints.Text = playerStats.data.player.statistics.clan.capture_points.ToString();
            labelStatisticsClanDamageDealt.Text = playerStats.data.player.statistics.clan.damage_dealt.ToString();
            labelStatisticsClanDamageReceived.Text = playerStats.data.player.statistics.clan.damage_received.ToString();
            labelStatisticsClanDirectHitsReceived.Text = playerStats.data.player.statistics.clan.direct_hits_received.ToString();
            labelStatisticsClanDraws.Text = playerStats.data.player.statistics.clan.draws.ToString();
            labelStatisticsClanDroppedCapturePoints.Text = playerStats.data.player.statistics.clan.capture_points.ToString();
            labelStatisticsClanExplosionHits.Text = playerStats.data.player.statistics.clan.explosion_hits.ToString();
            labelStatisticsClanExplosionHitsReceived.Text = playerStats.data.player.statistics.clan.explosion_hits_received.ToString();
            labelStatisticsClanFrags.Text = playerStats.data.player.statistics.clan.frags.ToString();
            labelStatisticsClanHits.Text = playerStats.data.player.statistics.clan.hits.ToString();
            labelStatisticsClanHitsPercentage.Text = playerStats.data.player.statistics.clan.hits_percents.ToString();
            labelStatisticsClanLosses.Text = playerStats.data.player.statistics.clan.losses.ToString();
            labelStatisticsClanNoDamageDirectHitsReceived.Text = playerStats.data.player.statistics.clan.no_damage_direct_hits_received.ToString();
            labelStatisticsClanPiercings.Text = playerStats.data.player.statistics.clan.piercings.ToString();
            labelStatisticsClanPiercingsReceived.Text = playerStats.data.player.statistics.clan.piercings_received.ToString();
            labelStatisticsClanShots.Text = playerStats.data.player.statistics.clan.shots.ToString();
            labelStatisticsClanSurvivedBattles.Text = playerStats.data.player.statistics.clan.survived_battles.ToString();
            labelStatisticsClanTankingFactor.Text = playerStats.data.player.statistics.clan.tanking_factor.ToString();
            labelStatisticsClanWins.Text = playerStats.data.player.statistics.clan.wins.ToString();
            labelStatisticsClanXp.Text = playerStats.data.player.statistics.clan.xp.ToString();
            //****************************************
            labelStatisticsCompanyAvgDamageAssisted.Text = playerStats.data.player.statistics.company.avg_damage_assisted.ToString();
            labelStatisticsCompanyAvgDamageAssistedRadio.Text = playerStats.data.player.statistics.company.avg_damage_assisted_radio.ToString();
            labelStatisticsCompanyAvgDamageAssistedTrack.Text = playerStats.data.player.statistics.company.avg_damage_assisted_track.ToString();
            labelStatisticsCompanyAvgDamageBlocked.Text = playerStats.data.player.statistics.company.avg_damage_blocked.ToString();
            labelStatisticsCompanyBattleAvgXp.Text = playerStats.data.player.statistics.company.battle_avg_xp.ToString();
            labelStatisticsCompanyBattles.Text = playerStats.data.player.statistics.company.battles.ToString();
            labelStatisticsCompanyCapturePoints.Text = playerStats.data.player.statistics.company.capture_points.ToString();
            labelStatisticsCompanyDamageDealt.Text = playerStats.data.player.statistics.company.damage_dealt.ToString();
            labelStatisticsCompanyDamageReceived.Text = playerStats.data.player.statistics.company.damage_received.ToString();
            labelStatisticsCompanyDirectHitsReceived.Text = playerStats.data.player.statistics.company.direct_hits_received.ToString();
            labelStatisticsCompanyDraws.Text = playerStats.data.player.statistics.company.draws.ToString();
            labelStatisticsCompanyDroppedCapturePoints.Text = playerStats.data.player.statistics.company.dropped_capture_points.ToString();
            labelStatisticsCompanyExplosionHits.Text = playerStats.data.player.statistics.company.explosion_hits.ToString();
            labelStatisticsCompanyExplosionHitsReceived.Text = playerStats.data.player.statistics.company.explosion_hits_received.ToString();
            labelStatisticsCompanyFrags.Text = playerStats.data.player.statistics.company.frags.ToString();
            labelStatisticsCompanyHits.Text = playerStats.data.player.statistics.company.hits.ToString();
            labelStatisticsCompanyHitsPercents.Text = playerStats.data.player.statistics.company.hits_percents.ToString();
            labelStatisticsCompanyLosses.Text = playerStats.data.player.statistics.company.losses.ToString();
            labelStatisticsCompanyNoDamageDirectReceived.Text = playerStats.data.player.statistics.company.no_damage_direct_hits_received.ToString();
            labelStatisticsCompanyPiercing.Text = playerStats.data.player.statistics.company.piercings.ToString();
            labelStatisticsCompanyPiercingsReceived.Text = playerStats.data.player.statistics.company.piercings_received.ToString();
            labelStatisticsCompanyShots.Text = playerStats.data.player.statistics.company.shots.ToString();
            labelStatisticsCompanySpotted.Text = playerStats.data.player.statistics.company.spotted.ToString();
            labelStatisticsCompanySurvivedBattles.Text = playerStats.data.player.statistics.company.survived_battles.ToString();
            labelStatisticsCompanyTankingFactor.Text = playerStats.data.player.statistics.company.tanking_factor.ToString();
            labelStatisticsCompanyWins.Text = playerStats.data.player.statistics.company.wins.ToString();
            labelStatisticsCompanyXp.Text = playerStats.data.player.statistics.company.xp.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
