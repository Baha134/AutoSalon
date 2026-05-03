using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Area("Admin")]
[Route("admin")]
public class AdminAccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signIn;

    public AdminAccountController(SignInManager<IdentityUser> signIn)
    {
        _signIn = signIn;
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return Redirect("/");
    }
}