using System;
using System.Diagnostics;
using Invenietis.DependencySolver.Abstractions;
using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver
{
    public static class SolverHelper
    {
        public static ISolver CreateNew( Func<string, IGitRepository> repoFactory, string url )
        {
            SolverProxy solverProxy = new SolverProxy();
            CsProjSolver csProjSolver = new CsProjSolver();
            XProjSolver xProjSolver = new XProjSolver();
            ProjectSolverChainItem solver1 = new ProjectSolverChainItem( ( _, p ) => p.ToLower().EndsWith( ".csproj" ), csProjSolver, new LastProjectSolver() );
            ProjectSolverChainItem solver2 = new ProjectSolverChainItem( ( _, p ) => p.ToLower().EndsWith( ".xproj" ), xProjSolver, solver1 );
            SolutionSolver solutionSolver = new SolutionSolver( solver2 );
            RepoVersionSolver repoVersionSolver = new RepoVersionSolver( solutionSolver );
            Solver result = new Solver( repoVersionSolver, repoFactory, url );
            solverProxy.Solver = result;

            return result;
        }

        class SolverProxy : ISolver
        {
            public IGitRepository Solve()
            {
                Debug.Assert( Solver != null );
                return Solver.Solve();
            }

            internal ISolver Solver { get; set; }
        }

        class ProjectSolverChainItem : IProjectSolver
        {
            readonly Func<IProject, string, bool> _handlePredicate;
            readonly IProjectSolver _solver;
            readonly IProjectSolver _next;

            internal ProjectSolverChainItem( Func<IProject, string, bool> handlePredicate, IProjectSolver solver, IProjectSolver next )
            {
                Debug.Assert( handlePredicate != null );
                Debug.Assert( solver != null );
                Debug.Assert( next != null );

                _handlePredicate = handlePredicate;
                _solver = solver;
                _next = next;
            }

            public void Solve( IProject project, string projectPath )
            {
                if( _handlePredicate( project, projectPath ) ) _solver.Solve( project, projectPath );
                else _next.Solve( project, projectPath );
            }
        }

        class LastProjectSolver : IProjectSolver
        {
            public void Solve( IProject project, string projectPath )
            {
                throw new InvalidOperationException( SolverResources.UnknownProjectType );
            }
        }
    }
}
