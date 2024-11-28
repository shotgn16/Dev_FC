using ForestChurches.Components.Users;
using ForestChurches.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForestChurches.Pages.Admin
{
    [Authorize(Policy = "Admin.Read")]
    public class IndexModel : PageModel
    {
        private UserManager<ChurchAccount> _userManager;
        private ForestChurchesContext _context;

        internal int registeredChurches;
        internal int registeredEvents;

        public IndexModel(UserManager<ChurchAccount> userManager, ForestChurchesContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public void OnGet()
        {
            registeredChurches = _userManager.Users.Count();
            registeredEvents = _context.Events.Count();
        }
    }
}
