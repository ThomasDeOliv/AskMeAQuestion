using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.Data.Model
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }

        // Naviguation
        public Response Response { get; set; }
        public int ResponseId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
