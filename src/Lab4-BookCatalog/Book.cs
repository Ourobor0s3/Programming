namespace Lab4_BookCatalog
{
    [Serializable]
    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int? Year { get; set; }
        public int? Pages { get; set; }
    }
}