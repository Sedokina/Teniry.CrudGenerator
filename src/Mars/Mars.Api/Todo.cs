using System.Reflection;
using Humanizer;
using Mars.Generators;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Mars.Api;

[GenerateService("RepositoryController.txt")]
public class Todo : IIdentifiable
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public bool IsCompleted { get; set; }
}

public class MongoDbOptions
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017/" + Assembly.GetEntryAssembly()?.GetName().Name?.Replace(".", "_");
}

public class MongoDbRepository<TModel> : IRepository<TModel>
    where TModel : class, IIdentifiable
{
    protected MongoDbOptions Options { get; init; }

    protected IMongoCollection<TModel> Collection { get; init; }

    public MongoDbRepository(IOptions<MongoDbOptions> options)
    {
        Options = options.Value;
        var connectionUri = new Uri(options.Value.ConnectionString);
        var client = new MongoClient(Options.ConnectionString);
        Collection = client
            .GetDatabase(connectionUri.AbsolutePath.Trim('/'))
            .GetCollection<TModel>(typeof(TModel).Name.Pluralize());
    }

    public Task DeleteAsync(Guid id)
    {
        return Collection.DeleteOneAsync(x => x.Id == id);
    }

    public Task<List<TModel>> GetListAsync()
    {
        return Collection.AsQueryable().ToListAsync();
    }

    public Task<TModel?> GetSingleAsync(Guid id)
    {
        return Task.FromResult(Collection.AsQueryable().FirstOrDefault(x => x.Id == id));
    }

    public Task InsertAsync(TModel model)
    {
        model.Id = Guid.NewGuid();
        return Collection.InsertOneAsync(model);
    }

    public Task UpdateAsync(Guid id, TModel model)
    {
        model.Id = id;
        return Collection.ReplaceOneAsync(x => x.Id == id, model);
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