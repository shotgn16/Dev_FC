using ForestChurches.Areas.Identity.Data;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Models;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog.Sinks.File;
using System.Diagnostics;
using System.Linq;

namespace ForestChurches.Pages.Admin
{
    [Authorize(Policy = "Admin.Read")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ChurchAccount> _userManager;
        private readonly ForestChurchesContext _context;
        private readonly IDatabaseService _databaseService;

        internal List<Models.ChurchInformation> registeredChurches;
        internal List<EventsModel> registeredEvents;

        internal string DatabaseSize = "2.5";

        public IndexModel(UserManager<ChurchAccount> userManager, ForestChurchesContext context, IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            _userManager = userManager;
            _context = context;
        }

        public async Task OnGetAsync()
        {
            registeredChurches = _context.ChurchInformation.OrderByDescending(a => a.ChurchAccount.CreatedDate).ToList();
            registeredEvents = _context.Events.ToList();


            //DatabaseSize = await _databaseService.GetDatabaseSize("forestchurches");
        }
    }
}
