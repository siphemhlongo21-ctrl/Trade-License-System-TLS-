using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace C8.TradeLicense.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        /* The ForeignKeyAttribute constructor takes a string as a parameter: if you place it on a 
         * foreign key property it represents the name of the associated navigation property. If you 
         * place it on the navigation property it represents the name of the associated foreign key.
         */
        public int UserId { get; set; }

        [ForeignKey( "UserId" )]
        public virtual User User { get; set; }
    }
}