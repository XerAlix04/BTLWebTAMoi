namespace Project1.Models.ViewModels
{
    public class RegisterModel
    {
        public string TenDangNhap { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
    }

    public class LoginModel
    {
        public string TenDangNhap { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
    }
}
