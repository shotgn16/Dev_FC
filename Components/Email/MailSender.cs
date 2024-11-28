using ForestChurches.Components.Token;
using ForestChurches.Components.Users;
using ForestChurches.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;
using System.Security.Policy;
using System.Text.Encodings.Web;

namespace ForestChurches.Components.Email
{
    public class MailSender : iMailSender
    {
        private readonly iEmail _mailRepository;
        private readonly ILogger<MailSender> _logger;
        private readonly CallbackTokenDataFormat _tokenDataFormat;

        Dictionary<string, string> UserData;
        
        public MailSender(iEmail mailRepository, ILogger<MailSender> logger, IDataProtectionProvider dataProtectionProvider)
        {
            _mailRepository = mailRepository;
            _logger = logger;

            _tokenDataFormat = new CallbackTokenDataFormat(dataProtectionProvider, "User  Registration");
        }

        internal string CreateCallback(Guid uid, DateTime expires, string redirectUrl)
        {
            CallbackToken token = new CallbackToken
            {
                uid = uid,
                ExpirationTime = expires,
                RedirectUrl = redirectUrl
            };

            return _tokenDataFormat.Protect(token);
        }


        public void SendEmailEventDeleted(Guid uid, string un, string redirectUrl ,EventsModel deletedEvent)
        {
            try {
                var token = CreateCallback(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                    { "{event_link}", token },
                    { "{eventName}", deletedEvent.Name },
                    { "{eventDate}", deletedEvent.Date.ToString() },
                    { "{eventStart}", deletedEvent.StartTime.ToString() },
                    { "{eventEnd}", deletedEvent.EndTime.ToString() }
                };
                _mailRepository.StartEmailAsync(un, UserData, "Event Deleted", "./templates/event_deleted.html");
            }

            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailEventCreated(Guid uid, string un, string redirectUrl, string repeats, EventsModel addedEvent)
        {
            try {
                var token = CreateCallback(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                        { "{event_name}", addedEvent.Name },
                        { "{event_description}", addedEvent.Description },
                        { "{event_location}", addedEvent.Address },
                        { "{event_date}", addedEvent.Date.ToString() },
                        { "{event_startTime}", addedEvent.StartTime.ToString() },
                        { "{event_endTime}", addedEvent.EndTime.ToString() },
                        { "{event_repeats", repeats }
                    };
                _mailRepository.StartEmailAsync(un, UserData, "Event Created", "./templates/event_created.html");
            }

            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailEventValidate(Guid uid, string un, string redirectUrl, EventsModel eventItem)
        {
            try {
                var token = CreateCallback(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                        { "{eventName}", eventItem.Name },
                        { "{eventDate}", eventItem.Date.ToString() },
                        { "{eventStart}", eventItem.StartTime.ToString() },
                        { "{eventEnd}", eventItem.EndTime.ToString() },
                        { "{event_link}", token }
                    };
                _mailRepository.StartEmailAsync(un, UserData, "Do you still need this?", "./templates/confirm_repeated_event.html");
            }

            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailContactRequest(Guid uid, string un, string email, string msg, string redirectUrl)
        {
            try {
                var token = CreateCallback(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                        { "{template_title}", "Forest Churches Contact Form" },
                        { "{template_content}", $"Name: {un} <br /><br /> Email: {email} <br /><br /> Message: {msg}" },
                        { "{template_button_name}", "View Messages" },
                        { "{template_link}", token }
                    };
                _mailRepository.StartEmailAsync("support@forestchurches.co.uk", UserData, "Forest Churches Contact Form", "./templates/admin_email.html");
            }

            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailConfirmAccount(Guid uid, string un, string redirectUrl)
        {
            try {
                var token = CreateCallback(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                        { "{confirm_email_link}", token }
                    };
                _mailRepository.StartEmailAsync(un, UserData, "Confirm your account", "./templates/confirm_email.html");
            }

            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailRegisteredUser(Guid uid, string un, string redirectUrl)
        {
            try {
                var token = CreateCallback(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                        { "{username}", un },
                        { "{user_account_link}", token }
                    };
                _mailRepository.StartEmailAsync(un, UserData, "Thanks for Registering", "./templates/registration_complete.html");
            }

            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailResetPassword(Guid uid, string un, string redirectUrl)
        {
            try {
                var token = CreateCallback(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                        { "{password_reset_link}", token } 
                    };
                _mailRepository.StartEmailAsync(un, UserData, "Reset Password", "./templates/password_reset.html");
            }

            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }
    }
}
