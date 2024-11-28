using ForestChurches.Components.AutoEvents;
using ForestChurches.Components.ImageHandler;
using ForestChurches.Components.LogReader;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Pages.Admin.Events
{
    [Authorize(Policy = "EventManagement.Edit")]
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;
        private ForestChurchesContext _context;
        private readonly ImageInterface _ImageRepository;
        private readonly EventInterface _eventController;
        private readonly UserManager<ChurchAccount> _userManager;

        public EventsModel currentEvent { get; set; }
        public List<SelectListItem> UserList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public EventsModel Input { get; set; }

        [BindProperty]
        public string SelectedUsername { get; set; }

        public EditModel(ILogger<EditModel> logger, ForestChurchesContext context, ImageInterface imageRepository, EventInterface eventController, UserManager<ChurchAccount> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _ImageRepository = imageRepository;
            _eventController = eventController;
        }

        public void OnGet(string id)
        {
            try
            {
                currentEvent = _context.Events.Where(x => x.ID == Guid.Parse(id)).FirstOrDefault();

                Input = new EventsModel
                {
                    Name = currentEvent.Name,
                    Description = currentEvent.Description,
                    Date = currentEvent.Date,
                    User = currentEvent.User,
                    Address = currentEvent.Address,
                    StartTime = currentEvent.StartTime,
                    EndTime = currentEvent.EndTime,
                    Church = currentEvent.Church,
                    Link = currentEvent.Link,
                    Repeats = currentEvent.Repeats,
                    SelectedUsername = currentEvent.User
                };

                var users = _context.Users.ToList();

                UserList = users.Select(user => new SelectListItem{
                    Text = user.UserName,
                    Value = user.UserName,
                    Selected = user.UserName == currentEvent?.User
                }).ToList();


                // Set SelectedUsername to the current event's user, if available
                SelectedUsername = currentEvent?.User;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<IActionResult> OnPostUpdateEvent(Guid id)
        {
            byte[] processedImage = new byte[5];
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (Input.Image != null)
                {
                    processedImage = await _ImageRepository.ConvertToByteArray(Input.Image);
                }

                else if (Input.Image == null)
                {
                    processedImage = await _ImageRepository.ConvertToByteArrayFromUrl("https://i.imgur.com/oLC9RcU.png");
                }

                Input.ID = id;
                await _eventController.UpdateEventAsync(user, Input, processedImage).ConfigureAwait(true);

                StatusMessage = $"Event ({Input.Name}) successfully updated...";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return RedirectToPage("./Index");
        }
    }
}
