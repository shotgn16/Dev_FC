using ForestChurches.Components.AutoEvents;
using ForestChurches.Components.ImageHandler;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Pages.Events.Edit
{
    [Authorize(Policy = "UserEvents.Edit")]
    public class IndexModel : PageModel
    {
        private ILogger<IndexModel> _logger;
        private readonly ForestChurchesContext _context;
        private readonly ImageInterface _ImageRepository;
        private readonly EventInterface _eventController;
        private readonly UserManager<ChurchAccount> _userManager;

        public EventsModel currentEvent { get; set; }
        
        [TempData]
        public Guid EventID { get; set; }

        [TempData]
        public string StatusMessage { get; set; } 

        [BindProperty]
        public int Frequency { get; set; }

        [BindProperty]
        public EventsModel Input { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            ForestChurchesContext context, 
            ImageInterface ImageRepository,
            EventInterface eventController, UserManager<ChurchAccount> userManager)
        {
            _ImageRepository = ImageRepository;
            _eventController = eventController;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public void OnGet(string id)
        {
            try
            {
                currentEvent = _context.Events.Where(x => x.ID == Guid.Parse(id)).FirstOrDefault();

                EventID = Guid.Parse(id);

                Input = new EventsModel
                {
                    Name = currentEvent.Name,
                    Description = currentEvent.Description,
                    Date = currentEvent.Date,
                    User = currentEvent.User, // Assuming this is automatically set to the current user elsewhere
                    Address = currentEvent.Address,
                    StartTime = currentEvent.StartTime,
                    EndTime = currentEvent.EndTime,
                    Church = currentEvent.Church,
                    Link = currentEvent.Link
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        // BUG : Currently not working... Cannot save changes to events
        public async Task<IActionResult> OnPostUpdateEvent()
        {
            byte[] processedImage = new byte[5];
            var user = await _userManager.GetUserAsync(User);
            
            // Assign EventID to the object
            Input.ID = EventID;
            
            try
            {
                if (Input.Image != null) {
                    processedImage = await _ImageRepository.ConvertToByteArray(Input.Image);
                }

                else if (Input.Image == null) {
                    processedImage = await _ImageRepository.ConvertToByteArrayFromUrl("https://i.imgur.com/oLC9RcU.png");
                }

                await _eventController.UpdateEventAsync(user ,Input, processedImage).ConfigureAwait(true);

                StatusMessage = $"Event ({Input.Name}) successfully updated";
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Page();
        }
    }
}