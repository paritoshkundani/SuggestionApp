namespace SuggestionAppLibrary.Models;

public class BasicUserModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    // did not use BsonId as this is a not a unique Identifier, can be there multiple times
    // this BasicUserModel will not be an independent object in Mongo, this will be a subobject of SuggestionModel
    public string Id { get; set; }
    public string DisplayName { get; set; }

    public BasicUserModel()
    {

    }

    public BasicUserModel(UserModel user)
    {
        Id = user.Id;
        DisplayName = user.DisplayName;
    }
}
