using Azure.Core;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        Dictionary<string, string> UserData;

        public MailSender(iEmail mailRepository, ILogger<MailSender> logger, IDataProtectionProvider dataProtectionProvider, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _mailRepository = mailRepository;
            _logger = logger;

            _tokenDataFormat = new CallbackTokenDataFormat(dataProtectionProvider, "User  Registration");
        }

        public void SendEmailEventDeleted(Guid uid, string un, string redirectUrl, EventsModel deletedEvent)
        {
            try
            {
                var token = GenerateUniqueToken(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                    { "{event_link}", GenerateEmailLink(token) },
                    { "{eventName}", deletedEvent.Name },
                    { "{eventDate}", deletedEvent.Date.ToString() },
                    { "{eventStart}", deletedEvent.StartTime.ToString() },
                    { "{eventEnd}", deletedEvent.EndTime.ToString() }
                };
                _mailRepository.StartEmailAsync(un, UserData, "Event Deleted", "./templates/event_deleted.html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailEventCreated(Guid uid, string un, string redirectUrl, string repeats, EventsModel addedEvent)
        {
            try
            {
                var token = GenerateUniqueToken(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                        { "{event_name}", addedEvent.Name },
                        { "{event_description}", addedEvent.Description },
                        { "{event_location}", addedEvent.Address },
                        { "{event_date}", addedEvent.Date.ToString() },
                        { "{event_startTime}", addedEvent.StartTime.ToString() },
                        { "{event_endTime}", addedEvent.EndTime.ToString() },
                        { "{event_repeats}", repeats },
                        { "{event_link}", GenerateEmailLink(token) }
                    };
                _mailRepository.StartEmailAsync(un, UserData, "Event Created", "./templates/event_created.html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailEventValidate(Guid uid, string un, string redirectUrl, EventsModel eventItem)
        {
            try
            {
                var token = GenerateUniqueToken(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                        { "{eventName}", eventItem.Name },
                        { "{eventDate}", eventItem.Date.ToString() },
                        { "{eventStart}", eventItem.StartTime.ToString() },
                        { "{eventEnd}", eventItem.EndTime.ToString() },
                        { "{event_link}", GenerateEmailLink(token) }
                    };
                _mailRepository.StartEmailAsync(un, UserData, "Do you still need this?", "./templates/confirm_repeated_event.html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailContactRequest(Guid uid, string un, string recipientEmail, string senderEmail, string msg, string redirectUrl)
        {
            try
            {
                var token = GenerateUniqueToken(uid, DateTime.UtcNow.AddDays(1), redirectUrl);

                UserData = new Dictionary<string, string> {
                        { "{template_title}", "Forest Churches Contact Form" },
                        { "{template_content}", $"Name: {un} <br /><br /> Email: {senderEmail} <br /><br /> Message: {msg}" },
                        { "{template_button_name}", "View Messages" },
                        { "{template_link}", GenerateEmailLink(token) }
                    };
                _mailRepository.StartEmailAsync(recipientEmail, UserData, "Forest Churches Contact Form", "./templates/admin_email.html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailConfirmAccount(Guid uid, string un, string redirectUrl)
        {
            try
            {
                var token = GenerateUniqueToken(uid, DateTime.UtcNow.AddDays(1), redirectUrl);
                var emailLink = GenerateEmailLink(token);

                UserData = new Dictionary<string, string> {
                        { "{confirm_email_link}", emailLink }
                    };
                _mailRepository.StartEmailAsync(un, UserData, "Confirm your account", "./templates/confirm_email.html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailRegisteredUser(Guid uid, string un, string redirectUrl)
        {
            try
            {
                var token = GenerateUniqueToken(uid, DateTime.UtcNow.AddDays(1), redirectUrl);
                var emailLink = GenerateEmailLink(token);

                UserData = new Dictionary<string, string> {
                        { "{username}", un },
                        { "{user_account_link}", emailLink }
                    };
                _mailRepository.StartEmailAsync(un, UserData, "Thanks for Registering", "./templates/registration_complete.html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailResetPassword(Guid uid, string un, string redirectUrl)
        {
            try
            {
                var token = GenerateUniqueToken(uid, DateTime.UtcNow.AddDays(1), redirectUrl);
                var emailLink = GenerateEmailLink(token);

                UserData = new Dictionary<string, string> 
                {
                    { "{password_reset_link}", emailLink }
                };
                _mailRepository.StartEmailAsync(un, UserData, "Reset Password", "./templates/password_reset.html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public void SendEmailInviteUser(string invitedEmail, string redirectUrl)
        {
            try
            {
                var token = GenerateUniqueToken(invitedEmail, DateTime.UtcNow.AddDays(1), redirectUrl);
                var emailLink = GenerateEmailLink(token);

                UserData = new Dictionary<string, string>
                {
                    { "{email}", invitedEmail },
                    { "{registration_link}", emailLink }
                };

                _mailRepository.StartEmailAsync(invitedEmail, UserData, "You're Invited", "./templates/registration_invite.html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        private string GenerateUniqueToken(Guid uid, DateTime expires, string redirectUrl)
        {
            CallbackToken token = new CallbackToken
            {
                uid = uid,
                ExpirationTime = expires,
                RedirectUrl = redirectUrl
            };

            return _tokenDataFormat.Protect(token);
        }

        private string GenerateUniqueToken(string email, DateTime expires, string redirectUrl)
        {
            CallbackToken token = new CallbackToken
            {
                uid = Guid.Empty,
                email = email,
                ExpirationTime = expires,
                RedirectUrl = redirectUrl
            };

            return _tokenDataFormat.Protect(token);
        }

        private string GenerateEmailLink(string token)
        {
            var request = _httpContextAccessor.HttpContext.Request;

            var baseUrl = $"{request.Scheme}://{request.Host.Value}/ValidateToken";
            return $"{baseUrl}?token={token}";
        }

        public bool ValidateCallback(string token, out Guid uid, out string redirectUrl)
        {
            try
            {
                var callbackToken = _tokenDataFormat.Unprotect(token);
                if (callbackToken.ExpirationTime > DateTime.UtcNow)
                {
                    uid = callbackToken.uid;
                    redirectUrl = callbackToken.RedirectUrl;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            uid = Guid.Empty;
            redirectUrl = string.Empty;
            return false;
        }
    }
}
