using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DashboardRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<List<Club>> GetAllUserClubs()
    {
        var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
        var userClubs = _db.Clubs.Where(r => r.AppUser.Id == curUser.ToString());
        return userClubs.ToList();
    }

    public async Task<List<Race>> GetAllUserRaces()
    {
        var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
        var userRaces = _db.Races.Where(r => r.AppUser.Id == curUser.ToString());
        return userRaces.ToList();
    }
    public async Task<AppUser> GetUserById(string id)
    {
        return await _db.Users.FindAsync(id);
    }
    public async Task<AppUser> GetUserByIdNoTracking(string id)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public bool Update(AppUser user)
    {
        _db.Users.Update(user);
        return Save();
    }

    public bool Save()
    {
        var saved = _db.SaveChanges();
        return saved > 0;
    }
}
