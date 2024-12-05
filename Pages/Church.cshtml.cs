using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Migrations;
using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Pages
{
    public class ChurchModel : PageModel
    {
        private readonly ForestChurchesContext _context;
        private readonly UserManager<ChurchAccount> _userManager;
        public ChurchModel(ForestChurchesContext context, UserManager<ChurchAccount> userManager)
        {
            _context = context;
            _userManager = userManager;

            if (ChurchID != Guid.Empty)
            {
                Load(ChurchID).RunSynchronously();
            }
        }

        // Data objects
        private Guid ChurchID { get; set; }

        public Models.ChurchInformation ChurchInformation { get; set; }

        public List<EventsModel> ChurchEvents { get; set; }

        public ChurchAccount AssociatedUser { get; set; }

        public async Task OnGetAsync(Guid id)
        {
            if (id != Guid.Empty)
            {
                await Load(id);
            }
            else if (ChurchID == Guid.Empty)
            {
                await Load(ChurchID);
            }
        }

        private async Task Load(Guid churchId)
        {
            if (churchId != Guid.Empty)
            {
                ChurchID = churchId;

                ChurchInformation = await _context.ChurchInformation
                    .Include(s => s.ServiceTimes)
                    .Where(variable => variable.ID == churchId.ToString())
                    .FirstOrDefaultAsync();

                if (ChurchInformation != null)
                {
                    AssociatedUser = await _userManager.FindByIdAsync(ChurchInformation.ChurchAccountId);

                    ChurchEvents = await _context.Events
                        .Where(variable => variable.Church == ChurchInformation.Name)
                        .Where(variable => variable.User == AssociatedUser.UserName)
                        .ToListAsync();
                }
            }
        }
    }
}
