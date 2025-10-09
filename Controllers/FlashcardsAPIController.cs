using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using Project1.Models.ViewModels;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashcardsAPIController : ControllerBase
    {
        private readonly WebTaContext _context;
        private readonly ILogger<FlashcardsAPIController> _logger;

        public FlashcardsAPIController(WebTaContext context, ILogger<FlashcardsAPIController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // For demo, use a fixed user ID. Replace with actual authentication.
        private int GetUserId() => 2;

        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<List<TuVungVM>>>> GetAllUserFlashcards()
        {
            var userId = GetUserId();
            var flashcards = await _context.NguoiDungTuVungs
                .Where(x => x.MaNguoiDung == userId)
                .Include(x => x.MaTuNavigation)
                .Select(x => new TuVungVM
                {
                    MaTu = (int)x.MaTu,
                    Tu = x.MaTuNavigation.Tu,
                    Nghia = x.MaTuNavigation.Nghia,
                    HinhAnh = x.MaTuNavigation.DuongDanAnh,
                    ViDu = x.MaTuNavigation.ViDu
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<TuVungVM>>
            {
                Success = true,
                Data = flashcards
            });
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ApiPagedResponse<TuVungVM>>>> GetPagedUserFlashcards([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            var query = _context.NguoiDungTuVungs
                .Where(x => x.MaNguoiDung == userId)
                .Include(x => x.MaTuNavigation)
                .Select(x => new TuVungVM
                {
                    MaTu = (int)x.MaTu,
                    Tu = x.MaTuNavigation.Tu,
                    Nghia = x.MaTuNavigation.Nghia,
                    HinhAnh = x.MaTuNavigation.DuongDanAnh,
                    ViDu = x.MaTuNavigation.ViDu // <-- ADD THIS LINE
                });

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var pagedData = new ApiPagedResponse<TuVungVM>(items, totalCount, page, pageSize);

            return Ok(new ApiResponse<ApiPagedResponse<TuVungVM>>
            {
                Success = true,
                Data = pagedData
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TuVungVM>>> GetFlashcard(int id)
        {
            var userId = GetUserId();
            var ndtv = await _context.NguoiDungTuVungs
                .Include(x => x.MaTuNavigation)
                .FirstOrDefaultAsync(x => x.MaNguoiDung == userId && x.MaTu == id);

            if (ndtv == null)
            {
                return NotFound(new ApiResponse<TuVungVM>
                {
                    Success = false,
                    Message = "Flashcard not found."
                });
            }

            var vm = new TuVungVM
            {
                MaTu = (int)ndtv.MaTu,
                Tu = ndtv.MaTuNavigation.Tu,
                Nghia = ndtv.MaTuNavigation.Nghia,
                HinhAnh = ndtv.MaTuNavigation.DuongDanAnh,
                ViDu = ndtv.MaTuNavigation.ViDu
            };

            return Ok(new ApiResponse<TuVungVM>
            {
                Success = true,
                Data = vm
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TuVungVM>>> AddFlashcard([FromBody] int tuVungId)
        {
            var userId = GetUserId();
            var exists = await _context.NguoiDungTuVungs
                .AnyAsync(x => x.MaNguoiDung == userId && x.MaTu == tuVungId);

            if (exists)
            {
                return BadRequest(new ApiResponse<TuVungVM>
                {
                    Success = false,
                    Message = "Flashcard already exists."
                });
            }

            var tuVung = await _context.TuVungs.FindAsync(tuVungId);
            if (tuVung == null)
            {
                return NotFound(new ApiResponse<TuVungVM>
                {
                    Success = false,
                    Message = "Vocabulary not found."
                });
            }

            var ndtv = new NguoiDungTuVung
            {
                MaNguoiDung = userId,
                MaTu = tuVungId,
                NgayOnTapCuoi = DateTime.Now
            };
            _context.NguoiDungTuVungs.Add(ndtv);
            await _context.SaveChangesAsync();

            var vm = new TuVungVM
            {
                MaTu = tuVung.MaTu,
                Tu = tuVung.Tu,
                Nghia = tuVung.Nghia,
                HinhAnh = tuVung.DuongDanAnh,
                ViDu = tuVung.ViDu
            };

            return Ok(new ApiResponse<TuVungVM>
            {
                Success = true,
                Data = vm
            });
        }

        [HttpDelete("{tuVungId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveFlashcard(int tuVungId)
        {
            var userId = GetUserId();
            var ndtv = await _context.NguoiDungTuVungs
                .FirstOrDefaultAsync(x => x.MaNguoiDung == userId && x.MaTu == tuVungId);

            if (ndtv == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Flashcard not found."
                });
            }

            _context.NguoiDungTuVungs.Remove(ndtv);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Flashcard removed."
            });
        }
    }
}
