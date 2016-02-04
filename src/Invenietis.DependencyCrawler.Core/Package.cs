using System;

namespace Invenietis.DependencyCrawler.Core
{
    public class Package
    {
        public Package( string packageManager, string id, VPackage lastRelease )
            : this( packageManager, id, lastRelease, null )
        {
        }

        public Package( PackageId id, VPackage lastRelease )
            : this( id, lastRelease, null )
        {
        }

        public Package( string packageManager, string id, VPackage lastRelease, VPackage lastPreRelease )
            : this( packageManager, id, lastRelease, lastPreRelease, false )
        {
        }

        public Package( PackageId id, VPackage lastRelease, VPackage lastPreRelease )
            : this( id, lastRelease, lastPreRelease, false )
        {
        }

        public Package( string packageManager, string id, VPackage lastRelease, VPackage lastPreRelease, bool isNotFound )
            : this( new PackageId( packageManager, id ), lastRelease, lastPreRelease, isNotFound )
        {
        }

        public Package( PackageId id, VPackage lastRelease, VPackage lastPreRelease, bool isNotFound )
        {
            if( isNotFound && ( lastRelease != null || lastPreRelease != null ) ) throw new ArgumentException();

            Id = id;
            LastRelease = lastRelease;
            LastPreRelease = lastPreRelease;
        }

        public Package( string packageManager, string id )
            : this( packageManager, id, null, null, true )
        {
        }

        public Package( PackageId id )
            : this( id, null, null, true )
        {
        }

        public PackageId Id { get; }

        public VPackage LastRelease { get; }

        public VPackage LastPreRelease { get; }

        public bool IsNotFound { get; }

        public override bool Equals( object obj )
        {
            Package other = obj as Package;
            return other != null
                && other.Id == Id
                && other.IsNotFound == IsNotFound
                && other.LastRelease == LastRelease
                && other.LastPreRelease == LastPreRelease;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() << 11
                ^ IsNotFound.GetHashCode() << 7
                ^ LastRelease.GetHashCode() << 3
                ^ LastPreRelease.GetHashCode();
        }
    }
}