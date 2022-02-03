namespace SuggestionAppLibrary.Models;

// intially this was complaining that we need to consider declaring all properties as nullable, TC removed the
//   <PropertyGroup>
//    <TargetFramework>net6.0</TargetFramework>
//    <ImplicitUsings>enable</ImplicitUsings>
//    <Nullable>enable</Nullable>  --> removed this to stop the compiler from showing that warning
//  </PropertyGroup>
public class CategoryModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string CategoryName { get; set; }
    public string CategoryDescription { get; set; }
}
