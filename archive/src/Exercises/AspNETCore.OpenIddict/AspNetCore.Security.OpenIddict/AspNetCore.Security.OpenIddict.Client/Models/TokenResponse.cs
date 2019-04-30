using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Security.OpenIddict.Client.Models
{
    public class TokenResponse
    {
        public string Resource { get; set; }
        public string Scope { get; set; }
        public string Token_Type { get; set; }
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public string Id_Token { get; set; }
        public int Expires_In { get; set; }

    }
}
