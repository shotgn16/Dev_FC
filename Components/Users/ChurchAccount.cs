using System.Diagnostics.CodeAnalysis;
using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;

namespace ForestChurches.Components.Users;

// Add profile data for application users by adding properties to the  class
public class ChurchAccount : IdentityUser
{
    [PersonalData]
    public ChurchInformation ChurchInformation { get; set; }

    [PersonalData]
    public DateTime CreatedDate { get; set; }

    [AllowNull]
    [PersonalData]
    public byte[] ImageArray { get; set; }

    [AllowNull]
    [PersonalData]
    public string Role { get; set; }
}

