using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models; // đổi namespace cho đúng
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly WebTaContext _context;

        public ExamController(WebTaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách câu hỏi (ngẫu nhiên)
        /// GET: /api/Exam/GetQuestions?count=10
        /// </summary>
        [HttpGet("GetQuestions")]
        public async Task<IActionResult> GetQuestions([FromQuery] int count = 10)
        {
            var questions = await _context.CauHois
                .Include(q => q.LuaChons)
                .OrderBy(r => Guid.NewGuid()) // random
                .Take(count)
                .Select(q => new
                {
                    ma_cau = q.MaCau,
                    noi_dung = q.NoiDung,
                    luaChon = q.LuaChons.Select(lc => new
                    {
                        ma = lc.Ma,
                        noi_dung = lc.NoiDung,
                        la_dap_an = lc.LaDapAn
                    })
                })
                .ToListAsync();

            return Ok(questions);
        }

        /// <summary>
        /// Gửi kết quả bài thi
        /// POST: /api/Exam/SubmitQuiz
        /// </summary>
        [HttpPost("SubmitQuiz")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizResultModel model)
        {
            if (model == null) return BadRequest("Không có dữ liệu gửi lên");

            //var entity = new KetQuaBaiThi
            //{
            //    MaNguoiDung = model.UserId ?? "guest",
            //    MaDe = 1, // giả sử đề mặc định
            //    Diem = model.Correct,
            //    ChiTiet = System.Text.Json.JsonSerializer.Serialize(model.Details),
            //    NgayTao = DateTime.Now
            //};
            var entity = new KetQuaBaiThi
            {
                MaNguoiDung = int.TryParse(model.UserId, out int userId) ? userId : 0, // 0 nếu không phải số
                MaDe = 1, // giả sử đang test với đề số 1
                Diem = model.Correct,
                ChiTiet = System.Text.Json.JsonSerializer.Serialize(model.Details),
                NgayTao = DateTime.Now
            };

            _context.KetQuaBaiThis.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã lưu kết quả thành công!" });
        }
    }

    /// <summary>
    /// Dùng để nhận dữ liệu từ JS gửi về
    /// </summary>
    public class QuizResultModel
    {
        public string UserId { get; set; }
        public int Total { get; set; }
        public int Correct { get; set; }
        public List<QuizDetail> Details { get; set; }
    }

    public class QuizDetail
    {
        public int Id { get; set; }
        public string Question { get; set; }
    }
}
