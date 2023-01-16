using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers;
public class DashboardController : Controller
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPhotoService _photoService;

    public DashboardController(IDashboardRepository dashboardRepository, IHttpContextAccessor httpContextAccessor, IPhotoService photoService)
    {
        _dashboardRepository = dashboardRepository;
        _httpContextAccessor = httpContextAccessor;
        _photoService = photoService;
    }

    private void MapUserUpdate(AppUser user, UpdateUserDashboardViewModel updateVM, ImageUploadResult photoResult)
    {
        user.Id = updateVM.Id;
        user.Pace = updateVM.Pace;
        user.Mileage = updateVM.Mileage;
        user.ProfileImageUrl = photoResult.Url.ToString();
        user.City = updateVM.City;
        user.State = updateVM.State;
    }

    public async Task<IActionResult> Index()
    {
        var userRaces = await _dashboardRepository.GetAllUserRaces();
        var userClubs = await _dashboardRepository.GetAllUserClubs();
        var dashboardViewModel = new DashboardViewModel()
        {
            Races = userRaces,
            Clubs = userClubs,
        };
        return View(dashboardViewModel);
    }

    public async Task<IActionResult> UpdateUserProfile()
    {
        var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
        var user = await _dashboardRepository.GetUserById(curUserId);

        if (user == null)
        {
            return View("Error");
        }

        var updateUserViewModel = new UpdateUserDashboardViewModel()
        {
            Id = curUserId,
            Pace = user.Pace,
            Mileage = user.Mileage,
            ProfileImageUrl = user.ProfileImageUrl,
            City = user.City,
            State = user.State
        };
        return View(updateUserViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserProfile(UpdateUserDashboardViewModel updateVM)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit profile");
            return View("UpdateUserProfile", updateVM);
        }

        var user = await _dashboardRepository.GetUserByIdNoTracking(updateVM.Id);

        if (String.IsNullOrEmpty(user.ProfileImageUrl))
        {
            var photoResult = await _photoService.AddPhotoAsync(updateVM.Image);

            MapUserUpdate(user, updateVM, photoResult);

            _dashboardRepository.Update(user);

            return RedirectToAction("Index");
        } else
        {
            try
            {
                await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
            } catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete photo");
                return View(updateVM);
            }
            var photoResult = await _photoService.AddPhotoAsync(updateVM.Image);

            MapUserUpdate(user, updateVM, photoResult);

            _dashboardRepository.Update(user);

            return RedirectToAction("Index");
        }
    }
}
