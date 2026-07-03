using Microsoft.AspNetCore.Identity;

namespace TaskNow.DAO;

public class ApplicationUser : IdentityUser
{
    public string Nome { get; set; } = string.Empty;
}