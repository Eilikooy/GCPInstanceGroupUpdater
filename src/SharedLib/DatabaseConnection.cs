using Microsoft.EntityFrameworkCore;
using SharedLib.Models;

namespace SharedLib
{
    public class DatabaseConnection : DbContext
    {
        public DatabaseConnection(DbContextOptions<DatabaseConnection> options) 
            : base(options) 
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DbSet<TemplateUpdate> TemplateUpdate { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
            modelBuilder.Entity<TemplateUpdate>().HasData(new TemplateUpdate {
                Id = 1, 
                FriendlyName = "Test",
                Project = "my-project",
                Region = "europe-north1",
                Zone = "europe-north1-a",
                TemplateInstanceName = "template-vm",
                InstanceGroupName = "my-app",
                SourceTemplateName = "my-app",
                TemplatePrefix = "my-app-template",
                ImagePrefix = "my-app-image",
                TemplateImageFamily = "my-app",
                SshCommand = "sudo apt-get update && sudo apt-get upgrade -y"
            });
            */
            base.OnModelCreating(modelBuilder);
        }
    }
}
