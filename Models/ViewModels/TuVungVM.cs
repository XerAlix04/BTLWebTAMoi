namespace Project1.Models.ViewModels
{
    public class TuVungVM
    {
        public int MaTu { get; set; }
        public string Tu { get; set; } = string.Empty;
        public string Nghia { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public string? ViDu { get; set; }
        // Add only what you need for the flashcard view
    }
}
