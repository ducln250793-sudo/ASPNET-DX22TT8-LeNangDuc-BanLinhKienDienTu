using Microsoft.AspNetCore.Mvc;
using PhonePartsStore.Data;
using PhonePartsStore.Models;

namespace PhonePartsStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin" && role != "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            var categories = _context.Categories.ToList();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin" && role != "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin" && role != "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin" && role != "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid){
                return View(category);
            }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ActionName("ToggleStatusAdmin")]
        public IActionResult ToggleStatus(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin" && role != "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            category.IsActive = !category.IsActive;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin" && role != "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var existing = await _context.Categories.FindAsync(category.Id);
            if (existing == null)
                return NotFound();

            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.IsActive = category.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin" && role != "Manager")
            {
                return RedirectToAction("Index", "Login");
            }

            if ( id == 0)
            {
                return NotFound();
            }
            var category = _context.Categories.Find(id);
            if(category == null)
            {
                return NotFound();
            }
            
            bool hasRelatedProducts = _context.Products.Any(p => p.CategoryId == id);
            if(hasRelatedProducts)
            {
                return BadRequest();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
