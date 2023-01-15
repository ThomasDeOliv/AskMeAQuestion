using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.Data.Model
{
    public class Poll
    {
        [Key]
        public int PollId { get; set; }

        [Required]
        [StringLength(maximumLength: 120)]
        public string PollTitle { get; set; }

        [Required]
        [StringLength(maximumLength: 255)]
        public string PollDescription { get; set; }

        [Required]
        public DateTime PollCreationDate { get; set; }

        public DateTime? PollClosingDate { get; set; }

        [Required]
        public bool PollMultipleAnswers { get; set; }

        [Required]
        [StringLength(maximumLength: 36)]
        public string PollClosingKey { get; set; }

        [Required]
        [StringLength(maximumLength: 36)]
        public string PollResultKey { get; set; }

        [Required]
        [StringLength(maximumLength: 36)]
        public string PollParticipationKey { get; set; }

        // Naviguation
        public User User { get; set; }

        public int? UserId { get; set; }

        public List<Participant> Participants { get; set; } = new List<Participant>();

        public List<Response> Responses { get; set; } = new List<Response>();
    }
}
