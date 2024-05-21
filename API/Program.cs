using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

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



var app = builder.Build();

/*using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>(); // get DataContext instance from the dependency injection (DI) 
    var user1 = new AppUser();
    user1.UserName = "Ali";
    context.Users.Add(user1);
    context.SaveChanges();
}*/



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
