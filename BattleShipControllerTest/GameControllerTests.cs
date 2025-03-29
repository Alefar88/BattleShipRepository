using Battleship.Controllers;
using BattleShip.Objects;
using Microsoft.AspNetCore.Mvc;

namespace BattleShipControllerTest
{
    public class GameControllerTests
    {
        [Fact]
        public void InitGame_ReturnsOkResult()
        {
            // Arrange
            var controller = new GameController();

            // Act
            var result = controller.InitGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }


        [Fact]
        public void FireMissile_Hit_ReturnsHitMessage()
        {
            // Arrange
            var controller = new GameController();
            controller.InitGame();
            var request = new FireRequest { X = 'A', Y = 1 };
            var goodResponse = new List<string> { "Hit!", "Missed!", "Sank" };
            // Act
            var result = controller.FireMissile(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var message = (result as OkObjectResult).Value.ToString();
            Assert.False(message.Contains("Error!"));
            Assert.True(goodResponse.Any(c => message.Contains(c)));
        }
    }
}