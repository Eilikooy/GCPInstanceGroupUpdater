using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLib.Models
{
    [Index(nameof(FriendlyName), IsUnique = true)]
    public class TemplateUpdate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string FriendlyName { get; set; } = string.Empty;
        [Required]
        public string Project { get; set; } = string.Empty;
        [Required]
        public string Region { get; set; } = string.Empty;
        [Required]
        public string Zone { get; set; } = string.Empty;
        [Required]
        public string TemplateInstanceName { get; set; } = string.Empty;
        [Required]
        public string ImagePrefix { get; set; } = string.Empty;
        [Required]
        public string SourceTemplateName { get; set; } = string.Empty;
        [Required]
        public string TemplatePrefix { get; set; } = string.Empty;
        [Required]
        public string InstanceGroupName { get; set; } = string.Empty;
        [Required]
        public string TemplateImageFamily { get; set; } = string.Empty;
        [Required]
        public string SshCommand { get; set; } = string.Empty;

    }
}
