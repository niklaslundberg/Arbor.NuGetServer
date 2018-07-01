using System;

namespace Arbor.NuGetServer.Abstractions
{
    public class NuGetTenantId : IEquatable<NuGetTenantId>, IComparable<NuGetTenantId>
    {
        public int CompareTo(NuGetTenantId other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return string.Compare(TenantId, other.TenantId, StringComparison.Ordinal);
        }

        public NuGetTenantId(string tenantId)
        {
            TenantId = tenantId;
        }

        public string TenantId { get; }

        public static bool operator ==(NuGetTenantId left, NuGetTenantId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NuGetTenantId left, NuGetTenantId right)
        {
            return !Equals(left, right);
        }

        public bool Equals(NuGetTenantId other)
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

            return Equals((NuGetTenantId)obj);
        }

        public override int GetHashCode()
        {
            return (TenantId != null ? TenantId.GetHashCode() : 0);
        }
    }
}
