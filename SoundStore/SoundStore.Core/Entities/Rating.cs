using SoundStore.Core.Commons;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundStore.Core.Entities
{
    public class Rating : IEntity<long>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Point is in range 1 to 5
        /// </summary>
        public int RatingPoint { get; set; }

        public string? Comment { get; set; }
    }
}
