using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;

namespace C8.TradeLicense.DataAccessLayer
{
    /*
     * Database Initialisers:
     * There are three basic db initialisers:
     * a) CreateDatabaseIfNotExists.
     * b) DropCreateDatabaseIfModelChanges
     * c) DropCreateDatabaseAlways
     * 
     * Db initialisers are called when the database is being accessed for the first time on any call to the context.
     * To tell EF to use this initialiser class, add to the entityFramework element in the root web.config.
     * <entityFramework>
     *     <contexts>
     *           <context type="C8.TradeLicense.DataAccessLayer.TradeLicenseDbContext, C8.TradeLicense">
     *                   <databaseInitializer type="C8.TradeLicense.DataAccessLayer.TradeLicenseDbContext, C8.TradeLicense" />
     *           </context>
     *     </contexts>
     *     
     * When you don't want EF to use the initializer, you can set an attribute on the context element: disableDatabaseInitialization="true".
     * 
     * Troubleshooting:
     * 
     * 1. If adding new EF models to the MVC project and you get a "There is already an object named '<modelName>' in the database." error. 
     * The problem is that the migration initial class is out-dated and needs to be re-created. To solve this run "add-migration initial" command
     * in the package management console. Also check if you may need to delete the database before doing the latter.
     * http://christesene.com/category/entity-framework/
     */
    public class TradeLicenseDbInitialiser : System.Data.Entity.DropCreateDatabaseIfModelChanges<TradeLicenseDbContext>
    {
        protected override void Seed( TradeLicenseDbContext context )
        {
            base.Seed( context );

            try
            {
                // JK.20140726a - Have to pass the same instance of the context to the identity manager, or it will crash (duplicate instance).
                var idManager = new IdentityManager( context );

                // JK.20140916a - Standard user roles for the system.

                if (!idManager.RoleExists("Chief Inspector"))
                    idManager.CreateRole("Chief Inspector");

                if ( !idManager.RoleExists( "Administrator" ) )
                    idManager.CreateRole( "Administrator" );

                if ( !idManager.RoleExists( "Clerk" ) )
                    idManager.CreateRole( "Clerk" );

                if ( !idManager.RoleExists( "Inspector" ) )
                    idManager.CreateRole( "Inspector" );


                if ( !idManager.RoleExists( "Applicant" ) )
                    idManager.CreateRole( "Applicant" );



                var admin = new ApplicationUser()
                {
                    UserName = "Admin",
                    User = new User()
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        Username = "Admin",
                        EmailAddress = "admin@example.com"
                    }
                };

                idManager.CreateUser( admin, "password" );
                idManager.AddUserToRole( admin.Id, "Administrator" );

                var clerk = new ApplicationUser()
                {
                    UserName = "Clerk",
                    User = new User()
                    {
                        FirstName = "Clerk",
                        LastName = "Clerk",
                        Username = "Clerk",
                        EmailAddress = "clerk@example.com"
                    }
                };

                idManager.CreateUser( clerk, "password" );
                idManager.AddUserToRole( clerk.Id, "Clerk" );

                var inspector = new ApplicationUser()
                {
                    UserName = "Inspector",
                    User = new User()
                    {
                        FirstName = "Inspector",
                        LastName = "Inspector",
                        Username = "Inspector",
                        EmailAddress = "inspector@example.com"
                    }
                };

                idManager.CreateUser( inspector, "password" );
                idManager.AddUserToRole( inspector.Id, "Inspector" );

                context.SaveChanges();
            }
            catch ( Exception x )
            {

                throw x;
            }
        }
    }
}