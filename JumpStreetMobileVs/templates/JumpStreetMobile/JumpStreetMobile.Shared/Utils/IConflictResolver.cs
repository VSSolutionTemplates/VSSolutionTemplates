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

    public interface IConflictResolver
    {
        Task<ResolverResponse> Resolve(object server, object local);
    }
}
