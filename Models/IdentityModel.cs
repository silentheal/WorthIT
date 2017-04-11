namespace WorthIt.Models
{

    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Linq;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public class IdentityModel : IdentityDbContext<User>
    {
        public IdentityModel() : base("name=WorthITLogin")
        {

        }
    }

    public class User : IdentityUser
    {

    }
}