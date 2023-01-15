using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.Data.Model
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(maximumLength: 320)]
        public string UserMail { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        public string UserLogin { get; set; }

        [Required]
        [StringLength(maximumLength: 255)]
        public string UserPassword { get; set; }

        [StringLength(maximumLength: 36)]
        public string UserSession { get; set; }

        // Naviguation
        public List<Vote> Votes { get; set; } = new List<Vote>();
        public List<Poll> Polls { get; set; } = new List<Poll>();
    }
}
