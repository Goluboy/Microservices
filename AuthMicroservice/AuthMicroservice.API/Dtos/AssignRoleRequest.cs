namespace AuthMicroservice.API.Dtos;

public class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public string Role { get; set; }
}