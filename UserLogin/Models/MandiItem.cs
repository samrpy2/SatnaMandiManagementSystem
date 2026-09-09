namespace UserLogin.Models
{
    public class MandiItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = ""; // जैसे: केला, आलू, टमाटर
        public double QuantityInQuintal { get; set; } // मात्रा (क्विंटल में)
        public double TodayRatePerQuintal { get; set; } // आज का थोक भाव (प्रति क्विंटल)
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow; // कब अपडेट हुआ
    }
}
