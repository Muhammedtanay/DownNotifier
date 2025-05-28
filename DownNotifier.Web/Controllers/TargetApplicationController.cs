using DownNotifier.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DownNotifier.Web.Controllers
{
    [Authorize]
    public class TargetApplicationController(ITargetApplicationService _targetApplicationService, UserManager<AppUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var targetApplicationData = await _targetApplicationService.GetAllAsync(userId);
            return View(targetApplicationData);
        }
        public async Task<IActionResult> Details(int id)
        {
            var targetApplicationData = await _targetApplicationService.GetByIdAsync(id);
            if (targetApplicationData == null) return NotFound();
            return View(targetApplicationData);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TargetApplicationViewModel targetApplicationViewModel)
        {
            if (!ModelState.IsValid) return View(targetApplicationViewModel);

            var userId = _userManager.GetUserId(User); 
            await _targetApplicationService.AddAsync(targetApplicationViewModel, userId);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var targetApplicationData = await _targetApplicationService.GetByIdAsync(id);
            if (targetApplicationData == null) return NotFound();
            return View(targetApplicationData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TargetApplicationViewModel targetApplicationViewModel)
        {
            if (id != targetApplicationViewModel.Id) return BadRequest();

            if (!ModelState.IsValid) return View(targetApplicationViewModel);
            var userId = _userManager.GetUserId(User);
            await _targetApplicationService.UpdateAsync(targetApplicationViewModel,userId);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var targetApplicationData = await _targetApplicationService.GetByIdAsync(id);
            if (targetApplicationData == null) return NotFound();
            return View(targetApplicationData);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _targetApplicationService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
