using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WorkManager.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public override bool Equals(object obj)
        {
            var z = obj as Project;
            return (z != null) && (z.Owner.Id == this.Id);
        }
    }
}
