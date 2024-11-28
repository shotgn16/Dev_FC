﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System;

namespace ForestChurches.Components.Token
{
    public class CallbackToken
    {
        public Guid uid { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class CallbackTokenDataFormat : ISecureDataFormat<CallbackToken>
    {
        private readonly IDataProtector dataProtector;

        public CallbackTokenDataFormat(IDataProtectionProvider dataProtectionProvider, string purpose)
        {
            dataProtector = dataProtectionProvider.CreateProtector(purpose);
        }

        public string Protect(CallbackToken data)
        {
            var json = JsonConvert.SerializeObject(data);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var protectedTokenBytes = dataProtector.Protect(bytes);
            return Convert.ToBase64String(protectedTokenBytes);
        }

        public string Protect(CallbackToken data, string? purpose)
        {
            // The purpose parameter is ignored in this implementation
            return Protect(data);
        }

        public CallbackToken Unprotect(string protectedText)
        {
            var protectedTokenBytes = Convert.FromBase64String(protectedText);
            byte[] bytes = dataProtector.Unprotect(protectedTokenBytes);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<CallbackToken>(json);
        }

        public CallbackToken Unprotect(string protectedText, string? purpose)
        {
            // The purpose parameter is ignored in this implementation
            return Unprotect(protectedText);
        }
    }
}