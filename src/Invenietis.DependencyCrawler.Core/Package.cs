using System;

namespace Invenietis.DependencyCrawler.Core
{
    public class Package
    {
        public Package( string name, VPackage lastRelease )
            : this( name, lastRelease, null )
        {
        }

        public Package( string name, VPackage lastRelease, VPackage lastPreRelease )
            : this( name, lastRelease, lastPreRelease, false )
        {
        }

        public Package( string name, VPackage lastRelease, VPackage lastPreRelease, bool isNotFound )
        {
            if( isNotFound && ( lastRelease != null || lastPreRelease != null ) ) throw new ArgumentException();

            Name = name;
            LastRelease = lastRelease;
            LastPreRelease = lastPreRelease;
        }

        public Package( string name )
            : this( name, null, null, true )
        {
        }

        public string Name { get; }

        public VPackage LastRelease { get; }

        public VPackage LastPreRelease { get; }

        public bool IsNotFound { get; }

        public override bool Equals( object obj )
        {
            Package other = obj as Package;
            return other != null
                && other.Name == Name
                && other.IsNotFound == IsNotFound
                && ( ( other.LastRelease == null && LastRelease == null ) || other.LastRelease.Equals( LastRelease ) )
                && ( ( other.LastPreRelease == null && LastPreRelease == null ) || other.LastPreRelease.Equals( LastPreRelease ) );
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() << 11
                ^ IsNotFound.GetHashCode() << 7
                ^ LastRelease.GetHashCode() << 3
                ^ LastPreRelease.GetHashCode();
        }
    }
}