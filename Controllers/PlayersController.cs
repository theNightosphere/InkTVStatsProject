using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using InkTVStats.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InkTVStats.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController : ControllerBase
    {
        const string SPREADSHEET_ID = "1vBxPer_lRk3oj3hWBFUY8owg7xTz44w3I_BG7tcq2Is";
        const string SHEET_NAME = "Match Reporting";

        SpreadsheetsResource.ValuesResource _googleSheetValues;

        private readonly ILogger<PlayersController> _logger;

        public PlayersController(ILogger<PlayersController> logger, GoogleSheetsHelper helper)
        {
            _logger = logger;
            _googleSheetValues = helper.Service.Spreadsheets.Values;
        }

        [HttpGet(Name = "GetAllStats")]
        public IActionResult Get()
        {
            string range = $"{SHEET_NAME}!A26:M";

            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;

            return Ok(PlayerMatchMapper.MapFromRangeData(values));
        }

        [Route("{playerName}/AverageStats/")]
        [HttpGet]
        public IActionResult GetAveragePlayerStats([FromRoute(Name = "playerName")][Required] string playerName)
        {
            string range = $"{SHEET_NAME}!A26:M";

            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            ValueRange googleSheetsValues = request.Execute();
            IList<IList<object>> values = googleSheetsValues.Values;

            List<PlayerMatchStat> overallStats = PlayerMatchMapper.MapFromRangeData(values);

            List<PlayerMatchStat> individualPlayerStats = GetMatchStatsForPlayer(overallStats, playerName);

            PlayerMatchStat playerTotalStats = new PlayerMatchStat();
            foreach (PlayerMatchStat stat in individualPlayerStats)
            {
                playerTotalStats.TurfAmount += stat.TurfAmount;
                playerTotalStats.Takedowns += stat.Takedowns;
                playerTotalStats.Kills += stat.Kills;
                playerTotalStats.Assists += stat.Assists;
                playerTotalStats.Deaths += stat.Deaths;
                playerTotalStats.Specials += stat.Specials;
                playerTotalStats.TeamScore += stat.TeamScore;
                playerTotalStats.GameTime += stat.GameTime;
            }
            AveragePlayerStatsResponse response = new AveragePlayerStatsResponse()
            {
                PlayerName = playerName,
                AverageTurfPainted = (double)playerTotalStats.TurfAmount / individualPlayerStats.Count,
                AverageTakedowns = (double)playerTotalStats.Takedowns / individualPlayerStats.Count,
                AverageKills = (double)playerTotalStats.Kills / individualPlayerStats.Count,
                AverageDeaths = (double)playerTotalStats.Deaths / individualPlayerStats.Count,
                AverageSpecials = (double)playerTotalStats.Specials / individualPlayerStats.Count,
                AverageTeamScore = (double)playerTotalStats.TeamScore / individualPlayerStats.Count,
                AverageAssists = (double)playerTotalStats.Assists / individualPlayerStats.Count,
                AverageGameTime = playerTotalStats.GameTime / individualPlayerStats.Count,
                TotalGamesPlayed = individualPlayerStats.Count
            };

            return Ok(response);
        }

        [Route("{playerName}/TurfPerWeapon/")]
        [HttpGet]
        public IActionResult GetTurfPerWeapon([FromRoute(Name = "playerName")][Required] string playerName)
        {
            string range = $"{SHEET_NAME}!A26:M";

            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;

            List<PlayerMatchStat> overallStats = PlayerMatchMapper.MapFromRangeData(values);

            List<PlayerMatchStat> individualPlayerStats = GetMatchStatsForPlayer(overallStats, playerName);

            Dictionary<string, int> totalTurfPaintedDict = new Dictionary<string, int>();
            Dictionary<string, int> totalMapsPlayedPerWeaponDict = new Dictionary<string, int>();

            // Calculate total turf painted per weapon and number of maps played with it. 
            foreach (PlayerMatchStat stat in individualPlayerStats)
            {
                if (!string.IsNullOrEmpty(stat.Weapon))
                {
                    if (totalMapsPlayedPerWeaponDict.ContainsKey(stat.Weapon.ToLowerInvariant()))
                    {
                        // Increase total turf painted
                        totalTurfPaintedDict[stat.Weapon.ToLowerInvariant()] += stat.TurfAmount;
                        // INcrement games
                        totalMapsPlayedPerWeaponDict[stat.Weapon.ToLowerInvariant()]++;
                    }
                    else
                    {
                        totalTurfPaintedDict.Add(stat.Weapon.ToLowerInvariant(), stat.TurfAmount);
                        totalMapsPlayedPerWeaponDict.Add(stat.Weapon.ToLowerInvariant(), 1);
                    }
                }
            }

            TurfPerWeaponResponse responseVal = new TurfPerWeaponResponse();
            foreach (string weapon in totalTurfPaintedDict.Keys)
            {
                int totalTurfPainted = totalTurfPaintedDict[weapon];
                int mapsPlayedWithWeapon = totalMapsPlayedPerWeaponDict[weapon];
                int averageTurfPainted = mapsPlayedWithWeapon > 1 ? totalTurfPainted / mapsPlayedWithWeapon : totalTurfPainted;
                responseVal.WeaponToAverageTurfValue.Add(weapon, averageTurfPainted);
                responseVal.WeaponToMapsPlayed.Add(weapon, mapsPlayedWithWeapon);
            }
            responseVal.PlayerName = playerName;

            return Ok(responseVal);
        }

        private List<PlayerMatchStat> GetMatchStatsForPlayer(List<PlayerMatchStat> stats, string playerName)
        {
            List<PlayerMatchStat> playerStats = new List<PlayerMatchStat>();
            foreach (PlayerMatchStat stat in stats)
            {
                if (playerName.ToLowerInvariant().StartsWith(stat.Name.ToLowerInvariant()))
                {
                    playerStats.Add(stat);
                }
            }
            return playerStats;
        }

        [Route("{playerName}/MapWinrate")]
        [HttpGet]
        public IActionResult GetPlayerMapWinrate([FromRoute(Name = "playerName")][Required] string playerName)
        {
            string range = $"{SHEET_NAME}!A26:M";

            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            ValueRange googleSheetsValues = request.Execute();
            IList<IList<object>> values = googleSheetsValues.Values;

            List<PlayerMatchStat> overallStats = PlayerMatchMapper.MapFromRangeData(values);

            List<PlayerMatchStat> individualPlayerStats = GetMatchStatsForPlayer(overallStats, playerName);

            Dictionary<string, MapWinrate> mapToWinrateDict = new Dictionary<string, MapWinrate>();

            foreach (PlayerMatchStat stat in individualPlayerStats)
            {
                if (!string.IsNullOrEmpty(stat.Stage))
                {
                    if (mapToWinrateDict.ContainsKey(stat.Stage))
                    {
                        if (stat.WonMatch)
                        {
                            mapToWinrateDict[stat.Stage].Wins++;
                        }
                        else
                        {
                            mapToWinrateDict[stat.Stage].Losses++;
                        }
                    }
                    else
                    {
                        if (stat.WonMatch)
                        {
                            mapToWinrateDict.Add(stat.Stage, new MapWinrate() { MapName = stat.Stage, Wins = 1 });
                        }
                        else
                        {
                            mapToWinrateDict.Add(stat.Stage, new MapWinrate() { MapName = stat.Stage, Losses = 1 });
                        }
                    }
                }
            }

            PlayerMapWinrates winrates = new PlayerMapWinrates()
            {
                PlayerName = playerName,
                MapWinrates = mapToWinrateDict.Values.ToList()
            };

            return Ok(winrates);
        }
    }
}