using ForestChurches.Components.Email;
using ForestChurches.Components.Users;
using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForestChurches.Pages
{
    public class ContactModel : PageModel
    {
        // Dependency Injection
        private readonly iMailSender _mailSender;
        private readonly UserManager<ChurchAccount> _userManager;

        // Data Objects
        [BindProperty]
        public UserContact Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public ContactModel(iMailSender mailSender, UserManager<ChurchAccount> userManager)
        {
            _mailSender = mailSender;
            _userManager = userManager;
        }

        public async Task OnPostAsync()
        {
            // User must be authenticated as this contact is ONLY for authenticated users, and will use their authenticated email address as the user contact.
            if (Input != null && User.Identity.IsAuthenticated) 
            {
                _mailSender.SendEmailContactRequest(Guid.Parse(_userManager.GetUserId(User)), _userManager.GetUserName(User), User.Identity.Name, Input.Message, "/Index");
                StatusMessage = "Your message has been sent!";
            }

            else
            {
                StatusMessage = "Error: An error occurred while sending your message";
            }
        }

    }
}
