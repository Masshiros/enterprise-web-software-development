﻿namespace Server.Contracts.Identity.Users
{
    public class ValidateForgotTokenRequest
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
