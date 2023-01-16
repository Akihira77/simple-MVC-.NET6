using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
using RunGroopWebApp.Services;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers;
public class RaceController : Controller
{
    private readonly IRaceRepository _raceRepository;
    private readonly IPhotoService _photoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
    {
        _raceRepository = raceRepository;
        _photoService = photoService;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IActionResult> Index()
    {
        var races = await _raceRepository.GetAll();
        return View(races);
    }

    public async Task<IActionResult> Details(int id)
    {
        var obj = await _raceRepository.GetByIdAsync(id);
        return View(obj);
    }

    public IActionResult Create()
    {
        var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
        var createRaceViewModel = new CreateRaceViewModel { AppUserId = curUserId };
        return View(createRaceViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
    {
        if (ModelState.IsValid)
        {
            var result = await _photoService.AddPhotoAsync(raceVM.Image);
            var race = new Race
            {
                Title = raceVM.Title,
                Description = raceVM.Description,
                Image = result.Url.ToString(),
                AppUserId = raceVM.AppUserId,
                Address = new Address
                {
                    City = raceVM.Address.City,
                    State = raceVM.Address.State,
                    Street = raceVM.Address.Street,
                }
            };
            _raceRepository.Add(race);
            return RedirectToAction("Index");
        } else
        {
            ModelState.AddModelError("", "Photo upload failed");
        }

        return View(raceVM);
    }

    public async Task<IActionResult> Update(int id)
    {
        var obj = await _raceRepository.GetByIdAsync(id);
        if (obj == null)
        {
            return NotFound();
        }
        var raceVM = new UpdateRaceViewModel
        {
            Title = obj.Title,
            Description = obj.Description,
            AddressId = obj.AddressId,
            Address = obj.Address,
            URL = obj.Image,
            RaceCategory = obj.RaceCategory
        };
        return View(raceVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, UpdateRaceViewModel raceVM)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit club");
            return View("Update", raceVM);
        }

        var userRace = await _raceRepository.GetByIdAsyncAsNoTracking(id);
        if (userRace != null)
        {
            try
            {
                await _photoService.DeletePhotoAsync(userRace.Image);
            } catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete photo");
                return View(raceVM);
            }
            var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);

            var race = new Race
            {
                Id = id,
                Title = raceVM.Title,
                Description = raceVM.Description,
                Address = raceVM.Address,
                Image = photoResult.Url.ToString(),
                RaceCategory = raceVM.RaceCategory,
                AppUserId = userRace.AppUserId,
                AddressId = raceVM.AddressId
            };
            _raceRepository.Update(race);
            return RedirectToAction("Index");
        }
        return View(raceVM);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var obj = await _raceRepository.GetByIdAsync(id);
        if (obj == null)
        {
            return View("Error");
        }
        return View(obj);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Race obj)
    {
        _raceRepository.Delete(obj);
        return RedirectToAction("Index");
    }
}
