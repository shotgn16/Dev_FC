using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Pages.Admin.Events
{
    [Authorize(Policy = "EventManagement.Read")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private ForestChurchesContext _context;

        internal List<EventsModel> Events;

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ForestChurchesContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            LoadAsync();
        }

        private async Task<IActionResult> LoadAsync()
        {
            Events = _context.Events.ToList();

            return RedirectToPage();
        }

        //[Authorize(Policy = "EventManagement.Delete")]
        public async Task OnPostDeleteEvent(Guid value)
        {
            try
            {
                var eventToDelete = _context.Events
                    .Where(e => e.ID == value)
                        .FirstOrDefault();

                var result = _context.Events
                    .Remove(eventToDelete);

                _context.SaveChanges();

                if (result.State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
                {
                    _logger.LogInformation($"Event: '{eventToDelete.Name}' successfully deleted!");
                    StatusMessage = $"Event '{eventToDelete.Name}' successfully deleted!";
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
