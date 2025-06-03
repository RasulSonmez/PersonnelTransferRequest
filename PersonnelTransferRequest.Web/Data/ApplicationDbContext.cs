using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Models;

namespace PersonnelTransferRequest.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Title> Titles { get; set; }
        public DbSet<TransferRequest> TransferRequests { get; set; }
        public DbSet<TransferPreference> TransferPreferences { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<SupportMessage> SupportMessages { get; set; }
        public DbSet<SupportMessageReply> SupportMessageReplies { get; set; }
    }
}
