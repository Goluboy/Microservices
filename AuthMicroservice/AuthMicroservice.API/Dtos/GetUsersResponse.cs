namespace AuthMicroservice.API.Dtos;


public record GetUsersResponse(IList<UsersWithRoles> users);
public record UsersWithRoles(Guid Id, string FirstName, string LastName, string Email, IList<string> Roles);