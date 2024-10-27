namespace churchWebAPI.Models
{
    public class clsBibleVerse
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
        public string VerseText { get; set; }
        public int TranslationId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }

    public class BibleVerseV2
    {
        public int Id { get; set; }
        public string Verse { get; set; }
        public string English { get; set; }
        public string Tamil { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
