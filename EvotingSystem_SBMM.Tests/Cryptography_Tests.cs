using EVotingSystem_SBMM.Helper;
using FluentAssertions;

namespace EVotingSystem_SBMM.Tests;

public class Cryptography_Tests
{
    [Fact]
    public void GenerateHash_ShouldGenerateHashedString()
    {
        // Arrange
        string value = "password";

        // Act
        string hashedValue = value.GenerateHash();

        // Assert
        hashedValue.Should().NotBeNullOrEmpty();
        hashedValue.Should().NotBe(value); // Hashed value should not match the original value
    }

    [Fact]
    public void HashVoterId_ShouldReturnHashedInteger()
    {
        // Arrange
        int voterId = 123456;

        // Act
        int hashedId = Cryptography.HashVoterId(voterId);

        // Assert
        hashedId.Should().NotBe(0); // Hashed id should not be default integer value
    }
}