using Microsoft.EntityFrameworkCore;
using Project1.Models;
using Project1.Services;


var builder = WebApplication.CreateBuilder(args);

// ----------------------
// CẤU HÌNH DỊCH VỤ
// ----------------------


// Thêm cache và session (phải có trước AddControllersWithViews)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian session tồn tại
    options.Cookie.HttpOnly = true; // cookie chỉ đọc bằng server
    options.Cookie.IsEssential = true; // bắt buộc cookie hoạt động
});

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<WebTaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ChatbotApiService>();
builder.Services.AddScoped<FlashcardsService>();
builder.Services.AddHttpContextAccessor();

// Make sure configuration is being loaded
var configuration = builder.Configuration;

builder.Services.AddHttpClient(); // <-- MOVE THE LINE HERE

// ----------------------
// TẠO ỨNG DỤNG
// ----------------------
var app = builder.Build();

// ----------------------
// CẤU HÌNH PIPELINE
// ----------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Phải đặt UseSession **trước** UseAuthorization và MapControllerRoute
app.UseSession();

app.UseAuthorization();

// Route mặc định cho MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

app.Run();
