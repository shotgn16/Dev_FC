using ForestChurches.Components.Email;
using ForestChurches.Components.Users;
using ForestChurches.Data;
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
        private readonly ForestChurchesContext _context;
        private readonly UserManager<ChurchAccount> _userManager;

        // Data Objects
        [BindProperty]
        public UserContact Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public ContactModel(iMailSender mailSender, UserManager<ChurchAccount> userManager, ForestChurchesContext context)
        {
            _context = context;
            _mailSender = mailSender;
            _userManager = userManager;
        }

        public async Task OnGetAsync(Guid cid)
        {
            Input = new UserContact();

            if (cid != null)
            {
                Input.Cid = cid;
            }

            if (User.Identity.IsAuthenticated == true)
            {
                Input.ClientEmail = User.Identity.Name;
            }
        }

        public async Task OnPostAsync()
        {
            string recipientEmail = "support@forestchurches.co.uk";

            if (Input.Cid != Guid.Empty)
            {
                recipientEmail = _context.ChurchInformation
                    .Where(a => a.ID == Input.Cid.ToString())
                    .Select(b => b.ChurchAccount.Email)
                    .FirstOrDefault();
            }

            // Determine the sender email based on user authentication status
            string senderEmail = User.Identity.IsAuthenticated ? User.Identity.Name : Input.ClientEmail;

            // User must be authenticated as this contact is ONLY for authenticated users, and will use their authenticated email address as the user contact.
            if (Input != null && User.Identity.IsAuthenticated)
            {
                _mailSender.SendEmailContactRequest(
                    Guid.Parse(_userManager.GetUserId(User)),
                    _userManager.GetUserName(User),
                    recipientEmail,
                    senderEmail,
                    Input.Message,
                    "/Index"
                );

                StatusMessage = "Your message has been sent!";
            }
            else
            {
                StatusMessage = "Error: An error occurred while sending your message";
            }
        }


    }
}
