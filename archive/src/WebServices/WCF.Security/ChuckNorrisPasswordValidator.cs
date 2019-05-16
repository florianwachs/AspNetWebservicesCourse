using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;

namespace WCF.Security
{
    public class ChuckNorrisPasswordValidator : UserNamePasswordValidator
    {
        public override void Validate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new SecurityTokenException("username and password required");
            }

            if (username == "Chuck" && password != "Norris")
            {
                throw new SecurityTokenException("username and password required");
            }
        }
    }
}