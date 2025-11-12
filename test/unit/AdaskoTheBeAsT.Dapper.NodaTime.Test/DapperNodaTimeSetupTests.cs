using System;
using AwesomeAssertions;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Test
{
    public class DapperNodaTimeSetupTests
    {
        [Fact]
        public void Register_Should_Throw_Exception_When_Null_Provider_Passed()
        {
            // Arrange
            Action action = () => DapperNodaTimeSetup.Register(null!);

            // Act and Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Register_Should_Not_Throw_Exception_When_Proper_Provider_Passed()
        {
            // Arrange
            Action action = () => DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);

            // Act and Assert
            action.Should().NotThrow<ArgumentNullException>();
        }
    }
}
