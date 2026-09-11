using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models
{
    public class MandiItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = ""; // जैसे: केला, आलू, टमाटर
        public double QuantityInQuintal { get; set; } // मात्रा (क्विंटल में)
        public double TodayRatePerQuintal { get; set; } // आज का थोक भाव (प्रति क्विंटल)
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow; // कब अपडेट हुआ
                                                                     // 📌 उत्पाद की श्रेणी (फल है या सब्जी)
        [Required(ErrorMessage = "कैटेगरी चुनना अनिवार्य है")]
        public string Category { get; set; } = "Vegetable"; // Default value
                                                            // 📌 कल का भाव (दाम में उतार-चढ़ाव देखने के लिए)
        public double YesterdayRatePerQuintal { get; set; } = 0;
        // 📌 सॉफ्ट डिलीट के लिए: अगर true है तो माल डिलीट माना जाएगा, पर DB में रहेगा
        public bool IsDeleted { get; set; } = false;

    }
}
