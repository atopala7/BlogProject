using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace WebApplication4.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public virtual ICollection<ToDo> ToDoes { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Blogger { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
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

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<WebApplication4.Models.ToDo> ToDoes { get; set; }

        public System.Data.Entity.DbSet<WebApplication4.Models.Post> Posts { get; set; }

       // public System.Data.Entity.DbSet<WebApplication4.Models.PostViewModel> PostViewModels { get; set; }

        public System.Data.Entity.DbSet<WebApplication4.ViewModels.PostAddForm> PostAddForms { get; set; }

        public System.Data.Entity.DbSet<WebApplication4.ViewModels.CommentAddForm> CommentAddForms { get; set; }

    }
}