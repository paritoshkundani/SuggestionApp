namespace SuggestionAppLibrary.DataAccess;

public class MongoUserData : IUserData
{
    private readonly IMongoCollection<UserModel> _users;

    public MongoUserData(IDbConnection db)
    {
        _users = db.UserCollection;
    }

    // get all user records
    public async Task<List<UserModel>> GetUsersAsync()
    {
        var results = await _users.FindAsync(_ => true);
        return results.ToList();
    }

    // get one user
    public async Task<UserModel> GetUser(string id)
    {
        var results = await _users.FindAsync(u => u.Id == id);
        return results.FirstOrDefault();
    }

    // locate based on Azure B2C object identifier
    public async Task<UserModel> GetUserFromAuthentication(string objectId)
    {
        var results = await _users.FindAsync(u => u.ObjectIdentifier == objectId);
        return results.FirstOrDefault();
    }

    public Task CreateUser(UserModel user)
    {
        return _users.InsertOneAsync(user);
    }

    public Task UpdateUser(UserModel user)
    {
        // locate the user based on Id
        var filter = Builders<UserModel>.Filter.Eq("id", user.Id);
        // replace the one is db with what we passed in, if not there add a new one to db (IsUpsert = true)
        return _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    }
}
