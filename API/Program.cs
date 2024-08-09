
using API.Data;
using API.Entities;
using API.Extenstions;
using API.Helper;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add dbcontext service
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json")
    .Build();
var connectionString = config.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connectionString)
);

// add service for AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

// add ITokenService interface to Dep.injection services
builder.Services.AddScoped<ITokenService, TokenService>();

// add service to repository pattern IUserRepository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// add service LogUserActivity
builder.Services.AddScoped<LogUserActivity>();

// add authentication service, using extension method
builder.Services.addIdentityService(config);

// add SignalR 
builder.Services.AddSignalR();

// Add Cloudinary settings
builder.Services.Configure<CloudinarySetttings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IPhotoService, PhotoService>();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("https://localhost:4200");
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure Kestrel for HTTPS with SSL certificate
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        var certConfig = config.GetSection("Kestrel:Endpoints:Https:Certificate");
        var certPath = certConfig["Path"];
        httpsOptions.ServerCertificate = new X509Certificate2(certPath, (string)null);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // Show detailed error information in development
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable serving static files from wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseRouting();
app.UseCors("AllowAll"); // Use CORS
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles(); // This line serves static files from wwwroot

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<PresenceHub>("/hubs/presence");
    endpoints.MapHub<MessageHub>("/hubs/message");
    endpoints.MapFallbackToController("Index", "Fallback");
});

// Seed roles for users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

        await context.Database.MigrateAsync();
        await Seed.SeedRoles(roleManager); // Call the SeedRoles method
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration");
    }
}

await app.RunAsync();


/////////////////////////////////////////////////////////////////

/*using API.Data;
using API.Entities;
using API.Extenstions;
using API.Helper;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// add dbcontext service
var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json").Build();
var connectionString = config.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options =>
              options.UseSqlServer(connectionString)
              );




    //add service for autoMapper
    builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

//add ITokenService interface to Dep.injection services
builder.Services.AddScoped<ITokenService,TokenService>();


//we dont need these service any more , replace them with
//add service to repository pattern IUserRepository
*//*builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<ILikesRepository, LikesRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();*//*
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


//add service LogUserActivity
builder.Services.AddScoped<LogUserActivity>();
//add authentication service , using extension method(api.extensions...)class
//we use extension class for cleaning purpuses
builder.Services.addIdentityService(config);

//add signalR 
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.Configure<CloudinarySetttings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IPhotoService,PhotoService>();


// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowCredentials()
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .WithOrigins("https://localhost:4200");
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();




var app =builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // Show detailed error information in development

}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable serving static files from wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseRouting();
app.UseCors("AllowAll");// Use CORS
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles(); // This line serves static files from wwwroot

*//*app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
*/

/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");*//*


//app.MapFallbackToController("Index", "Fallback");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<PresenceHub>("/hubs/presence");
    endpoints.MapHub<MessageHub>("/hubs/message");
   endpoints.MapFallbackToController("Index", "Fallback");

});



//seed roles for users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try

    {
        var context = services.GetRequiredService<DataContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

        await context.Database.MigrateAsync();
        await Seed.SeedRoles(roleManager); // Call the SeedRoles method
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration");
    }
}

await app.RunAsync();*/


////////////////////////////////////////////////////////////////
///