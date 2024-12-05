using ForestChurches.Components.Email;
using ForestChurches.Components.FileManager;
using ForestChurches.Components.Http.Google;
using ForestChurches.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using Microsoft.CodeAnalysis.Scripting;
using ForestChurches.Components.Users;

namespace ForestChurches.Pages
{
    public class NearMeModel : PageModel
    {
        private GoogleInterface _googleAPI;
        private PostcodeValidatorInterface _iPostcode;
        private FileInterface _fileInterface;
        private ILogger<NearMeModel> _logger;
        private ForestChurchesContext _context;
        private iEmail _mailRepository;
        private UserManager<ChurchAccount> _userManager;

        internal List<ChurchInformation> _places;
        internal List<EventsModel> _events;
        internal List<OrganizedEvents> _organisedEvents;
        internal List<string> cid;
        internal string emailAddress;

        [TempData]
        internal bool isAuthenticated { get; set; }
        internal bool ShowPlacesModal { get; set; }
        internal bool ShowEventsModal { get; set; }

        public NearMeModel(
            GoogleInterface GoogleInterface, 
            FileInterface fileInterface, 
            PostcodeValidatorInterface iPostcode, 
            ForestChurchesContext context, 
            iEmail mailRepository, 
            UserManager<ChurchAccount> userManager,
            ILogger<NearMeModel> logger)
        {
            this._googleAPI = GoogleInterface;
            this._context = context;
            this._mailRepository = mailRepository;
            this._fileInterface = fileInterface;
            this._iPostcode = iPostcode;
            this._userManager = userManager;
            this._logger = logger;

            this.cid = new();
            this._places = new();
            this._events = new();
            this._organisedEvents = new();
        }

        // Churches Near Me!
        public async Task<IActionResult> OnPostChurchForm(string postcode)
        {
            try
            {
                postcode = postcode.ToUpper();
                ViewData["postcode"] = postcode;

                //await _iPostcode.ValidatePostcodeAsync(postcode);

                double[] inputCoordinates = await _googleAPI.ConvertToCoordinates(postcode);
                double latitude = inputCoordinates[0];
                double longtitude = inputCoordinates[1];

                // Retrieve all churches from the database
                // WARNING: Currently retrieves ALL church information - MUST BE RESTRICTED
                var churches = await _context.ChurchInformation.Where(s => s.Status == "Open").ToListAsync();

                // Calculate distance and order churches by distance
                var orderedChurches = churches
                    .Select(church => new
                    {
                        Church = church,
                        Distance = HavarsineDistance(latitude, longtitude, inputCoordinates[0], inputCoordinates[1])
                    })
                    .OrderBy(x => x.Distance)
                    .ToList();

                // You can now store the ordered list in a property or directly in the ViewData
                this.cid = orderedChurches.Select(c => c.Church.ID).ToList(); // Assuming Church has an Id property
                _places = orderedChurches.Select(c => c.Church).ToList(); // If you want to keep the Church objects

                ShowPlacesModal = true;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }

            return Page();
        }

        private double HavarsineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of the Earth in kilometers
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Distance in kilometers
        }

        public async Task<IActionResult> OnPostEventForm(string postcode)
        {
            try
            {
                postcode = postcode.ToUpper();
                ViewData["postcode"] = postcode;

                //await _iPostcode.ValidatePostcodeAsync(postcode);

                double[] userCoordinates = await _googleAPI.ConvertToCoordinates(postcode);
                double userLatitude = userCoordinates[0];
                double userLongitude = userCoordinates[1];

                var events = await _context.Events.ToListAsync();
                var eventDistances = new List<(EventsModel Event, double Distance)>();

                foreach (var ev in events)
                {
                    double[] eventCoordinates = await _googleAPI.ConvertToCoordinates(ev.Address);
                    double eventLatitude = eventCoordinates[0];
                    double eventLongitude = eventCoordinates[1];

                    double distance = HavarsineDistance(userLatitude, userLongitude, eventLatitude, eventLongitude);
                    eventDistances.Add((ev, distance));
                }

                var orderedEvents = eventDistances
                    .OrderBy(x => x.Distance)
                    .Select(x => x.Event)
                    .ToList();

                _events = orderedEvents;
                ShowEventsModal = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDownloadInformation(string Id)
        {
            var church = _context.ChurchInformation.Where(a => a.ID == Id).FirstOrDefault();

            return await _fileInterface.CreateFile(church, "text/plain", "Church Information");
        }

        public async Task<IActionResult> OnPostSendInformationWithEmail(string Id, string email = "")
        {
            emailAddress = email;

            // Getting church details from database
            var church = _context.ChurchInformation
                .Include(c => c.ServiceTimes)
                .Where(a => a.ID == Id)
                .FirstOrDefault();

            // Checking if the user is Authenticated or not
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                emailAddress = user?.Email;
            }

            // Ensure email is not null or empty for both authenticated and unauthenticated users
            if (string.IsNullOrEmpty(email))
            {
                // Throw error?
                return Page();
            }

            // Defining empty dictionary object for user data (used in emails)
            Dictionary<string, string> userData = new();

            if (church != null)
            {
                userData = new Dictionary<string, string>()
                {
                    { "{username}", email },
                    { "{church_name}", church?.Name ?? "Not available" },
                    { "{church_address}", church?.Address ?? "Not available" },
                    { "{church_phone}", church?.Phone ?? "Not available" },
                    { "{church_website}", church?.Website ?? "Not available" },
                    { "{church_wheelchairEntrance}", church.WheelchairAccess == null ? "Not available" : (church.WheelchairAccess ? "Available" : "Unavailable") }
                };

                // Concatenate service times into a single string
                string serviceTimesString = string.Join("<br>", church.ServiceTimes.Select(st => $"{st.Time:HH:mm}: {st.Note}"));

                // Add the concatenated service times to userData
                userData["{serviceTimes}"] = serviceTimesString;
            }
            
            await _mailRepository.StartEmailAsync(emailAddress, userData, "Church Information", "./templates/church_information.html");
            TempData["Message"] = $"The information has been sent to '{emailAddress}'. Please be sure to also check your spam folder.";

            return RedirectToPage("/NearMe");
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


