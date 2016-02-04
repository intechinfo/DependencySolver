using System;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.Core.Tests
{
    [TestFixture]
    public class PackageSegmentTests
    {
        [Test]
        public void Ctor_WithInvalidInputs_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>( () => new PackageSegment( null, "start" ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( string.Empty, "start" ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( "  ", "start" ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( "NuGet", null ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( "NuGet", string.Empty ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( "NuGet", " " ) );
        }
    }
}
