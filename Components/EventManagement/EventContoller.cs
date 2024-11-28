using ForestChurches.Components.Email;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Components.AutoEvents
{
    public class EventContoller : EventInterface
    {
        // Dependency Injection
        private ILogger<EventContoller> _logger;
        private ForestChurchesContext _context;
        private UserManager<ChurchAccount> _userManager;
        private readonly iMailSender _mailSender;

        // Property Delcaration
        public List<EventsModel> Events { get; set; }
        public List<EventsModel> EventsToDelete { get; set; }
        public List<EventsModel> RepeatedEvents { get; set; }
        public List<EventsModel> RepeatedEventsToDelete { get; set; }

        public EventContoller(ForestChurchesContext context, iMailSender mailSender, UserManager<ChurchAccount> userManager, ILogger<EventContoller> logger)
        {
            _logger = logger;
            _context = context;
            _mailSender = mailSender;
            _userManager = userManager;

            // Call the GetEvents method to populate the Events property
            GetEvents();
        }

        public async Task GetEvents()
        {
            Events = _context.Events
                .Where(a => a.Repeats == "None")
                .ToList();
        }

        public async Task GetRepeatedEvents()
        {
            RepeatedEvents = _context.Events
                .Where(a => a.Repeats != "None")
                .ToList();
        }

        public async Task SortAndDeleteEvents()
        {
            foreach (var item in Events)
            {
                var tempDateTime = item.Date.ToDateTime(item.EndTime);

                if (DateTime.Now.AddHours(24) >= tempDateTime)
                {
                    EventsToDelete.Add(item);
                }
            }
        }

        public async Task InformUsersAndDeleteEvents(ChurchAccount User, List<EventsModel> eventsToDelete)
        {
            foreach (var item in eventsToDelete)
            {
                _context.Events.Remove(item);

                //var userData = new Dictionary<string, string>()
                //{ 
                //    { "{eventName}", item.Name },
                //    { "{eventDate}", item.Date.ToString() },
                //    { "{eventStart}", item.StartTime.ToString() },
                //    { "{eventEnd}", item.EndTime.ToString() }
                //};

                _mailSender.SendEmailEventDeleted(Guid.Parse(User.Id), User.Email, "/Events", item);
            }

            _context.SaveChanges();
        }

        // Task : Check auto-event management works
        public async Task CheckRepeatedEvents(ChurchAccount User)
        {
            foreach (var item in RepeatedEvents)
            {
                var user = _userManager.FindByNameAsync(item.User).Result;
                var tempDateTime = item.Date.ToDateTime(item.EndTime);

                if (DateTime.Now.AddDays(14) >= tempDateTime)
                {
                    //var callbackUrl = Url.Page(
                    //    "/Events",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    // Send Email
                    _mailSender.SendEmailEventValidate(Guid.Parse(User.Id), User.UserName, "/Events" ,item);
                }
            }
        }

        // SWITCHING TO DIFFERENT TYPE OF FUNCTIONS NOW

        public async Task UpdateEventAsync(ChurchAccount User ,EventsModel input, Byte[] Image)
        {
            try
            {
                var currentChurch = _context.ChurchInformation.Where(a => a.ChurchAccountId == User.Id).FirstOrDefault();

                _context.Events.Where(a => a.ID == input.ID).ExecuteUpdate(setters => setters
                    .SetProperty(p => p.User, User.Email)
                    .SetProperty(p => p.Church, currentChurch.Name)
                    .SetProperty(p => p.Repeats, input.Repeats)
                    .SetProperty(p => p.Address, input.Address)
                    .SetProperty(p => p.StartTime, input.StartTime)
                    .SetProperty(p => p.EndTime, input.EndTime)
                    .SetProperty(p => p.Date, input.Date)
                    .SetProperty(p => p.Description, input.Description)
                    .SetProperty(p => p.ImageArray, Image)
                    .SetProperty(p => p.Link, input.Link)
                );

                _context.SaveChanges();
            }
             
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }
    }
}
