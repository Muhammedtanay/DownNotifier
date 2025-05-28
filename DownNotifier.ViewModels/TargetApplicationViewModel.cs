using System.ComponentModel.DataAnnotations;
namespace DownNotifier.ViewModels
{
    public class TargetApplicationViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, Url]
        public string Url { get; set; }

        [Display(Name = "Kontrol Aralığı (dk)"), Range(1, 1440)]
        public int CheckIntervalInMinutes { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastChecked { get; set; }

        public int LastStatusCode { get; set; }
    }
}
