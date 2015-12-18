using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Invenietis.DependencySolver.Util;

namespace Invenietis.DependencySolver.Abstractions.Tests
{
    public sealed class ResourceFile : IDisposable
    {
        readonly string _resourceName;
        string _path;

        public ResourceFile( string resourceName )
        {
            _resourceName = resourceName;
        }

        public string Path
        {
            get
            {
                EnsureLoaded();
                return _path;
            }
        }

        void EnsureLoaded()
        {
            if( _path == null )
            {
                _path = System.IO.Path.GetTempFileName();
                File.SetAttributes( _path, FileAttributes.Temporary );
                using( Stream src = typeof( ResourceFile ).GetTypeInfo().Assembly.GetManifestResourceStream( _resourceName ) )
                using( Stream destination = File.OpenWrite( _path ) )
                {
                    src.CopyTo( destination );
                }
            }
        }

        public void Dispose()
        {
            if( _path != null && File.Exists( _path ) ) File.Delete( _path );
        }
    }

    public sealed class ZippedResource : IDisposable
    {
        readonly string _resourceName;
        string _path;

        public ZippedResource( string resourceName )
        {
            _resourceName = resourceName;
        }

        public string Path
        {
            get
            {
                EnsureLoaded();
                return _path;
            }
        }

        void EnsureLoaded()
        {
            if( _path == null )
            {
                _path = System.IO.Path.Combine( System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName() );
                using( ResourceFile zip = new ResourceFile( _resourceName ) )
                {
                    ZipFile.ExtractToDirectory( zip.Path, _path );
                }
            }
        }

        public void Dispose()
        {
            if( _path != null && Directory.Exists( _path ) ) FileUtil.DeleteDirectory( _path );
        }
    }
}
