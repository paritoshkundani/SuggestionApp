using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess;

public class MongoCategoryData : ICategoryData
{
    private readonly IMongoCollection<CategoryModel> _categories;
    private readonly IMemoryCache _cache;
    private const string cacheName = "CategoryData";

    public MongoCategoryData(IDbConnection db, IMemoryCache cache)
    {
        _cache = cache;
        _categories = db.CategoryCollection;
    }

    /*
     * TC mentions another approach called Lazy Caching, did not show it. Basically if multiple calls come here and
     * nothing is cached yet, all would call db and cache the same info, lazy cache apporach would make the requests
     * queue up, so only the first go to db and add to cache then others would be able to try and there would be cache
     * already, incase the call is expensive that is another approach.
     */
    public async Task<List<CategoryModel>> GetAllCategories()
    {
        var output = _cache.Get<List<CategoryModel>>(cacheName);

        if (output is null)
        {
            var results = await _categories.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(cacheName, output, TimeSpan.FromDays(1));
        }

        return output;
    }

    public Task CreateCategory(CategoryModel category)
    {
        return _categories.InsertOneAsync(category);
    }
}
