using Kiselev.UserDbTestTask.Data.Users;
using Kiselev.UserDbTestTask.DataBase;
using Kiselev.UserDbTestTask.Repository.User;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<UserDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/users", async (IUserRepository repository) => 
    Results.Ok(await repository.GetUsersAsync()));

app.MapGet("/users/{id}", async (int id, IUserRepository repository) => 
    await repository.GetUserAsync(id) is UserModel userModel
        ? Results.Ok(userModel)
        : Results.NotFound());

app.MapPost("/users", async ([FromBody] UserDTO userDTO, IUserRepository repository) =>
{
    await repository.AddUserAsync(userDTO);
    await repository.SaveAsync();
    return Results.Created($"/users/{userDTO.Id}", userDTO);
});

app.MapPut("/users", async ([FromBody] UserDTO userDTO, IUserRepository repository) =>
{
    if(await repository.TryUpdateUserAsync(userDTO) == false)
        return Results.NotFound();
      
    await repository.SaveAsync();
    return Results.NoContent();
});


app.MapDelete("/users/{id}", async (int id, IUserRepository repository) =>
{
    if (await repository.TryDeleteUserAsync(id) == false)
        return Results.NotFound();

    await repository.SaveAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();

app.Run();

void RegisterServices(IServiceCollection services)
{
    services.AddDbContext<UserDb>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
    });

    services.AddScoped<IUserRepository, UserRepository>();
}