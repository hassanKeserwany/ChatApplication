using API.Data;
using API.Entities;
using API.Extenstions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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





//add ITokenService interface to Dep.injection services
builder.Services.AddScoped<ITokenService,TokenService>();



//add authentication service , using extension method(api.extensions...)class
//we user extension class for cleaning purpuses
builder.Services.addIdentityService(config);



/*using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>(); // get DataContext instance from the dependency injection (DI) 
    var user1 = new AppUser();
    user1.UserName = "Ali";
    context.Users.Add(user1);
    context.SaveChanges();
}*/





// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

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

app.Run();
