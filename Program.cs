using Kiselev.UserDbTestTask.Controllers;
using Kiselev.UserDbTestTask.DataBase;
using Kiselev.UserDbTestTask.Repository.User;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();
Configure(app);

new UserController().Register(app);

app.Run();

void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddDbContext<UserDb>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
    });

    services.AddScoped<IUserRepository, UserRepository>();
}

void Configure(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserDb>();
        db.Database.EnsureCreated();
    }

    app.UseHttpsRedirection();
}