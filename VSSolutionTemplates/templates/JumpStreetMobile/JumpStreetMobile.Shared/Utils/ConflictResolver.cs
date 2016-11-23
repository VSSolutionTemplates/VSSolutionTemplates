using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpStreetMobile.Shared.Utils
{
    public enum ResolverResponse
    {
        Cancel,
        ServerVersion,
        LocalVersion
    }

    // Declaration for conflict resolver that gets called from ExecuteTableOperationAsync() when synchronization conflicts occur
    public delegate Task<ResolverResponse> ConflictResolver(object server, object local);
}
