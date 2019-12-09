using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Day9.Interfaces
{
    public interface IGitHubIssueUpdate
    {
        Task AddCommentToIssue(long repositoryId, int issueNumber, string issueCreator);
    }
}
