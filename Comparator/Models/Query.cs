using System.ComponentModel.DataAnnotations;

namespace Comparator.Models
{
    public class Query
    {
        // TODO: Specify content
        [Required]
        public string Keywords { get; set; }
    }
}