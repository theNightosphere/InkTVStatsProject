namespace InkTVStats.Models
{
    public class AveragePlayerStatsResponse
    {
        public string? PlayerName { get; set; }
        public double AverageTurfPainted { get; set; }
        public double AverageTakedowns { get; set; }
        public double AverageKills { get; set; }
        public double AverageAssists { get; set; }
        public double AverageDeaths { get; set; }
        public double AverageSpecials { get; set; }
        public TimeSpan AverageGameTime { get; set; }
        public double AverageTeamScore { get; set; }
        public int TotalGamesPlayed { get; set; }

    }
}
