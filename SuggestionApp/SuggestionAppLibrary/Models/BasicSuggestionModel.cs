namespace SuggestionAppLibrary.Models;

public class BasicSuggestionModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    // did not use BsonId as this is a not a unique Identifier, can be there multiple times
    // this BasicSuggesionModel will not be an independent object in Mongo, this will be a subobject of UserModel
    public string Id { get; set; }
    public string Suggestion { get; set; }

    public BasicSuggestionModel()
    {

    }

    public BasicSuggestionModel(SuggestionModel suggestion)
    {
        Id = suggestion.Id;
        Suggestion = suggestion.Suggestion;
    }
}
