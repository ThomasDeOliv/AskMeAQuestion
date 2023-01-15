using System;

namespace AskMeAQuestion.Services.ViewModel
{
    public class PollGetLinksViewModel
    {
        public string PollTitle { get; set; }
        public string PollParticipationKey { get; set; }
        public string PollResultKey { get; set; }
        public string PollClosingKey { get; set; }
        public string PollParticipationLink { get; set; }
        public string PollResultLink { get; set; }
        public string PollClosingLink { get; set; }
        public DateTime? PollClosingDate { get; set; }
        public string UserName { get; set; }
    }
}
