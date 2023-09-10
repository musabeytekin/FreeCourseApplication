using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models;

public class SignInInput
{
    [Display(Name = "Email Address")]
    public string Email { get; set; }
    
    [Display(Name = "Password")]
    public string Password { get; set; }
    
    [Display(Name = "Remember Me")]
    public bool IsRemember { get; set; }
}