using ForestChurches.Components.AutoEvents;
using ForestChurches.Components.ImageHandler;
using ForestChurches.Components.LogReader;
using ForestChurches.Components.Roles;
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
    [Authorize(Policy = "EventManagement.Write")]
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly UserManager<ChurchAccount> _userManager;
        private readonly ForestChurchesContext _context;
        private ImageInterface _iImage;
        private EventInterface _eventController;

        [BindProperty]
        public int RepetitionFrequency { get; set; }

        [BindProperty]
        public EventsModel Input { get; set; }
        public IList<ChurchRoles> Roles;

        [BindProperty]
        public string SelectedUsername { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public List<SelectListItem> UserList { get; set; }
        public Guid EventID { get; set; }

        public CreateModel(
            ILogger<CreateModel> logger, 
            ImageInterface iImage, 
            UserManager<ChurchAccount> userManager, 
            ForestChurchesContext context,
            EventInterface eventController)
        {
            _logger = logger;
            _iImage = iImage;
            _userManager = userManager;
            _context = context;
            _eventController = eventController;

            var users = _userManager.Users.ToList();
            UserList = users.Select(user => new SelectListItem
            {
                Text = user.UserName,
                Value = user.UserName
            }).ToList();

            Input = new EventsModel();
        }

        public void OnGet(string eventName)
        {
            Input.Name = eventName;
        }

        public async Task<ActionResult> OnPostCreateEvent()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(SelectedUsername);
                var church = _context.ChurchInformation.Where(i => i.ChurchAccountId == user.Id).FirstOrDefault();

                if (Input != null)
                {
                    if (Input.Image != null)
                    {
                        byte[] Image = await _iImage.ConvertToByteArray(Input.Image);

                        _context.Events.Add(new EventsModel 
                        {
                            Name = Input.Name,
                            Description = Input.Description,
                            Address = Input.Address,
                            Date = Input.Date,
                            Repeats = Input.Repeats,
                            StartTime = Input.StartTime,
                            EndTime = Input.EndTime,
                            Link = Input.Link,
                            ImageArray = Image,
                            User = user.UserName,
                            Church = church.Name
                        });

                        _context.SaveChanges();

                        EventID = _context.Events.Where(a => a.ID == Input.ID).Select(a => a.ID).FirstOrDefault();
                        StatusMessage = EventID != Guid.Empty ? "New Event Successfullly Created!" : "An Error occurred while submitting your event. ";
                    }

                    else if (Input.Image == null)
                    {
                        byte[] Image = await _iImage.ConvertToByteArrayFromUrl("https://i.imgur.com/1yxyCKl.png");

                        _context.Events.Add(new EventsModel
                        {
                            Name = Input.Name,
                            Description = Input.Description,
                            Address = Input.Address,
                            Date = Input.Date,
                            Repeats = Input.Repeats,
                            StartTime = Input.StartTime,
                            EndTime = Input.EndTime,
                            Link = Input.Link,
                            ImageArray = Image,
                            User = user.UserName,
                            Church = church.Name
                        });

                        EventID = _context.Events.Where(a => a.ID == Input.ID).Select(a => a.ID).FirstOrDefault();
                            StatusMessage = EventID != Guid.Empty ? "New Event Successfullly Created!" : "An Error occurred while submitting your event. ";

                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                StatusMessage = "Error: " + ex.Message;
            }

            return RedirectToPage("./Index");
        }
    }
}
