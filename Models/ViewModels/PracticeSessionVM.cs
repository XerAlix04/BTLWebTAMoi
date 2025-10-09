using Project1.Models;

namespace Project1.Models.ViewModels
{
    public class PracticeSessionVM
    {
        public List<TuVungVM> AllFlashcards { get; set; } = new();
        public List<PracticeWord> RemainingWords { get; set; } = new();
        public List<PracticeWord> CompletedWords { get; set; } = new();
        public PracticePhase CurrentPhase { get; set; }
        public int CurrentWordIndex { get; set; }
        public int TotalWords { get; set; }

        // Helper method to get next word
        public PracticeWord GetNextWord()
        {
            if (RemainingWords.Any())
            {
                var nextWord = RemainingWords[0];
                RemainingWords.RemoveAt(0);
                return nextWord;
            }
            return null;
        }

        // Helper method to add word to queue
        public void AddWordToQueue(PracticeWord word)
        {
            RemainingWords.Add(word);
        }
    }
    public class PracticeWord
    {
        public TuVungVM Word { get; set; }
        public string Difficulty { get; set; }
        public ExerciseType ExerciseType { get; set; }
        public List<string> Choices { get; set; } = new();
        public string UserAnswer { get; set; }
    }

    public enum PracticePhase
    {
        InitialReview,
        Exercises
    }

    public enum ExerciseType
    {
        MultipleChoice,
        FillInBlank
    }
}
