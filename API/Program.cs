using API.Data;
using API.Entities;
using API.Extenstions;
using API.Helper;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

//add service to repository pattern IUserRepository
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<ILikesRepository, LikesRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepostory>();



//add service LogUserActivity
builder.Services.AddScoped<LogUserActivity>();
//add authentication service , using extension method(api.extensions...)class
//we use extension class for cleaning purpuses
builder.Services.addIdentityService(config);

// Add services to the container.
builder.Services.Configure<CloudinarySetttings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IPhotoService,PhotoService>();


// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


var app =builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
// Use CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.Run();



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

await app.RunAsync();