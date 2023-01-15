using System;
using System.Collections.Generic;

namespace AskMeAQuestion.Services.ViewModel
{
    public class PollResultsViewModel
    {
        public string PollTitle { get; set; }
        public bool PollEnable { get; set; }
        public string PollDescription { get; set; }
        public bool PollMultipleAnswers { get; set; }
        public int PollNumberOfGuests { get; set; }
        public int PollNumberOfVotes { get; set; }
        public double PollResponseRate { get; set; }
        public int PollNumberOfVotesPerUser { get; set; }
        public List<VoteViewModel> PollVotes { get; set; } = new List<VoteViewModel>();
        public DateTime PollCreationDate { get; set; }
        public DateTime? PollClosingDate { get; set; }
        public string UserName { get; set; }
    }

    public struct VoteViewModel
    {
        public int VoteResponseId { get; set; }
        public string VoteResponseDescription { get; set; }
        public int VoteIgnored { get; set; }
        public int VoteNumber { get; set; }
        public double VotePrctOfVotes { get; set; }
    }
}
