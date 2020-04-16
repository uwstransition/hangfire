using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireAuthorization.Authorization
{
    public class MembershipService : IMembershipService
    {
        public Task<bool> ReadUserInGroupsAsync(string userSub, IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            //This is in the package
            return Task.FromResult(userSub == "11");
        }
    }
}