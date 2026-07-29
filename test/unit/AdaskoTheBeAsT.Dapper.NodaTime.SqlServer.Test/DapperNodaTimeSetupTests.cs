using System;
using AwesomeAssertions;
using Moq;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.Test
{
    public class DapperNodaTimeSetupTests
    {
        [Fact]
        public void Register_Should_Throw_Exception_When_Null_Provider_Passed()
        {
            // Arrange
            Action action = () => DapperNodaTimeSetup.Register(null!, new Mock<INodaTimeTypeHandlerConfiguration>(MockBehavior.Strict).Object);

            // Act and Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Register_Should_Not_Throw_Exception_When_Proper_Provider_Passed()
        {
            // Arrange
            Action action = () => DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb, new Mock<INodaTimeTypeHandlerConfiguration>(MockBehavior.Strict).Object);

            // Act and Assert
            action.Should().NotThrow<ArgumentNullException>();
        }
    }
}
