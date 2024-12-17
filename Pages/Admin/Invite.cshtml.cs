using ForestChurches.Components.Email;
using ForestChurches.Components.UserRegistration;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using System.Linq;

namespace ForestChurches.Pages.Admin
{
    [Authorize(Policy = "Admin.Read")]
    public class InviteModel : PageModel
    {
        private readonly ForestChurchesContext _context;
        private readonly ILogger<InviteModel> _logger;
        private readonly iRegistrationGenerate _register;
        private readonly iMailSender _mailSender;

        internal List<WhitelistedUsers> invites;
        string _url;

        [TempData]
        public string StatusMessage { get; set; }

        public InviteModel(ForestChurchesContext context, ILogger<InviteModel> logger, iRegistrationGenerate register, iMailSender mailSender)
        {
            _register = register;
            _context = context;
            _logger = logger;
            _mailSender = mailSender;

            invites = _context.WhitelistedUsers.ToList();
        }

        public void OnGet()
        {
            invites = _context.WhitelistedUsers.ToList();
        }

        public async Task<IActionResult> OnPostAddWhitelistedUser(string email)
        {
            if (!string.IsNullOrEmpty(email)) 
            {
                _context.WhitelistedUsers.Add(new WhitelistedUsers
                {
                    Email = email,
                    ID = Guid.NewGuid(),
                    DateOfRegistration = DateTime.MinValue,
                    Status = "Pending",
                    ExpiryDate = DateTime.UtcNow.AddHours(24)
                });

                // Send an email to the user telling them they are Invited
                _url = _register.GenerateUrl(HttpContext.Request, email);

                _mailSender.SendEmailInviteUser(email, _url);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveWhitelistedUser(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var _selectedUser = _context.WhitelistedUsers
                    .Where(a => a.Email == email)
                    .FirstOrDefault();

                _context.WhitelistedUsers.Remove(_selectedUser);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostResendInvite(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                _url = _register.GenerateUrl(HttpContext.Request, email);
                var emailData = new Dictionary<string, string>()
                {
                    { "{email}", email },
                    { "{registration_link}", _url },
                };

                _mailSender.SendEmailInviteUser(email, _url);

            }

            return RedirectToPage();
        }
    }
}
