using System;
using System.IO;

namespace Invenietis.DependencySolver.Util
{
    public static class FileUtil
    {
        public static bool DeleteDirectory( string path )
        {
            try
            {
                foreach( string filePath in Directory.EnumerateFiles( path, "*", SearchOption.AllDirectories ) )
                {
                    File.SetAttributes( filePath, RemoveAttributes( filePath, FileAttributes.ReadOnly ) );
                }
                Directory.Delete( path, true );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static FileAttributes AddAttributes( string path, FileAttributes attributes )
        {
            FileAttributes current = File.GetAttributes( path );
            return current | attributes;
        }

        public static FileAttributes RemoveAttributes( string path, FileAttributes attributes )
        {
            FileAttributes current = File.GetAttributes( path );
            return current & ~attributes;
        }

        public static string RelativePath( string root, string path )
        {
            root = root.NormalizeDirectory();
            if( !path.StartsWith( root ) ) throw new ArgumentException( UtilResources.InvalidRoot, nameof( path ) );

            return path.Substring( root.Length );
        }

        public static string NormalizeDirectory( this string @this )
        {
            return @this.EndsWith( @"\" ) ? @this : $@"{@this}\";
        }
    }
}
