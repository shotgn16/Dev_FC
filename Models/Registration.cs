﻿using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ForestChurches.Models
{
    public class Registration
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // Custom Parameters

        [Required]
        [PersonalData]
        [DataType(DataType.Text)]
        [Display(Name = "Church Name")]
        public string ChurchName { get; set; }

        [Required]
        [PersonalData]
        [DataType(DataType.Text)]
        [Display(Name = "Church Denomination")]
        public string ChurchDenomination { get; set; }

        [Required]
        [PersonalData]
        [DataType(DataType.Text)]
        [Display(Name = "Church Website")]
        public string ChurchWebsite { get; set; }

        [Ignore]
        [PersonalData]
        public byte[] ImageArray { get; set; }

        [Ignore]
        [PersonalData]
        public IFormFile Image { get; set; }

        [AllowNull]
        [DataType(DataType.MultilineText)]
        public List<string> Roles { get; set; }
    }
}
