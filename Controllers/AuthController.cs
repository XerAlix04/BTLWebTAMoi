using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Models.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace Project1.Controllers
{
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly WebTaContext _context;

        public AuthController(WebTaContext context)
        {
            _context = context;
        }

        // 🟢 API: POST /Auth/Register
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TenDangNhap) ||
                string.IsNullOrWhiteSpace(model.MatKhau) ||
                string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new { success = false, message = "Thiếu thông tin." });
            }

            // Kiểm tra trùng tên đăng nhập
            var existing = _context.NguoiDungs
                .FirstOrDefault(u => u.TenDangNhap == model.TenDangNhap || u.Email == model.Email);
            if (existing != null)
            {
                return Conflict(new { success = false, message = "Tên đăng nhập hoặc email đã tồn tại." });
            }

            // Hash mật khẩu (SHA256 để đơn giản, nên dùng BCrypt trong thực tế)
            string hashed = HashPassword(model.MatKhau);

            var user = new NguoiDung
            {
                TenDangNhap = model.TenDangNhap,
                Email = model.Email,
                MatKhau = hashed,
                NgonNguUaThich = "vi",
                NgayTao = DateTime.Now
            };

            _context.NguoiDungs.Add(user);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Đăng ký thành công!" });
        }

        // 🟢 API: POST /Auth/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TenDangNhap) ||
                string.IsNullOrWhiteSpace(model.MatKhau))
            {
                return BadRequest(new { success = false, message = "Thiếu thông tin." });
            }

            string hashed = HashPassword(model.MatKhau);

            var user = _context.NguoiDungs
                .FirstOrDefault(u => u.TenDangNhap == model.TenDangNhap && u.MatKhau == hashed);

            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu." });
            }

            // Tạo session đăng nhập
            HttpContext.Session.SetString("UserName", user.TenDangNhap);
            HttpContext.Session.SetInt32("UserId", user.MaNguoiDung);

            return Ok(new
            {
                success = true,
                message = "Đăng nhập thành công!",
                user 
            });
        }

        // 🟠 Hàm hash mật khẩu
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
