using Kiselev.UserDbTestTask.Data.Users;
using Kiselev.UserDbTestTask.Repository.User;

namespace Kiselev.UserDbTestTask.Controllers
{
    public class UserController
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/get_users", async (IUserRepository repository) =>
                Results.Ok(await repository.GetUsersAsync()))
                .Produces<List<UserDTO>>(StatusCodes.Status200OK)
                .WithTags("Getters");

            app.MapGet("/get_user/{id}", async (int id, IUserRepository repository) =>
                await repository.GetUserAsync(id) is UserDTO userDTO
                    ? Results.Ok(userDTO)
                    : Results.NotFound($"User with id ({id}) not found"))
                    .Produces<UserDTO>(StatusCodes.Status200OK)
                    .Produces(StatusCodes.Status404NotFound)
                    .WithTags("Getters");

            app.MapPost("/create_user", async ([FromBody] UserDTO userDTO, IUserRepository repository) =>
            {
                var result = await repository.TryAddUserAsync(userDTO);
                if (result.IsSuccess == false)
                    return Results.BadRequest(result.Description);

                await repository.SaveAsync();
                return Results.Created($"/users/{userDTO.Id}", userDTO);
            })
                .Accepts<UserDTO>("application/json")
                .Produces<UserDTO>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithTags("Creators");

            app.MapPut("/update_user", async ([FromBody] UserDTO userDTO, IUserRepository repository) =>
            {
                var result = await repository.TryUpdateUserAsync(userDTO);
                if (result.IsSuccess == false)
                    return Results.BadRequest(result.Description);

                await repository.SaveAsync();
                return Results.NoContent();
            })
                .Accepts<UserDTO>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithTags("Updaters");


            app.MapPut("/add_user_role/{id}", async (int id, Roles role, IUserRepository repository) =>
            {
                var result = await repository.TryAddNewRoleAsync(id, role);
                if (result.IsSuccess == false)
                    return Results.BadRequest(result.Description);

                await repository.SaveAsync();
                return Results.NoContent();
            })
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithTags("Updaters");

            app.MapDelete("/delete_user/{id}", async (int id, IUserRepository repository) =>
            {
                if (await repository.TryDeleteUserAsync(id) == false)
                    return Results.NotFound($"User with id ({id}) not found");

                await repository.SaveAsync();
                return Results.NoContent();
            })
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithTags("Deleters");

            app.MapDelete("/delete_all_user_roles/{id}", async (int id, IUserRepository repository) =>
            {
                if (await repository.TryDeleteUsersRoleAsync(id) == false)
                    return Results.NotFound($"User with id ({id}) not found");

                await repository.SaveAsync();
                return Results.NoContent();
            })
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithTags("Deleters");
        }
    }
}
