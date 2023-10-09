using Kiselev.UserDbTestTask.Data.Users;
using Kiselev.UserDbTestTask.Repository.User;

namespace Kiselev.UserDbTestTask.Controllers
{
    public class UserController
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/users", async (IUserRepository repository) =>
                Results.Ok(await repository.GetUsersAsync()))
                .Produces<List<UserDTO>>(StatusCodes.Status200OK)
                .WithName("Get all users")
                .WithTags("Getters");

            app.MapGet("/users/{id}", async (int id, IUserRepository repository) =>
                await repository.GetUserAsync(id) is UserDTO userDTO
                    ? Results.Ok(userDTO)
                    : Results.NotFound($"User with id ({id}) not found"))
                    .Produces<UserDTO>(StatusCodes.Status200OK)
                    .Produces(StatusCodes.Status404NotFound)
                    .WithName("Get user")
                    .WithTags("Getters");

            app.MapPost("/users", async ([FromBody] UserDTO userDTO, IUserRepository repository) =>
            {
                var result = await repository.TryAddUserAsync(userDTO);
                if (result.IsSuccess == false)
                    return Results.BadRequest(result.Description);
                else
                {
                    await repository.SaveAsync();
                    return Results.Created($"/users/{userDTO.Id}", userDTO);
                }
            })
                .Accepts<UserDTO>("application/json")
                .Produces<UserDTO>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName("Create user")
                .WithTags("Creators");

            app.MapPut("/users", async ([FromBody] UserDTO userDTO, IUserRepository repository) =>
            {
                if (await repository.TryUpdateUserAsync(userDTO) == false)
                    return Results.NotFound($"User with id ({userDTO.Id}) not found");

                await repository.SaveAsync();
                return Results.NoContent();
            })
                .Accepts<UserDTO>("application/json")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("Update user")
                .WithTags("Updaters");


            app.MapDelete("/users/{id}", async (int id, IUserRepository repository) =>
            {
                if (await repository.TryDeleteUserAsync(id) == false)
                    return Results.NotFound($"User with id ({id}) not found");

                await repository.SaveAsync();
                return Results.NoContent();
            })
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("Delete user")
                .WithTags("Deleters");
        }
    }
}
