namespace InkTVStats.Models
{
    public class MapWinrate
    {
        public string? MapName { get; set; }
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public double WinPercentage { get { return (((double)Wins) / (Wins + Losses)) * 100; } }
    }
}
