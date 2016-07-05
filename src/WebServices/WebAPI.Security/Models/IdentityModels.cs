using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using WebAPI.Security.Infrastructure;

namespace WebAPI.Security.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new IdentityInitDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class IdentityInitDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            AddInitUsers(context);
            base.Seed(context);
        }

        private void AddInitUsers(ApplicationDbContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            AddUser(manager, "chuck", "chuck@norris.com", "norris", GetManagerClaims());
            AddUser(manager, "jason", "jason@bourne.com", "bourne", GetSupportClaims());
        }

        private void AddUser(ApplicationUserManager manager, string username, string email, string password, IEnumerable<Claim> claims)
        {
            var user = manager.FindByName(username);
            if (user == null)
            {
                manager.Create(new ApplicationUser { UserName = username, Email = email }, password);
                user = manager.FindByName(username);

                foreach (var claim in claims)
                {
                    manager.AddClaim(user.Id, claim);
                }
            }
        }

        private IEnumerable<Claim> GetManagerClaims()
        {
            yield return CreateClaim(ClaimTypes.PostalCode, "12345");
            yield return CreateClaim(AppClaimTypes.Department, "Customer Management");
            yield return CreateClaim(AppClaimTypes.Permission, AppPermissions.CanCreatePerson);
            yield return CreateClaim(AppClaimTypes.Permission, AppPermissions.CanDeletePerson);
            yield return CreateClaim(AppClaimTypes.Permission, AppPermissions.CanUpdatePerson);
            yield return CreateClaim(AppClaimTypes.Permission, AppPermissions.CanReadPersonAge);
        }

        private IEnumerable<Claim> GetSupportClaims()
        {
            yield return CreateClaim(ClaimTypes.PostalCode, "54321");
            yield return CreateClaim(AppClaimTypes.Department, "Customer Support");
            yield return CreateClaim(AppClaimTypes.Permission, AppPermissions.CanCreatePerson);
            yield return CreateClaim(AppClaimTypes.Permission, AppPermissions.CanUpdatePerson);
        }

        private Claim CreateClaim(string type, string value)
        {
            return new Claim(type, value, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer);
        }
    }
}