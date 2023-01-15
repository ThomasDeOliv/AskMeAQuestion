using System.Collections.Generic;

namespace AskMeAQuestion.Services.ViewModel
{
    public class PollParticipationViewModel
    {
        public int PollId { get; set; }
        public int UserId { get; set; }
        public string PollTitle { get; set; }
        public string PollDescription { get; set; }
        public bool PollMultipleAnswers { get; set; }
        public List<ResponseViewModel> Responses { get; set; } = new List<ResponseViewModel>();
        public List<int> PollVotedResponses { get; set; } = new List<int>();
        public string UserName { get; set; }
        public string PollResultKey { get; set; }
        public int PollParticipantsNumber { get; set; }
    }

    public struct ResponseViewModel
    {
        public int ResponseId { get; set; }
        public string ResponseDescription { get; set; }
        public bool ResponseIsSelected { get; set; }
    }

}
