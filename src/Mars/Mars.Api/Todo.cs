using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Mars.Api;

public class Todo : IIdentifiable
{
    [Key] // key is required for mongo
    public Guid Id { get; set; }

    public string Content { get; set; } = "";
    public bool IsCompleted { get; set; }
}

public class MongoDbRepository<TModel> : IRepository<TModel>
    where TModel : class, IIdentifiable
{
    private readonly MarsDb _db;

    public MongoDbRepository(MarsDb db)
    {
        _db = db;
    }

    public async Task DeleteAsync(Guid id)
    {
        var model = await _db.Set<TModel>().FindAsync(id);
        if (model == null) return;
        _db.Set<TModel>().Remove(model);
        await _db.SaveChangesAsync();
    }

    public async Task<List<TModel>> GetListAsync()
    {
        return await _db.Set<TModel>().ToListAsync();
    }

    public async Task<TModel?> GetSingleAsync(Guid id)
    {
        return await _db.Set<TModel>().FindAsync(id);
    }

    public async Task InsertAsync(TModel model)
    {
        model.Id = Guid.NewGuid();
        await _db.Set<TModel>().AddAsync(model);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, TModel model)
    {
        _db.Update(model);
        await _db.SaveChangesAsync();
    }
}

public interface IIdentifiable
{
    Guid Id { get; set; }
}

public interface IRepository<TModel>
    where TModel : class, IIdentifiable
{
    Task<List<TModel>> GetListAsync();

    Task<TModel> GetSingleAsync(Guid id);

    Task InsertAsync(TModel model);

    Task UpdateAsync(Guid id, TModel model);

    Task DeleteAsync(Guid id);
}