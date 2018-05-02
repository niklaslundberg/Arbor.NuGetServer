using System;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class NuGetTenant : IEquatable<NuGetTenant>
    {
        public NuGetTenant(string tenantId)
        {
            TenantId = tenantId;
        }

        public string TenantId { get; }

        public static bool operator ==(NuGetTenant left, NuGetTenant right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NuGetTenant left, NuGetTenant right)
        {
            return !Equals(left, right);
        }

        public bool Equals(NuGetTenant other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(TenantId, other.TenantId);
        }

        public override string ToString()
        {
            return $"{nameof(TenantId)}: {TenantId}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((NuGetTenant)obj);
        }

        public override int GetHashCode()
        {
            return (TenantId != null ? TenantId.GetHashCode() : 0);
        }
    }
}
