using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireAuthorization.Authorization
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private IMembershipService _membershipService;
        private AuthorizationSettings _authorizationSettings;

        public HangfireAuthorizationFilter(IMembershipService membershipService, AuthorizationSettings authorizationSettings)
        {
            _membershipService = membershipService;
            _authorizationSettings = authorizationSettings;
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            if(!context.GetHttpContext().User.Identity.IsAuthenticated)
            {
                return false;
            }

            var userSub = context.GetHttpContext().User.FindFirst("sub").Value;

            return new JoinableTaskFactory(new JoinableTaskContext()).Run(async delegate
            {
                return await _membershipService.ReadUserInGroupsAsync(userSub, new[] { _authorizationSettings.HangfireAuthorizationGroup });
            });
        }
    }
}
