using System.ComponentModel.DataAnnotations;

namespace Comparator.Models
{
    public class Query
    {
        [Required]
        public string Keywords { get; set; }
    }
}