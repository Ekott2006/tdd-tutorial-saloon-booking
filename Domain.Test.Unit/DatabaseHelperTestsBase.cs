using Domain.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Domain.Test.Unit;

public class DatabaseHelperTestsBase: IDisposable
{
    private readonly SqliteConnection _connection;
    protected DataContext Context { get; set; }

    protected DatabaseHelperTestsBase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        DbContextOptions<DataContext> options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(_connection)
            .Options;
        Context = new DataContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose() {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}