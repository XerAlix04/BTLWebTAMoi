using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Services;
using Project1.Models.ViewModels;
using System.Text.Json;

namespace Project1.Controllers
{
    public class FlashcardsController : Controller
    {
        private readonly FlashcardsService _service;
        public FlashcardsController(FlashcardsService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var pagedFlashcards = await _service.GetUserFlashcardsPagedAsync(page, pageSize);
            if (pagedFlashcards == null)
            {
                // You might want to return an error view or an empty list
                return View(new ApiPagedResponse<TuVungVM>(new List<TuVungVM>(), 0, page, pageSize));
            }
            return View(pagedFlashcards);
        }

        public async Task<IActionResult> View(int id)
        {
            var flashcard = await _service.GetFlashcardAsync(id);
            if (flashcard == null)
            {
                return NotFound();
            }
            return View(flashcard);
        }

        public IActionResult Practice()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StartPractice()
        {
            var flashcards = await _service.GetAllUserFlashcardsAsync();
            if (!flashcards.Any())
            {
                TempData["Error"] = "No flashcards available for practice.";
                return RedirectToAction("Index");
            }

            // Initialize practice session
            var practiceSession = new PracticeSessionVM
            {
                AllFlashcards = flashcards,
                RemainingWords = new List<PracticeWord>(),
                CompletedWords = new List<PracticeWord>(),
                CurrentPhase = PracticePhase.InitialReview,
                CurrentWordIndex = 0,
                TotalWords = Math.Min(10, flashcards.Count)
            };

            // Select initial 10 random words
            var randomWords = flashcards.OrderBy(x => Guid.NewGuid()).Take(practiceSession.TotalWords).ToList();
            foreach (var word in randomWords)
            {
                practiceSession.RemainingWords.Add(new PracticeWord
                {
                    Word = word,
                    Difficulty = null
                });
            }

            // Store session (you'll need to implement session storage)
            StorePracticeSession(practiceSession);

            return RedirectToAction("PracticeWord");
        }

        public IActionResult PracticeWord()
        {
            var session = GetPracticeSession();
            if (session == null || !session.RemainingWords.Any())
            {
                return RedirectToAction("PracticeComplete");
            }

            var currentWord = session.RemainingWords.First();
            return View(currentWord);
        }

        [HttpPost]
        public IActionResult SubmitDifficulty(string difficulty)
        {
            var session = GetPracticeSession();
            if (session != null && session.RemainingWords.Any())
            {
                var currentWord = session.RemainingWords.First();
                currentWord.Difficulty = difficulty;
                session.CompletedWords.Add(currentWord);
                session.RemainingWords.RemoveAt(0);

                // If initial review complete, setup exercises
                if (session.CurrentPhase == PracticePhase.InitialReview &&
                    session.CompletedWords.Count >= session.TotalWords)
                {
                    SetupExercises(session);
                }

                StorePracticeSession(session);
            }

            return RedirectToAction("PracticeWord");
        }

        public IActionResult SubmitExerciseAnswer(int wordId, string answer)
        {
            var session = GetPracticeSession();
            if (session != null && session.RemainingWords.Any())
            {
                var currentExercise = session.RemainingWords.First();
                currentExercise.UserAnswer = answer;
                session.CompletedWords.Add(currentExercise);
                session.RemainingWords.RemoveAt(0);

                // Check if all exercises are complete
                if (!session.RemainingWords.Any())
                {
                    return RedirectToAction("PracticeComplete");
                }

                StorePracticeSession(session);
            }

            return RedirectToAction("PracticeWord");
        }

        public IActionResult PracticeComplete()
        {
            var session = GetPracticeSession();
            ClearPracticeSession();
            return View(session);
        }

        // Helper methods for session management
        private void StorePracticeSession(PracticeSessionVM session)
        {
            HttpContext.Session.SetString("PracticeSession", JsonSerializer.Serialize(session));
        }

        private PracticeSessionVM GetPracticeSession()
        {
            var sessionJson = HttpContext.Session.GetString("PracticeSession");
            return sessionJson == null ? null : JsonSerializer.Deserialize<PracticeSessionVM>(sessionJson);
        }

        private void ClearPracticeSession()
        {
            HttpContext.Session.Remove("PracticeSession");
        }

        private void SetupExercises(PracticeSessionVM session)
        {
            session.CurrentPhase = PracticePhase.Exercises;
            var mediumHardWords = session.CompletedWords
                .Where(w => w.Difficulty == "Medium" || w.Difficulty == "Hard")
                .Select(w => w.Word)
                .ToList();

            if (!mediumHardWords.Any())
            {
                // If no medium/hard words, use some easy words
                mediumHardWords = session.CompletedWords
                    .Take(Math.Min(3, session.CompletedWords.Count))
                    .Select(w => w.Word)
                    .ToList();
            }

            session.RemainingWords = new List<PracticeWord>();
            var random = new Random();

            foreach (var word in mediumHardWords)
            {
                var exerciseType = string.IsNullOrEmpty(word.ViDu) ?
                ExerciseType.MultipleChoice :
                (random.Next(2) == 0 ? ExerciseType.MultipleChoice : ExerciseType.FillInBlank);

                var exercise = new PracticeWord
                {
                    Word = word,
                    ExerciseType = exerciseType
                };

                // Generate choices for multiple choice exercises
                if (exerciseType == ExerciseType.MultipleChoice)
                {
                    exercise.Choices = GenerateMultipleChoiceOptions(word, mediumHardWords, random);
                }

                session.RemainingWords.Add(exercise);
            }
        }

        private List<string> GenerateMultipleChoiceOptions(TuVungVM correctWord, List<TuVungVM> allWords, Random random)
        {
            var options = new List<string> { correctWord.Nghia };

            // Get other words for incorrect options
            var otherWords = allWords
                .Where(w => w.MaTu != correctWord.MaTu)
                .ToList();

            // If we don't have enough other words, use some from completed words or repeat
            while (options.Count < 4 && otherWords.Any())
            {
                var randomWord = otherWords[random.Next(otherWords.Count)];
                if (!options.Contains(randomWord.Nghia))
                {
                    options.Add(randomWord.Nghia);
                }
            }

            // If we still don't have enough options, add some generic ones
            var genericOptions = new List<string> { "không biết", "cần tra cứu", "chưa học" };
            while (options.Count < 4)
            {
                var genericOption = genericOptions[random.Next(genericOptions.Count)];
                if (!options.Contains(genericOption))
                {
                    options.Add(genericOption);
                }
            }

            return options.OrderBy(x => random.Next()).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> Add(int tuVungId)
        {
            var flashcard = await _service.AddFlashcardAsync(tuVungId);
            if (flashcard == null)
                return BadRequest("Could not add flashcard.");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int tuVungId)
        {
            var success = await _service.RemoveFlashcardAsync(tuVungId);
            if (!success)
                return BadRequest("Could not remove flashcard.");
            return RedirectToAction("Index");
        }


    }
}
