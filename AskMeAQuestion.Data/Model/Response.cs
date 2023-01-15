using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.Data.Model
{
    public class Response
    {
        [Key]
        public int ResponseId { get; set; }
        [Required]
        [StringLength(maximumLength: 120)]
        public string ResponseDescription { get; set; }


        // Naviguation
        public Poll Poll { get; set; }
        public int PollId { get; set; }
        public List<Vote> Votes { get; set; } = new List<Vote>();
    }
}
