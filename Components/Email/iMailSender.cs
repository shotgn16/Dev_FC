using ForestChurches.Models;

namespace ForestChurches.Components.Email
{
    public interface iMailSender
    {
        void SendEmailEventDeleted(Guid uid, string un, string redirectUrl, EventsModel deletedEvent);
        void SendEmailEventCreated(Guid uid, string un, string redirectUrl, string repeats ,EventsModel addedEvent);
        void SendEmailContactRequest(Guid uid, string un, string recipientEmail, string senderEmail, string msg, string redirectUrl);
        void SendEmailConfirmAccount(Guid uid, string un, string redirectUrl);
        void SendEmailRegisteredUser(Guid uid, string un, string redirectUrl);
        void SendEmailEventValidate(Guid uid, string un, string redirectUrl, EventsModel eventItem);
        void SendEmailResetPassword(Guid uid, string un, string redirectUrl);
    }
}
