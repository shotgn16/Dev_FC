using ForestChurches.Components.AutoEvents;
using ForestChurches.Components.Email;
using ForestChurches.Components.Http;
using ForestChurches.Components.ImageHandler;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Pages.Events.Add
{
    [Authorize(Policy = "UserEvents.View")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private ForestChurchesContext _context;
        private ImageInterface _iImage;
        private EventInterface _eventController;
        private UserManager<ChurchAccount> _userManager;
        private readonly iMailSender _mailSender;

        // Binds form input to this property
        [BindProperty]
        public EventsModel Input { get; set; }

        internal ChurchAccount CurrentUser;
        internal ChurchInformation CurrentChurch;

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger, 
            IHttpWrapper httpWrapper,
            ForestChurchesContext context, 
            ImageInterface iImage, 
            iMailSender mailSender,
            UserManager<ChurchAccount> userManager,
            EventInterface eventController)
        {
            _logger = logger;
            _context = context;
            _iImage = iImage;
            _userManager = userManager;
            _mailSender = mailSender;
            _eventController = eventController;

            LoadAsync();
        }

        private async Task<IActionResult> LoadAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);
            CurrentChurch = _context.ChurchInformation.Where(a => a.ChurchAccountId == CurrentUser.Id).FirstOrDefault();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Create a variable and assign a random value to it. 
            byte[] processedImage = new byte[1];

            CurrentUser = await _userManager.GetUserAsync(User);
            CurrentChurch = _context.ChurchInformation.Where(a => a.ChurchAccountId == CurrentUser.Id).FirstOrDefault();

            try
            {
                // Convert the image
                if (Input.Image == null)
                {
                    processedImage = await _iImage.ConvertToByteArrayFromUrl("https://i.imgur.com/oLC9RcU.png");
                }

                else if (Input.Image != null)
                {
                    processedImage = await _iImage.ConvertToByteArray(Input.Image);
                }

                // Add the rest of the information to the event...
                var newEvent = new EventsModel()
                {
                    ID = Guid.NewGuid(),
                    Name = Input.Name,
                    Description = Input.Description,
                    Date = Input.Date,
                    User = CurrentUser.UserName,
                    Address = Input.Address,
                    StartTime = Input.StartTime,
                    EndTime = Input.EndTime,
                    Church = CurrentChurch.Name,
                    ImageArray = processedImage,
                    Repeats = Input.Repeats,
                    Link = Input.Link
                };

                await _context.Events.AddAsync(newEvent);
                await _context.SaveChangesAsync();

                StatusMessage = $"Event ({Input.Name}) successfully created!";

                if (StatusMessage == $"Event ({Input.Name}) successfully created!")
                {
                    //var repeats = IntToRepeats(Input.Repeats);

                    _mailSender.SendEmailEventCreated(Guid.Parse(_userManager.GetUserId(User)), _userManager.GetUserName(User), "/Events", Input.Repeats, Input);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                StatusMessage = $"An error occurred while creating the event ({Input.Name}).";

                throw ex;

            }

            return Page();
        }
    }
}
