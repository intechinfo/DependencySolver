using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Invenietis.DependencySolver.Abstractions;
using Invenietis.DependencySolver.Core;
using Invenietis.DependencySolver.Core.Abstractions;
using Invenietis.DependencySolver.Util;

namespace Invenietis.DependencySolver
{
    public sealed class SolutionSolver : ISolutionSolver
    {
        public static readonly Regex ProjectRegex = new Regex( @"^Project\(""{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}}""\) = ""(?<projectname>[^""]+)"", ""(?<projectpath>[^""]+.(?<projecttype>(x|cs)proj))"", ""{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}}""\r?$", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase );

        readonly IProjectSolver _projectSolver;

        public SolutionSolver( IProjectSolver projectSolver )
        {
            _projectSolver = projectSolver;
        }

        public void Solve( ISolution solution, string workingDirectoryPath, string slnPath )
        {
            string solutionFileContent = File.ReadAllText( slnPath, Encoding.UTF8 );
            MatchCollection matches = ProjectRegex.Matches( solutionFileContent );
            foreach( Match match in ProjectRegex.Matches( solutionFileContent ) )
            {
                string projectPath = match.Groups[ "projectpath" ].Value;
                projectPath = Path.Combine( Path.GetDirectoryName( slnPath ), projectPath );
                string relativeProjectPath = FileUtil.RelativePath( workingDirectoryPath, projectPath );
                string projectType = match.Groups[ "projecttype" ].Value;
                IProject project;
                if( solution.AddOrCreateProject( relativeProjectPath, out project ) )
                {
                    _projectSolver.Solve( project, projectPath );
                }
            }
        }
    }
}