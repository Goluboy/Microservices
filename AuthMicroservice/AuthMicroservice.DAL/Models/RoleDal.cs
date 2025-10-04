using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AuthMicroservice.DAL.Models;

[Table("AspNetRoles")]
public class RoleDal : IdentityRole<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? CreatedBy { get; set; }
    
    [ForeignKey("CreatedBy")]
    public virtual UserDal? Creator { get; set; }
}