namespace InkTVStats.Models
{
    public class TurfPerWeaponResponse
    {
        public string PlayerName { get; set; }
        public Dictionary<string, double> WeaponToAverageTurfValue { get; set; }
        public Dictionary<string, int> WeaponToMapsPlayed { get; set; }

        public TurfPerWeaponResponse()
        {
            PlayerName = string.Empty;
            WeaponToAverageTurfValue = new Dictionary<string, double>();
            WeaponToMapsPlayed = new Dictionary<string, int>();
        }
    }


}
