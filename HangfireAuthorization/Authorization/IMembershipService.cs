using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireAuthorization.Authorization
{
    public interface IMembershipService
    {
        Task<bool> ReadUserInGroupsAsync(string userSub, IEnumerable<string> groupIds, CancellationToken cancellationToken = default);
    }
}