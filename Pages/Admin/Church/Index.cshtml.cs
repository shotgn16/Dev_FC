using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForestChurches.Pages.Admin.Church
{
    public class IndexModel : PageModel
    {
        private ForestChurchesContext _context;
        private ILogger<IndexModel> _logger;

        public List<ChurchInformation> Churches { get; private set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(
            ForestChurchesContext context,
            ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            Churches = _context.ChurchInformation.ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteChurch(string value)
        {
            IdentityResult result = new();
            ChurchInformation church = new();
            try
            {
                church = _context.ChurchInformation.Where(c => c.ID == value).FirstOrDefault();

                _context.Remove(church);

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting ({church.Name}).");
            }

            return Page();
        }

    }
}
