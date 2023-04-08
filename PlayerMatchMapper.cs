using System.Globalization;

namespace InkTVStats
{
    public static class PlayerMatchMapper
    {
        public static List<PlayerMatchStat> MapFromRangeData(IList<IList<object>> values)
        {
            List<PlayerMatchStat> stats = new List<PlayerMatchStat>();

            foreach (var value in values)
            {

                PlayerMatchStat stat = new PlayerMatchStat();

                stat.Name = value[0].ToString();
                stat.Weapon = value[1].ToString();
                if (Int32.TryParse(value[2].ToString(), out int turfAmount))
                {
                    stat.TurfAmount = turfAmount;
                }
                else
                {
                    stat.TurfAmount = -1;
                }
                if (Int32.TryParse(value[3].ToString(), out int takedowns))
                {
                    stat.Takedowns = takedowns;
                }
                else
                {
                    stat.Takedowns = -1;
                }
                if (Int32.TryParse(value[4].ToString(), out int assists))
                {
                    stat.Assists = assists;
                }
                else
                {
                    stat.Assists = -1;
                }
                stat.Kills = stat.Takedowns - stat.Assists;
                if (Int32.TryParse(value[5].ToString(), out int deaths))
                {
                    stat.Deaths = deaths;
                }
                else
                {
                    stat.Deaths = -1;
                }
                if (Int32.TryParse(value[6].ToString(), out int specials))
                {
                    stat.Specials = specials;
                }
                else
                {
                    stat.Specials = -1;
                }
                if (Int32.TryParse(value[7].ToString(), out int winInt))
                {
                    stat.WonMatch = winInt == 1;
                }
                else
                {
                    stat.WonMatch = false;
                }
                if (Int32.TryParse(value[9].ToString(), out int teamScore))
                {
                    stat.TeamScore = teamScore;
                }
                else
                {
                    stat.TeamScore = -1;
                }
                bool parsed = TimeSpan.TryParseExact((string)value[10], "m\\:ss", CultureInfo.InvariantCulture, out TimeSpan result);
                if (parsed)
                {
                    stat.GameTime = result;
                }
                else
                {
                    stat.GameTime = new TimeSpan(0);
                }
                stat.Mode = (string)value[11];
                stat.Stage = (string)value[12];
                stats.Add(stat);
            }

            return stats;
        }
    }
}
