using ForestChurches.Components.Http.Google;
using ForestChurches.Components.ImageHandler;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Areas.Identity.Pages.Account.Manage
{
    public class ChurchProfileModel : PageModel
    {
        private readonly ILogger<ChurchProfileModel> _logger;
        private readonly UserManager<ChurchAccount> _userManager;
        private readonly ForestChurchesContext _context;
        private readonly GoogleInterface _googleAPI;
        private readonly ImageInterface _imageInterface;

        public ChurchProfileModel(ILogger<ChurchProfileModel> logger, UserManager<ChurchAccount> userManager,
            ForestChurchesContext context, GoogleInterface googleAPI, ImageInterface imageInterface)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _googleAPI = googleAPI;
            _imageInterface = imageInterface;
        }

        public ChurchAccount CurrentUser { get; set; }
        public ChurchInformation CurrentChurch { get; set; }
        public List<ServiceTimes> ServiceTimes { get; set; }

        [BindProperty]
        public ProfileUpdateModel SaveChangesModel { get; set; }

        [BindProperty]
        public IFormFile UnprocessedImage { get; set; }

        [BindProperty]
        public ServiceTimes ChurchService { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                CurrentUser = await _userManager.GetUserAsync(User);
                if (CurrentUser == null)
                {
                    throw new ArgumentNullException("CurrentUser is null");
                }

                CurrentChurch = await _context.ChurchInformation
                    .FirstOrDefaultAsync(c => c.ChurchAccountId == CurrentUser.Id);

                if (CurrentChurch == null)
                {
                    throw new ArgumentNullException("CurrentChurch is null");
                }

                // Populate SaveChangesModel with data from CurrentChurch
                SaveChangesModel = new ProfileUpdateModel
                {
                    ID = CurrentChurch.ID,
                    Name = CurrentChurch.Name,
                    Description = CurrentChurch.Description,
                    Denomination = CurrentChurch.Denominaion,
                    Congregation = CurrentChurch.Congregation,
                    Address = CurrentChurch.Address,
                    Phone = CurrentChurch.Phone,
                    Churchsuite = CurrentChurch.Churchsuite,
                    Website = CurrentChurch.Website,
                    Parking = CurrentChurch.Parking,
                    Restrooms = CurrentChurch.Restrooms,
                    WheelchairAccess = CurrentChurch.WheelchairAccess,
                    Wifi = CurrentChurch.Wifi,
                    Refreshments = CurrentChurch.Refreshments
                };

                ServiceTimes = await _context.ServiceTimes
                    .Where(c => c.ChurchInformationId == CurrentChurch.ID).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading church information.");
                StatusMessage = "An error occurred while loading your church information. Please try again later.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateChurchProfileAsync([FromForm] ProfileUpdateModel SaveChangesModel)
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
                    StatusMessage = "Error: Unable to find the current user.";
                    return Page();
                }

                CurrentChurch = await _context.ChurchInformation
                    .FirstOrDefaultAsync(c => c.ChurchAccountId == user.Id);
                if (CurrentChurch == null)
                {
                    StatusMessage = "Error: Church information is missing.";
                    return Page();
                }

                CurrentChurch.Name = SaveChangesModel.Name;
                CurrentChurch.Description = SaveChangesModel.Description;
                CurrentChurch.Denominaion = SaveChangesModel.Denomination;
                CurrentChurch.Congregation = SaveChangesModel.Congregation;
                CurrentChurch.Address = SaveChangesModel.Address;
                CurrentChurch.Phone = SaveChangesModel.Phone;
                CurrentChurch.Churchsuite = SaveChangesModel.Churchsuite;
                CurrentChurch.Website = SaveChangesModel.Website;
                CurrentChurch.Parking = SaveChangesModel.Parking;
                CurrentChurch.Restrooms = SaveChangesModel.Restrooms;
                CurrentChurch.WheelchairAccess = SaveChangesModel.WheelchairAccess;
                CurrentChurch.Wifi = SaveChangesModel.Wifi;
                CurrentChurch.Refreshments = SaveChangesModel.Refreshments;

                _context.Update(CurrentChurch);
                await _context.SaveChangesAsync();

                StatusMessage = $"The Church: ({CurrentChurch.Name}) was successfully updated!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating church information.");
                StatusMessage = "An error occurred while updating church information. Please try again later.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveServiceAsync(Guid id)
        {
            try
            {
                var service = await _context.ServiceTimes.SingleOrDefaultAsync(a => a.Id == id);
                if (service != null)
                {
                    _context.ServiceTimes.Remove(service);
                    await _context.SaveChangesAsync();
                    StatusMessage = "Service successfully deleted!";
                }
                else
                {
                    StatusMessage = "Error: Service not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing the service.");
                StatusMessage = "An error occurred while removing the service. Please try again later.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddServiceTimeAsync()
        {
            try
            {
                CurrentUser = await _userManager.GetUserAsync(User);
                CurrentChurch = await _context.ChurchInformation
                    .FirstOrDefaultAsync(c => c.ChurchAccountId == CurrentUser.Id);

                if (CurrentChurch == null)
                {
                    StatusMessage = "Error: Unable to find the current church.";
                    return RedirectToPage();
                }

                var existingServices = await _context.ServiceTimes
                    .Where(c => c.ChurchInformationId == CurrentChurch.ID)
                    .ToListAsync();

                if (!DoesDataExist(existingServices, ChurchService))
                {
                    var newService = new ServiceTimes
                    {
                        ChurchInformationId = CurrentChurch.ID,
                        Note = ChurchService.Note,
                        Time = ChurchService.Time,
                        Id = Guid.NewGuid()
                    };
                    _context.ServiceTimes.Add(newService);
                    await _context.SaveChangesAsync();
                    StatusMessage = $"Service: '{ChurchService.Note} ({ChurchService.Time})' successfully added!";
                }
                else
                {
                    StatusMessage = "Warning: Unable to add service. Your service already exists.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the service time.");
                StatusMessage = "An error occurred while adding the service time. Please try again later.";
            }

            return RedirectToPage();
        }

        private bool DoesDataExist(List<ServiceTimes> existingData, ServiceTimes newData)
        {
            return existingData.Any(data => data.Time == newData.Time && data.Note == newData.Note);
        }

        public async Task<IActionResult> OnGetMarkChurchStatusAsync(string status)
        {
            try
            {
                CurrentUser = await _userManager.GetUserAsync(User);
                CurrentChurch = await _context.ChurchInformation
                    .FirstOrDefaultAsync(c => c.ChurchAccountId == CurrentUser.Id);

                if (CurrentChurch != null)
                {
                    CurrentChurch.Status = status;
                    _context.Update(CurrentChurch);
                    await _context.SaveChangesAsync();
                    StatusMessage = $"The Church ({CurrentChurch.Name}) was successfully set to status ({status}).";
                }
                else
                {
                    StatusMessage = "Error: Unable to find the current church.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing the church status.");
                StatusMessage = "An error occurred while changing the church status. Please try again later.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateUserLogoAsync()
        {
            try
            {
                CurrentUser = await _userManager.GetUserAsync(User);

                if (UnprocessedImage != null)
                {
                    CurrentUser.ImageArray = await _imageInterface.ConvertToByteArray(UnprocessedImage);
                    _context.Update(CurrentUser);
                    await _context.SaveChangesAsync();
                    StatusMessage = "Logo successfully updated!";
                }
                else
                {
                    StatusMessage = "Error: No image file selected.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the logo.");
                StatusMessage = "An error occurred while updating the logo. Please try again later.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetGetAddressAsync(double latitude, double longitude)
        {
            string address = string.Empty;

            try
            {
                address = await _googleAPI.ConvertToAddress(latitude, longitude);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the address.");
            }

            return Content(address);
        }
    }
}
