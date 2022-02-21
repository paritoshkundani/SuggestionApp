using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace SuggestionAppUI;

public static class RegisterServices
{
    // TC did not want to do all this in Program.cs, wanted to keep it cleaner so moved things in here
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // below was from Program.cs just moved it here
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();    // need AddMicrosoftIdentityConsentHandler for Azure B2C

        // Azusre B2C to get the UI to load for login/logout
        builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

        // new things he added
        // caching is built into web projects so no need here to add it to Nuget for this project, AppLibrary he did at Nuget as that's a class project which will also use it
        builder.Services.AddMemoryCache();

        // set up Azure B2C for authentication
        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));

        // use jobTitle = Admin claim for AdminPolicy to apply
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireClaim("jobTitle", "Admin"));
        });

        builder.Services.AddSingleton<IDbConnection, DbConnection>();  // he did it as singleton (1 connection for all users), said can make it scoped
        builder.Services.AddSingleton<ICategoryData, MongoCategoryData>();
        builder.Services.AddSingleton<IStatusData, MongoStatusData>();
        builder.Services.AddSingleton<ISuggestionData, MongoSuggestionData>();
        builder.Services.AddSingleton<IUserData, MongoUserData>();
    }
}
