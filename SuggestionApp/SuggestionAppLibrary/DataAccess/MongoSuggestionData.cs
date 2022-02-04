using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess;

public class MongoSuggestionData : ISuggestionData
{
    private readonly IDbConnection _db;
    private readonly IUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<SuggestionModel> _suggestions;
    private const string CacheName = "SuggestionData";

    public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
    {
        _db = db;
        _userData = userData;
        _cache = cache;
        _suggestions = db.SuggestionCollection;
    }

    public async Task<List<SuggestionModel>> GetAllSuggestionsAsync()
    {
        var output = _cache.Get<List<SuggestionModel>>(CacheName);

        if (output is null)
        {
            var result = await _suggestions.FindAsync(s => s.Archived == false);
            output = result.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }

        return output;
    }

    public async Task<List<SuggestionModel>> GetAllApprovedSuggestionsAsync()
    {
        var output = await GetAllSuggestionsAsync();

        return output.Where(x => x.ApprovedForRelease).ToList();
    }

    public async Task<SuggestionModel> GetSuggestion(string id)
    {
        var results = await _suggestions.FindAsync(s => s.Id == id);
        return results.FirstOrDefault();
    }

    public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval()
    {
        var output = await GetAllSuggestionsAsync();
        return output.Where(x => x.ApprovedForRelease == false && x.Rejected == false).ToList();
    }

    public async Task UpdateSuggestion(SuggestionModel suggestion)
    {
        await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
        // TC removed all items from cache since we just updated one, but could have changed it to remove the one that was just updated
        _cache.Remove(CacheName);
    }

    public async Task UpvoteSuggestion(string suggestionId, string userId)
    {
        var client = _db.Client;

        // starting a transaction as we are updating multiple collections
        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            // we have to get the database this way in order to do the transaction rather than using _db
            var db = client.GetDatabase(_db.DbName);
            var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectioName);
            var suggestion = (await suggestionsInTransaction.FindAsync(s => s.Id == suggestionId)).First(); // we want existing one so don't use FirstOrDefault, if null let it throw exception

            bool isUpvote = suggestion.UserVotes.Add(userId);

            // adding to hashset, if hashset does not add it (returns false) means it's already there, so assume that means someone is trying to undo the vote
            if (isUpvote == false)
            {
                suggestion.UserVotes.Remove(userId);
            }

            await suggestionsInTransaction.ReplaceOneAsync(s => s.Id == suggestionId, suggestion);

            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUser(suggestion.Author.Id);

            if (isUpvote)
            {
                user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
            }
            else
            {
                var suggestionToRemove = user.VotedOnSuggestions.Where(s => s.Id == suggestionId).First();
                user.VotedOnSuggestions.Remove(suggestionToRemove);
            }
            await usersInTransaction.ReplaceOneAsync(u => u.Id == userId, user);

            await session.CommitTransactionAsync();

            _cache.Remove(CacheName);
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public async Task CreateSuggestion(SuggestionModel suggestion)
    {
        // again transaction, like above UpvoteSuggestion
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectioName);
            await suggestionsInTransaction.InsertOneAsync(suggestion);

            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUser(suggestion.Author.Id);
            user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
            await usersInTransaction.ReplaceOneAsync(u => u.Id == user.Id, user);

            await session.CommitTransactionAsync();

            // did not cache, the suggestion cache updates every 1 minute
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}
