using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers;
public class ClubController : Controller
{
    private readonly IClubRepository _clubRepository;
    private readonly IPhotoService _photoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
    {
        _clubRepository = clubRepository;
        _photoService = photoService;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IActionResult> Index()
    {
        var clubs = await _clubRepository.GetAll();
        return View(clubs);
    }

    public async Task<IActionResult> Details(int id)
    {
        var obj = await _clubRepository.GetByIdAsync(id);
        return View(obj);
    }

    public IActionResult Create()
    {
        var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
        var createClubViewModel = new CreateClubViewModel
        {
            AppUserId = curUserId
        };
        return View(createClubViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClubViewModel clubVM)
    {
        if (ModelState.IsValid)
        {
            var result = await _photoService.AddPhotoAsync(clubVM.Image);
            var club = new Club
            {
                Title = clubVM.Title,
                Description = clubVM.Description,
                Image = result.Url.ToString(),
                AppUserId = clubVM.AppUserId,
                Address = new Address
                {
                    City = clubVM.Address.City,
                    State = clubVM.Address.State,
                    Street = clubVM.Address.Street,
                }
            };
            _clubRepository.Add(club);
            return RedirectToAction("Index");
        } else
        {
            ModelState.AddModelError("", "Photo upload failed");
        }

        return View(clubVM);
    }

    public async Task<IActionResult> Update(int id)
    {
        var obj = await _clubRepository.GetByIdAsync(id);
        if (obj == null)
        {
            return NotFound();
        }
        var clubVM = new UpdateClubViewModel
        {
            Title = obj.Title,
            Description = obj.Description,
            AddressId = obj.AddressId,
            Address = obj.Address,
            URL = obj.Image,
            ClubCategory = obj.ClubCategory
        };
        return View(clubVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, UpdateClubViewModel clubVM)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit club");
            return View("Update", clubVM);
        }

        var userClub = await _clubRepository.GetByIdAsyncAsNoTracking(id);
        if (userClub != null)
        {
            try
            {
                await _photoService.DeletePhotoAsync(userClub.Image);
            } catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete photo");
                return View(clubVM);
            }
            var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

            var club = new Club
            {
                Id = id,
                Title = clubVM.Title,
                Description = clubVM.Description,
                Address = clubVM.Address,
                Image = photoResult.Url.ToString(),
                ClubCategory = clubVM.ClubCategory,
                AppUserId = userClub.AppUserId,
                AddressId = clubVM.AddressId
            };
            _clubRepository.Update(club);
            return RedirectToAction("Index");
        }
        return View(clubVM);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var obj = await _clubRepository.GetByIdAsync(id);
        if (obj == null)
        {
            return View("Error");
        }
        return View(obj);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Club obj)
    {
        _clubRepository.Delete(obj);
        return RedirectToAction("Index");
    }
}
