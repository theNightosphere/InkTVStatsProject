namespace InkTVStats.Models
{
    public class PlayerMapWinrates
    {
        public string? PlayerName { get; set; }
        public List<MapWinrate> MapWinrates { get; set; } = new List<MapWinrate>();

        public PlayerMapWinrates()
        {
            PlayerName = string.Empty;
        }
    }
}
