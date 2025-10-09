using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Project1.Controllers
{
    public class SpeechController : Controller
    {
        // 🔑 Thông tin Azure
        private readonly string _azureKey = "EPW0olvc2ZxkUTQyrp3y2bReCMT5KSmdm91jm6ulDYeRmNikFnDTJQQJ99BJAC3pKaRXJ3w3AAAYACOGFnFJ";
        private readonly string _azureRegion = "eastasia";

        // 📘 Danh sách từ luyện phát âm
        private static readonly List<string> WordList = new()
        {
            "apple", "banana", "computer", "friend", "music",
            "teacher", "beautiful", "weather", "language", "travel",
            "university", "family", "restaurant", "library", "holiday",
            "coffee", "morning", "chocolate", "elephant", "information"
        };

        // 🎲 Lấy ngẫu nhiên 1 từ
        private string GetRandomWord()
        {
            var rand = new Random();
            return WordList[rand.Next(WordList.Count)];
        }

        /// <summary>
        /// 🔹 Hiển thị giao diện luyện nói
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            string randomWord = GetRandomWord();
            ViewBag.Word = randomWord;
            ViewBag.ReferenceText = randomWord;
            return View();
        }

        /// <summary>
        /// 🔹 API trả JSON khi gọi fetch để đổi từ
        /// </summary>
        [HttpGet]
        [Route("Speech/Index")]
        public IActionResult GetRandomWordJson()
        {
            string randomWord = GetRandomWord();
            return Json(new { randomWord });
        }

        /// <summary>
        /// 📦 Gửi file âm thanh + referenceText để Azure chấm điểm
        /// </summary>
        [HttpPost]
        [Route("Speech/Check")]
        public async Task<IActionResult> Check(IFormFile audio, string referenceText)
        {
            if (audio == null || audio.Length == 0)
                return BadRequest("❌ Không có file âm thanh được tải lên.");

            if (string.IsNullOrWhiteSpace(referenceText))
                return BadRequest("❌ Thiếu referenceText — không biết bạn đang đọc từ nào.");

            // ✅ B1: Lưu file gốc
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/audio");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var originalFilePath = Path.Combine(uploadsFolder, Path.GetFileName(audio.FileName));
            using (var stream = new FileStream(originalFilePath, FileMode.Create))
            {
                await audio.CopyToAsync(stream);
            }

            // ✅ B2: Chuyển sang WAV PCM 16kHz
            var wavFilePath = Path.Combine(
                uploadsFolder,
                Path.GetFileNameWithoutExtension(originalFilePath) + "_converted.wav"
            );
            ConvertToWavPcm16(originalFilePath, wavFilePath);

            // ✅ B3: Gửi sang Azure chấm điểm
            var result = await AssessPronunciationAsync(wavFilePath, referenceText);

            ViewBag.ScoreResult = result;
            ViewBag.AudioFile = "/audio/" + Path.GetFileName(wavFilePath);
            ViewBag.RandomWord = referenceText;

            return View("Result");
        }

        // 🔉 Chuyển file về chuẩn WAV PCM16 16kHz mono
        private void ConvertToWavPcm16(string inputPath, string outputPath)
        {
            using var reader = new AudioFileReader(inputPath);
            var newFormat = new WaveFormat(16000, 16, 1);
            using var conversionStream = new MediaFoundationResampler(reader, newFormat);
            WaveFileWriter.CreateWaveFile(outputPath, conversionStream);
        }

        // 🧠 Chấm phát âm bằng Azure Speech
        private async Task<string> AssessPronunciationAsync(string audioPath, string referenceText)
        {
            var speechConfig = SpeechConfig.FromSubscription(_azureKey, _azureRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";

            using var audioConfig = AudioConfig.FromWavFileInput(audioPath);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var pronunciationConfig = new PronunciationAssessmentConfig(
                referenceText,
                GradingSystem.HundredMark,
                Granularity.Phoneme,
                enableMiscue: true
            );
            pronunciationConfig.ApplyTo(recognizer);

            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                var pronResult = PronunciationAssessmentResult.FromResult(result);
                return $@"
                    <b>🔹 Từ cần đọc:</b> {referenceText}<br/>
                    <b>🗣️ Bạn phát âm:</b> {result.Text}<br/>
                    <b>🎯 Điểm tổng:</b> {pronResult.PronunciationScore:F1}<br/>
                    <b>Accuracy:</b> {pronResult.AccuracyScore:F1}<br/>
                    <b>Fluency:</b> {pronResult.FluencyScore:F1}<br/>
                    <b>Completeness:</b> {pronResult.CompletenessScore:F1}
                ";
            }
            else
            {
                return $"❌ Không nhận diện được giọng nói. Lý do: {result.Reason}";
            }
        }
    }
}
