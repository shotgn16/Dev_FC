using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForestChurches.Pages.Admin.Church
{
    public class EditModel : PageModel
    {
        private readonly ForestChurchesContext _context;
        private readonly ILogger<EditModel> _logger;
        private readonly UserManager<ChurchAccount> _userManager;

        public EditModel(
            ForestChurchesContext context,
            ILogger<EditModel> logger, UserManager<ChurchAccount> userManager)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public ProfileUpdateModel SaveChangesModel { get; set; }
        public ChurchInformation Church { get; private set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet(string id)
        {
            try
            {
                Church = _context.ChurchInformation.Where(c => c.ID == id).FirstOrDefault();
                if (Church == null)
                {
                    StatusMessage = "Error: Church not found.";
                    return Page();
                }

                SaveChangesModel = new ProfileUpdateModel
                {
                    ID = Church.ID,
                    Name = Church.Name,
                    Description = Church.Description,
                    Denomination = Church.Denominaion,
                    Congregation = Church.Congregation,
                    Address = Church.Address,
                    Phone = Church.Phone,
                    Churchsuite = Church.Churchsuite,
                    Website = Church.Website,
                    Parking = Church.Parking,
                    Restrooms = Church.Restrooms,
                    WheelchairAccess = Church.WheelchairAccess,
                    Wifi = Church.Wifi,
                    Refreshments = Church.Refreshments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving ({id}).");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateChurch()
        {
            try
            {
                if (SaveChangesModel == null)
                {
                    StatusMessage = "Error: Unable to save changes. Please try again later.";
                    return Page();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    StatusMessage = "Error: Unable to retrieve user information.";
                    return Page();
                }

                var church = _context.ChurchInformation.Where(c => c.ChurchAccount.Id == user.Id).FirstOrDefault();
                if (church == null)
                {
                    StatusMessage = "Error: Church information is not found.";
                    return Page();
                }

                church.Name = SaveChangesModel.Name;
                church.Description = SaveChangesModel.Description;
                church.Denominaion = SaveChangesModel.Denomination;
                church.Congregation = SaveChangesModel.Congregation;
                church.Address = SaveChangesModel.Address;
                church.Phone = SaveChangesModel.Phone;
                church.Churchsuite = SaveChangesModel.Churchsuite;
                church.Website = SaveChangesModel.Website;
                church.Parking = SaveChangesModel.Parking;
                church.Restrooms = SaveChangesModel.Restrooms;
                church.WheelchairAccess = SaveChangesModel.WheelchairAccess;
                church.Wifi = SaveChangesModel.Wifi;
                church.Refreshments = SaveChangesModel.Refreshments;

                _context.Update(church);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating ({SaveChangesModel.Name}).");
                return Page();
            }

            return RedirectToPage();
        }

    }
}
