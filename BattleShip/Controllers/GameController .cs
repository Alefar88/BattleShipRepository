using BattleShip.Objects;

using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace Battleship.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private static readonly int GRID_SIZE = 10;
        private static readonly Dictionary<string, int> Ships = new Dictionary<string, int>
        {
            { "Aircraft Carrier", 5 },
            { "Battleship", 4 },
            { "Submarine", 3 },
            { "Destroyer", 3 },
            { "Patrol Boat", 2 }
        };

        private static List<List<char>> board;
        private static Dictionary<string, List<(int, int)>> shipsPositions = new Dictionary<string, List<(int, int)>>();

        [HttpPost("init")]
        public IActionResult InitGame()
        {
            try
            {
                board = new List<List<char>>();
                shipsPositions = new Dictionary<string, List<(int, int)>>();

                for (int i = 0; i < GRID_SIZE; i++)
                {
                    var row = new List<char>();
                    for (int j = 0; j < GRID_SIZE; j++)
                    {
                        row.Add('~');
                    }
                    board.Add(row);
                }

                PlaceShips();

                return Ok(new { message = "Board Initialized!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("fire")]
        public IActionResult FireMissile([FromBody] FireRequest request)
        {
            try
            {
                int x = Char.ToUpper(request.X) - 'A' ;
                int y = request.Y - 1;

                if (board[x][y] == '~')
                {
                    return Ok(new { message = "Missed!" });
                }
                else
                {
                    char shipInitial = board[x][y];
                    board[x][y] = 'X';
                    foreach (var ship in shipsPositions)
                    {
                        if (ship.Value.Contains((x, y)))
                        {
                            ship.Value.Remove((x, y));
                            if (!ship.Value.Any())
                            {
                                return Ok(new { message = $"Sank {ship.Key}!" });
                            }
                            return Ok(new { message = "Hit!" });
                        }
                    }
                }

                return Ok(new { message = "Error!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        private void PlaceShips()
        {
            Random random = new Random();
            foreach (var ship in Ships)
            {
                bool placed = false;
                while (!placed)
                {
                    bool horizontal = random.Next(2) == 0;
                    int row = random.Next(GRID_SIZE);
                    int col = random.Next(GRID_SIZE);

                    if (horizontal && col + ship.Value <= GRID_SIZE && CanPlaceShip(row, col, ship.Value, horizontal))
                    {
                        for (int i = 0; i < ship.Value; i++)
                        {
                            board[row][col + i] = ship.Key[0];
                        }
                        shipsPositions[ship.Key] = new List<(int, int)>();
                        for (int i = 0; i < ship.Value; i++)
                        {
                            shipsPositions[ship.Key].Add((row, col + i));
                        }
                        placed = true;
                    }
                    else if (!horizontal && row + ship.Value <= GRID_SIZE && CanPlaceShip(row, col, ship.Value, horizontal))
                    {
                        for (int i = 0; i < ship.Value; i++)
                        {
                            board[row + i][col] = ship.Key[0];
                        }
                        shipsPositions[ship.Key] = new List<(int, int)>();
                        for (int i = 0; i < ship.Value; i++)
                        {
                            shipsPositions[ship.Key].Add((row + i, col));
                        }
                        placed = true;
                    }
                }
            }
        }

        private bool CanPlaceShip(int row, int col, int size, bool horizontal)
        {
            for (int i = 0; i < size; i++)
            {
                if (horizontal)
                {
                    if (board[row][col + i] != '~') return false;
                }
                else
                {
                    if (board[row + i][col] != '~') return false;
                }
            }
            return true;
        }
    }
}