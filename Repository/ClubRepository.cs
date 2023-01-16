using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository;

public class ClubRepository : IClubRepository
{
    private readonly ApplicationDbContext _db;

    public ClubRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public bool Add(Club club)
    {
        _db.Add(club);
        return Save();
    }

    public bool Delete(Club club)
    {
        _db.Remove(club);
        return Save();
    }

    public async Task<IEnumerable<Club>> GetAll()
    {
        return await _db.Clubs.ToListAsync();
    }

    public async Task<Club> GetByIdAsync(int id)
    {
        return await _db.Clubs
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Club> GetByIdAsyncAsNoTracking(int id)
    {
        return await _db.Clubs
            .Include(c => c.Address)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<IEnumerable<Club>> GetClubByCity(string city)
    {
        return await _db.Clubs.Where(c => c.Address.City.Contains(city)).ToListAsync();
    }

    public bool Save()
    {
        var saved = _db.SaveChanges();
        return saved > 0;
    }

    public bool Update(Club club)
    {
        _db.Update(club);
        return Save();
    }
}
