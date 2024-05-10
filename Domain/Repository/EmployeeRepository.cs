using Domain.Data;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class EmployeeRepository(DataContext context)
{
    public async Task<List<Employee>> Get() => await context.Employees.ToListAsync();

}

