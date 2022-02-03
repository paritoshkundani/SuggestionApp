namespace SuggestionAppUI;

public static class RegisterServices
{
    // TC did not want to do all this in Program.cs, wanted to keep it cleaner so moved things in here
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // below was from Program.cs just moved it here
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        // new things he added

        // caching is built into web projects so no need here to add it to Nuget for this project, AppLibrary he did at Nuget as that's a class project which will also use it
        builder.Services.AddMemoryCache();  
    }
}
