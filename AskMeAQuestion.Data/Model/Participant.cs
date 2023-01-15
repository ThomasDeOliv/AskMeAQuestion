using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.Data.Model
{
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }

        [Required]
        [StringLength(maximumLength: 320)]
        public string ParticipantMail { get; set; }

        public bool ParticipantAlreadyVoted { get; set; }

        // Naviguation
        public Poll Poll { get; set; }
        public int PollId { get; set; }
    }
}
