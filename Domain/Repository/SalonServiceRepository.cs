using Domain.Data;
using Domain.DTOs;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class SalonServiceRepository(DataContext context)
{
    public async Task<IEnumerable<SalonServiceResponse>> Get() => await context.SalonServices.Where(x => x.IsActive).AsNoTracking().Select(x => new SalonServiceResponse(x.Id, x.Name, x.Price, x.Duration)).ToListAsync();

    public async Task<SalonServiceResponse?> Get(Guid id) {
        var x = await context.SalonServices.FirstOrDefaultAsync(x => x.IsActive && x.Id == id);
        if (x is null) return null;
        return new SalonServiceResponse(x.Id, x.Name, x.Price, x.Duration);
        }
}