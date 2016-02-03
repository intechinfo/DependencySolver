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
            Assert.Throws<ArgumentException>( () => new PackageSegment( null, "end" ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( string.Empty, "end" ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( "  ", "end" ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( "start", null ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( "start", string.Empty ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( "start", " " ) );
            Assert.Throws<ArgumentException>( () => new PackageSegment( "last", "first" ) );
        }
    }
}
