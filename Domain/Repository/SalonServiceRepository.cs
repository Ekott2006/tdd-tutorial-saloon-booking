using Domain.Data;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class HairdressingServiceRepository(DataContext context)
{
    public async Task<IEnumerable<SalonService>> Get() => await context.SalonServices.ToListAsync();
}