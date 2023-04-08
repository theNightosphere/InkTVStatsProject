namespace InkTVStats
{
    public class PlayerMatchStat
    {
        public string? Name { get; set; }
        public string? Weapon { get; set; }
        public int TurfAmount { get; set; }
        public int Takedowns { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
        public int Specials { get; set; }
        public bool WonMatch { get; set; }
        public int TeamScore { get; set; }
        public TimeSpan GameTime { get; set; }
        public string? Mode { get; set; }
        public string? Stage { get; set; }
    }
}
