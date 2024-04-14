using System.Collections.Immutable;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using FizzWare.NBuilder;
using FluentAssertions;

namespace EVotingSystem_SBMM.Tests;

public class PasswordHandle_Tests
{
    private readonly IPasswordHandle _passwordHandle;

    public PasswordHandle_Tests()
    {
        _passwordHandle = new PasswordHandle();
    }
   [Fact]
        public void ValidatePassword_ShouldReturnTrue_WhenPasswordsMatch()
        {
            // Arrange
            string hashedPassword = "hashedPassword";
            string inputPassword = "hashedPassword";

            // Act
            bool result = _passwordHandle.ValidatePassword(hashedPassword, inputPassword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidatePassword_ShouldReturnFalse_WhenPasswordsDoNotMatch()
        {
            // Arrange
            string hashedPassword = "hashedPassword";
            string inputPassword = "incorrectPassword";

            // Act
            bool result = _passwordHandle.ValidatePassword(hashedPassword, inputPassword);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CheckByInput_ShouldReturnTrue_WhenPasswordsMatch()
        {
            // Arrange
            string hashedPassword = "hashedPassword";
            string inputPassword = "hashedPassword";

            // Act
            bool result = _passwordHandle.CheckByInput(hashedPassword, inputPassword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CheckByInput_ShouldReturnFalse_WhenPasswordsDoNotMatch()
        {
            // Arrange
            string hashedPassword = "hashedPassword";
            string inputPassword = "incorrectPassword";

            // Act
            bool result = _passwordHandle.CheckByInput(hashedPassword, inputPassword);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GenerateNewPassword_ShouldReturnNewPassword()
        {
            // Act
            string newPassword = _passwordHandle.GenerateNewPassword();

            // Assert
            newPassword.Should().NotBeNullOrEmpty();
            newPassword.Length.Should().Be(8); // Assuming you expect the password length to be 8 characters
        }

        [Fact]
        public void HashPassword_ShouldReturnHashedPassword()
        {
            // Arrange
            string password = "password";

            // Act
            string hashedPassword = _passwordHandle.HashPassword(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password); // Assuming hashing produces a different result than the original password
        }
    
    
    
    
    
    
    
    
    
    
    
    
}