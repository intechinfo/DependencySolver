using System;
using System.IO;
using Invenietis.DependencySolver.Abstractions;
using Invenietis.DependencySolver.Core.Abstractions;
using Invenietis.DependencySolver.Util;
using LibGit2Sharp;
using SimpleGitVersion;

namespace Invenietis.DependencySolver
{
    public sealed class Solver : ISolver
    {
        readonly IRepoVersionSolver _repoVersionSolver;
        readonly Func<string, IGitRepository> _repoFactory;
        readonly string _url;
        string _workingDirectoryPath;

        public Solver(
            IRepoVersionSolver repoVersionSolver,
            Func<string, IGitRepository> repoFactory,
            string url )
        {
            _repoVersionSolver = repoVersionSolver;
            _repoFactory = repoFactory;
            _url = url;
        }

        public IGitRepository Solve()
        {
            string path = Path.Combine( Path.GetTempPath(), Path.GetRandomFileName() );
            IGitRepository result = _repoFactory( path );
            try
            {
                Repository.Clone( _url, path );
                _workingDirectoryPath = path.NormalizeDirectory();
                using( Repository repository = new Repository( path ) )
                {
                    Branch tmpBranch = repository.CreateBranch( $"tmp-{Guid.NewGuid()}" );
                    repository.Checkout( tmpBranch );
                    foreach( Tag tag in repository.Tags )
                    {
                        ReleaseTagVersion version;
                        if( ReleaseTagVersion.TryParse( tag.Name, out version ) )
                        {
                            Commit c = repository.Lookup<Commit>( tag.Target.Id );
                            repository.Checkout( c );
                            IGitRepositoryVersion repoVersion = result.CreateVersion( version );
                            _repoVersionSolver.Solve( repoVersion, _workingDirectoryPath );
                        }
                    }
                }
                return result;
            }
            finally
            {
                if( Directory.Exists( path ) ) FileUtil.DeleteDirectory( path );
            }
        }
    }
}