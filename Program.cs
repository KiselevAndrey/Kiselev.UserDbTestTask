using Kiselev.UserDbTestTask.Data.Users;
using Kiselev.UserDbTestTask.DataBase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

var app = builder.Build();

// проверка что ДБ создастся при разработке
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<UserDb>();
    db.Database.EnsureCreated();
}


app.MapGet("/users", async (UserDb db) => await db.Users.ToListAsync());

app.MapGet("/users/{id}", async (int id, UserDb db) => 
    await db.Users.FirstOrDefaultAsync(u => u.Id == id) is UserDTO userDTO
        ? Results.Ok(userDTO)
        : Results.NotFound());

app.MapPost("/users", async ([FromBody] UserDTO userDTO, UserDb db) =>
{
    db.Users.Add(userDTO);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{userDTO.Id}", userDTO);
});

app.MapPut("/users", async ([FromBody] UserDTO userDTO, UserDb db) =>
{
    var userFromDb = await db.Users.FindAsync(new object[] { userDTO.Id });
    if(userFromDb == null)
        return Results.NotFound();

    userFromDb.CopyFrom(userDTO);

    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.MapDelete("/users/{id}", async (int id, UserDb db) =>
{
    var userFromDb = await db.Users.FindAsync(new object[] { id });
    if (userFromDb == null)
        return Results.NotFound();

    db.Remove(userFromDb);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();

app.Run();
