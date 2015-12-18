using System.Collections.Generic;

namespace Invenietis.DependencySolver.JSon
{
    class DependencyFinder : JSONVisitor
    {
        Dictionary<string, string> _dependencies;
        bool _isParsingDependencies;
        string _currentDependency;

        public DependencyFinder( StringMatcher m )
            : base( m )
        {
        }

        public override bool VisitObjectProperty( int startPropertyIndex, string propertyName, int propertyIndex )
        {
            if( propertyName == "dependencies" )
            {
                _isParsingDependencies = true;
                bool result = base.VisitObjectProperty( startPropertyIndex, propertyName, propertyIndex );
                _isParsingDependencies = false;
                return result;
            }

            if( _isParsingDependencies )
            {
                _currentDependency = propertyName;
                bool result = base.VisitObjectProperty( startPropertyIndex, propertyName, propertyIndex );
                _currentDependency = null;
                return result;
            }

            return base.VisitObjectProperty( startPropertyIndex, propertyName, propertyIndex );
        }

        public override bool VisitTerminal()
        {
            if( _currentDependency != null )
            {
                StringMatcher m = new StringMatcher( Matcher.Text, Matcher.StartIndex );
                string version;
                m.TryMatchJSONQuotedString( out version );
                _dependencies.Add( _currentDependency, version );
            }
            return base.VisitTerminal();
        }

        public IReadOnlyDictionary<string, string> Dependencies
        {
            get
            {
                if( _dependencies == null )
                {
                    _dependencies = new Dictionary<string, string>();
                    Visit();
                }
                return _dependencies;
            }
        }
    }
}
