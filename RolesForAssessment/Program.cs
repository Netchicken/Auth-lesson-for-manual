using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using RolesForAssessment.AuthorizationHandlers;
using RolesForAssessment.AuthorizationRequirements;
using RolesForAssessment.Data;

var CORSAllowSpecificOrigins = "_CORSAllowed";

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORSAllowSpecificOrigins,
                         policy =>
                         {
                             policy.WithOrigins("http://localhost:3000", "http://www.contoso.com");
                         });
});



// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)


    );

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//the default identity of the user
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


// All 3 handlers need to the registered with the service container in program.cs: 
builder.Services.AddSingleton<IAuthorizationHandler, IsInRoleHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, HasClaimHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ViewRolesHandler>();



builder.Services.AddRazorPages();

//Authorization within a Razor Pages application is provided by a number of services, including an IAuthorizationService. These must be added to the service container at application startup. A convenience method, AddAuthorization takes care of adding all the required services: builder.Services.AddAuthorization();

builder.Services.AddAuthorization(options =>
{
    // options.AddPolicy("AdminPolicy", policyBuilder => policyBuilder.RequireRole("Admin"));



    options.AddPolicy("AdminPolicy", policyBuilder => policyBuilder.RequireClaim("Admin"));




    //We use the RequireAssertion method, which takes an AuthorizationHandlerContext as a parameter providing access to the current user

    //all of this gets replaced by the Authorization Handlers later on in the course
    options.AddPolicy("OLDViewRolesPolicy", policyBuilder => policyBuilder.RequireAssertion(context =>
    {
        //if they have a claim of type "Joining Date" and the value is less than 6 months ago, and they have Permission and View Roles they can view roles
        // We use the FindFirst method to access a claim and obtain its value(if there is one) and convert it to a DateTime
        var joiningDateClaim = context.User.FindFirst(c => c.Type == "Joining Date")?.Value;
        var joiningDate = Convert.ToDateTime(joiningDateClaim);


        return context.User.HasClaim("Permission", "View Roles") && joiningDate > DateTime.MinValue && joiningDate < DateTime.Now.AddMonths(-6);

    }));

    //this policy only allows people who can view roles and have been employed longer than 6 months and comes from the ViewRolesRequirement Class
    options.AddPolicy("ViewRolesPolicy", policyBuilder => policyBuilder.AddRequirements(new ViewRolesRequirement(months: -6)));


    //this replaces the one above by moving all the code out to its own classes
    options.AddPolicy("ViewClaimsPolicy", policyBuilder => policyBuilder.AddRequirements(new ViewClaimsRequirement(months: -6)));

});


//Having configured the policy we can apply it to the AuthorizeFolder method to ensure that only members of the Admin role can access the content: 
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/RolesManager", "ViewRolesPolicy");
});

// // options.Conventions.AuthorizeFolder("/RolesManager", "AdminPolicy");
// options.Conventions.AuthorizeFolder("/ClaimsManager", "AdminPolicy");

//=======NEW SECURITY============

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedEmail = false;


    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;

});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

//=============END NEW SECURITY================

builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();

}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseCors(CORSAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();   //Authorization middleware is enabled by default in the web application template by the inclusion of app.UseAuthorization() in the Program class.  

app.MapControllers();

app.MapRazorPages();



app.Run();

public partial class Program { }