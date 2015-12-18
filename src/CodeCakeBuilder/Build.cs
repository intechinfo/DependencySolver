using Cake.Common.IO;
using Cake.Core;
using Cake.Common.Diagnostics;
using SimpleGitVersion;
using Code.Cake;
using System;
using System.Linq;
using Cake.Core.Diagnostics;
using System.Collections.Generic;

namespace CodeCake
{
    /// <summary>
    /// Sample build "script".
    /// It can be decorated with AddPath attributes that inject paths into the PATH environment variable. 
    /// </summary>
    [AddPath( "CodeCakeBuilder/Tools" )]
    public class Build : CodeCakeHost
    {
        public Build()
        {
            DNXSolution dnxSolution = null;
            IEnumerable<DNXProjectFile> projectsToPublish = null;
            SimpleRepositoryInfo gitInfo = null;
            string configuration = null;

            Setup( () =>
            {
                dnxSolution = Cake.GetDNXSolution( p => p.ProjectName != "CodeCakeBuilder" );
                if( !dnxSolution.IsValid ) throw new Exception( "Unable to initialize solution." );
                projectsToPublish = dnxSolution.Projects.Where( p => !p.ProjectName.EndsWith( ".Tests" ) );
            } );

            Teardown( () =>
            {
                dnxSolution.RestoreProjectFiles();
            } );

            Task( "Check-Repository" )
                .Does( () =>
                {
                    gitInfo = dnxSolution.RepositoryInfo;
                    if( !gitInfo.IsValid )
                    {
                        if( Cake.IsInteractiveMode()
                            && Cake.ReadInteractiveOption( "Repository is not ready to be published. Proceed anyway?", 'Y', 'N' ) == 'Y' )
                        {
                            Cake.Warning( "GitInfo is not valid, but you choose to continue..." );
                        }
                        else throw new Exception( "Repository is not ready to be published." );
                    }
                    configuration = gitInfo.IsValidRelease && gitInfo.PreReleaseName.Length == 0 ? "Release" : "Debug";
                    Cake.Information( "Publishing {0} projects with version={1} and configuration={2}: {3}",
                        projectsToPublish.Count(),
                        gitInfo.SemVer,
                        configuration,
                        string.Join( ", ", projectsToPublish.Select( p => p.ProjectName ) ) );
                } );

            Task( "Set-ProjectVersion" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                {
                    if( dnxSolution.UpdateProjectFiles( true ) > 0 )
                    {
                        Cake.DNURestore( c =>
                        {
                            c.Quiet = true;
                            c.ProjectPaths.UnionWith( dnxSolution.Projects.Select( p => p.ProjectFilePath ) );
                        } );
                    }
                } );

            Task( "Clean" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                {
                    Cake.CleanDirectories( "**/bin/" + configuration, d => !d.Path.Segments.Contains( "CodeCakeBuilder" ) );
                    Cake.CleanDirectories( "**/obj/" + configuration, d => !d.Path.Segments.Contains( "CodeCakeBuilder" ) );
                    Cake.DeleteFiles( "**/TestResult.xml" );
                } );

            Task( "Build-And-Pack" )
                .IsDependentOn( "Clean" )
                .IsDependentOn( "Set-ProjectVersion" )
                .Does( () =>
                {
                    Cake.DNUBuild( c =>
                    {
                        c.GeneratePackage = true;
                        c.Configurations.Add( configuration );
                        c.ProjectPaths.UnionWith( projectsToPublish.Select( p => p.ProjectDir ) );
                        c.Quiet = true;
                    } );
                } );

            Task( "Unit-Testing" )
                .IsDependentOn( "Build-And-Pack" )
                .Does( () =>
                {
                    var testProjects = dnxSolution.Projects.Where( p => !p.ProjectName.EndsWith( "Abstractions.Tests" ) && p.ProjectName.EndsWith( ".Tests" ) );
                    foreach( var p in testProjects )
                    {
                        foreach( var framework in p.Frameworks )
                        {
                            Cake.DNXRun( c => {
                                c.Arguments = "test";
                                c.Configuration = configuration;
                                c.Framework = framework;
                                c.Project = p.ProjectFilePath;
                            } );
                        }
                    }
                } );

            // The Default task for this script can be set here.
            Task( "Default" ).IsDependentOn( "Unit-Testing" );
        }
    }
}