using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using System;
using System.Linq;


namespace Project1.Controllers
{
    public class RegisterController : Controller
    {
        private readonly WebTaContext db = new WebTaContext();

        // GET: /Auth/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng username
                var existingUser = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == model.TenDangNhap);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại!");
                    return View(model);
                }

                // Gán ngày tạo
                model.NgayTao = DateTime.Now;

                // ⚠️ Trong thực tế nên Hash mật khẩu (BCrypt, SHA256,...)
                db.NguoiDungs.Add(model);
                db.SaveChanges();

                // Redirect sang Login
                return RedirectToAction("Login", "Auth");
            }

            return View(model);
        }
    }
}
