
namespace SuggestionAppLibrary.DataAccess;

public interface ISuggestionData
{
    Task CreateSuggestion(SuggestionModel suggestion);
    Task<List<SuggestionModel>> GetAllApprovedSuggestionsAsync();
    Task<List<SuggestionModel>> GetAllSuggestionsAsync();
    Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval();
    Task<SuggestionModel> GetSuggestion(string id);
    Task UpdateSuggestion(SuggestionModel suggestion);
    Task UpvoteSuggestion(string suggestionId, string userId);
}