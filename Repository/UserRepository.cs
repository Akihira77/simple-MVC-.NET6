using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public bool Add(AppUser user)
    {
        throw new NotImplementedException();
    }

    public bool Delete(AppUser user)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<AppUser>> GetAllUsers()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<AppUser> GetUserById(string id)
    {
        return await _db.Users.FindAsync(id);
    }

    public bool Save()
    {
        var saved = _db.SaveChanges();
        return saved > 0;
    }

    public bool Update(AppUser user)
    {
        _db.Update(user);
        return Save();
    }
}
