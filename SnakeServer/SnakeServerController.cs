using NetworkUtil;
using SnakeGame;
using SnakeModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

/// <summary>
/// A class responsible for simulating a snake game, handling client connections, & dispatching updates to clients.
/// Game settings controlled by the settings.xml file found in the appropriate bin folder.
/// </summary>
public class SnakeServerController
{
    // The sockets of each client & their associated snake.
    private Dictionary<SocketState, Snake> clientsToSnakes = new Dictionary<SocketState, Snake>();
    // The world this is simulating.
    private World world;
    // The settings of this game.
    private GameSettings settings;


    // The last time this server was updated.
    private DateTime lastUpdateTime;
    // The number of frames util the next powerup spawns.
    private int framesUntilPowerUp;
    // The id of the next powerup.
    private int nextPowerUpId = 0;


    // Half the width of a snake
    public const int snakeExtents = 5;
    // Half the width of a wall
    public const int wallExtents = 25;
    // The radius of a powerup.
    public const int powerUpRadius = 8;

    /// <summary>
    /// Creates a new snake server with the given settings.
    /// </summary>
    /// <param name="settings"></param>
    public SnakeServerController(GameSettings settings)
    {
        world = new World(settings.UniverseSize);
        foreach (Wall wall in settings.Walls)
        {
            world.walls.Add(wall.Id, wall);
        }
        this.settings = settings;

    }

    /// <summary>
    /// Starts a new sever after reading the settings from settings.xml
    /// </summary>
    /// <param name="arg"></param>
    public static void Main(string[] arg)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));
        SnakeServerController serverController;

        using (Stream stream = File.Open("settings.xml", FileMode.OpenOrCreate))
        {
            try
            {
                serverController = new SnakeServerController((GameSettings)serializer.Deserialize(stream)!);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Warning: Settings file empty or improperly formatted, using default settings.");
                serverController = new SnakeServerController(new GameSettings());
            }
        }

        Networking.StartServer(serverController.OnClientConnected, 11000);
        Console.WriteLine("Server Started");
        serverController.Update();
    }

    private void Update()
    {
        while (true)
        {
            // Wait for next frame.
            while ((DateTime.Now - lastUpdateTime).TotalMilliseconds < settings.MSPerFrame) { }
            lastUpdateTime = DateTime.Now;

            lock (world)
            {

                #region Power Ups
                // Spawn powerups            
                if (world.powerUps.Count < settings.MaxPowerUps && --framesUntilPowerUp <= 0)
                {
                    Vector2D respawnLocation = new Vector2D(0, 0);
                    do
                    {
                        respawnLocation = GetRandomPosition();
                    } while (IsPowerUpCollidingWithWall(respawnLocation));
                    world.powerUps.Add(nextPowerUpId, new PowerUp(nextPowerUpId, respawnLocation));
                    nextPowerUpId++;
                    framesUntilPowerUp = new Random().Next(settings.MaxPowerUpDelay);
                }

                // Kill powerups
                foreach (int ID in new List<int>(world.powerUps.Keys))
                {
                    if (world.powerUps[ID].Died)
                    {
                        world.powerUps.Remove(ID);
                    }
                }
                #endregion


                foreach (Snake snake in world.snakes.Values)
                {
                    if (snake.Disconnected)
                    {
                        continue;
                    }
                    // Respawning
                    snake.Died = false;
                    if (!snake.IsAlive)
                    {
                        snake.RespawnFrames--;
                        if (snake.RespawnFrames < 0)
                        {
                            snake.IsAlive = true;
                            snake.Body = GetRandomSnakeSpawnLocations();
                            snake.Direction = snake.Body[1] - snake.Body[0];
                            snake.Direction.Normalize();
                        }
                        else
                        {
                            continue;
                        }
                    }


                    #region Head Movement

                    Vector2D snakeHeadPosition = snake.Body[snake.Body.Count - 1];
                    Vector2D snakeHeadDir = (snakeHeadPosition - snake.Body[snake.Body.Count - 2]);
                    snakeHeadDir.Normalize();


                    // Handle turning.
                    if (!snakeHeadDir.Equals(snake.Direction))
                    {
                        snake.Body.Add(snakeHeadPosition);
                    }


                    // Wrap snake around the map.
                    float remainingSpeed = settings.SnakeSpeed;
                    Vector2D HeadPosition = snakeHeadPosition + snake.Direction * remainingSpeed;
                    if (HeadPosition.X > settings.UniverseSize / 2)
                    {
                        Vector2D newHeadPosition = new Vector2D(settings.UniverseSize / 2, HeadPosition.Y);
                        remainingSpeed -= (float)(HeadPosition - newHeadPosition).Length();
                        snake.Body[snake.Body.Count - 1] = newHeadPosition;
                        snake.Body.Add(new Vector2D(-settings.UniverseSize / 2, HeadPosition.Y));
                        snake.Body.Add(new Vector2D(-settings.UniverseSize / 2, HeadPosition.Y));
                    }
                    else if (HeadPosition.Y > settings.UniverseSize / 2)
                    {
                        Vector2D newHeadPosition = new Vector2D(HeadPosition.X, settings.UniverseSize / 2);
                        remainingSpeed -= (float)(HeadPosition - newHeadPosition).Length();
                        snake.Body[snake.Body.Count - 1] = newHeadPosition;
                        snake.Body.Add(new Vector2D(HeadPosition.X, -settings.UniverseSize / 2));
                        snake.Body.Add(new Vector2D(HeadPosition.X, -settings.UniverseSize / 2));
                    }
                    else if (HeadPosition.X < -settings.UniverseSize / 2)
                    {
                        Vector2D newHeadPosition = new Vector2D(-settings.UniverseSize / 2, HeadPosition.Y);
                        remainingSpeed -= (float)(HeadPosition - newHeadPosition).Length();
                        snake.Body[snake.Body.Count - 1] = newHeadPosition;
                        snake.Body.Add(new Vector2D(settings.UniverseSize / 2, HeadPosition.Y));
                        snake.Body.Add(new Vector2D(settings.UniverseSize / 2, HeadPosition.Y));
                    }
                    else if (HeadPosition.Y < -settings.UniverseSize / 2)
                    {
                        Vector2D newHeadPosition = new Vector2D(HeadPosition.X, -settings.UniverseSize / 2);
                        remainingSpeed -= (float)(HeadPosition - newHeadPosition).Length();
                        snake.Body[snake.Body.Count - 1] = newHeadPosition;
                        snake.Body.Add(new Vector2D(HeadPosition.X, settings.UniverseSize / 2));
                        snake.Body.Add(new Vector2D(HeadPosition.X, settings.UniverseSize / 2));
                    }


                    // Update head position
                    snake.Body[snake.Body.Count - 1] = snake.Body[snake.Body.Count - 1] + snake.Direction * remainingSpeed;

                    #endregion


                    #region Tail Movement

                    if (snake.GrowthFrames == 0)
                    {
                        float tailSpeed = settings.SnakeSpeed;
                        // Handle vertex deletion
                        float TailLength() => (float)(snake.Body[1] - snake.Body[0]).Length();
                        while (TailLength() <= tailSpeed)
                        {
                            tailSpeed -= TailLength();
                            snake.Body.RemoveAt(0);

                            // Handle map wrapping segments.
                            if (TailLength() >= settings.UniverseSize)
                            {
                                snake.Body.RemoveAt(0);
                            }
                        }

                        Vector2D snakeTailDir = (snake.Body[1] - snake.Body[0]);
                        snakeTailDir.Normalize();

                        // Move tail
                        snake.Body[0] = snake.Body[0] + snakeTailDir * tailSpeed;
                    }
                    else if (snake.GrowthFrames > 0)
                    {
                        // Grow snake
                        snake.GrowthFrames--;
                    }

                    #endregion


                    #region Collision
                    // Check if head collides with walls or other snakes
                    if (IsSnakeSegmentColliding(snake.Id, snake.Body[snake.Body.Count - 2], snake.Body[snake.Body.Count - 1]))
                    {
                        KillSnake(snake);
                        continue;
                    }


                    // Snake self collision 
                    GetBoxMinMax(out Vector2D snakeHeadMin, out Vector2D snakeHeadMax,
                                     snake.Body[snake.Body.Count - 1], snake.Body[snake.Body.Count - 2], snakeExtents);
                    bool farEnough = false;
                    for (int i = snake.Body.Count - 2; i >= 0; i--)
                    {
                        GetBoxMinMax(out Vector2D snakeBodyMin, out Vector2D snakeBodyMax,
                                     snake.Body[i], snake.Body[i + 1], snakeExtents);

                        // Ignore map wrapping segments
                        if ((snake.Body[i] - snake.Body[i + 1]).Length() >= settings.UniverseSize)
                        {
                            farEnough = true;
                            continue;
                        }

                        // Ignore segments until they stop colliding with the head.
                        if (!farEnough)
                        {
                            if (!AreBoxesColliding(snakeBodyMin, snakeBodyMax, snakeHeadMin, snakeHeadMax))
                            {
                                farEnough = true;
                            }
                            continue;
                        }

                        // Detect collisions
                        if (AreBoxesColliding(snakeBodyMin, snakeBodyMax, snakeHeadMin, snakeHeadMax))
                        {
                            KillSnake(snake);
                            break;
                        }
                    }

                    if (!snake.IsAlive)
                        continue;


                    // Power up collision
                    foreach (PowerUp powerUp in world.powerUps.Values)
                    {
                        if ((powerUp.Location - snake.Body[snake.Body.Count - 1]).Length() <= powerUpRadius + snakeExtents)
                        {
                            powerUp.Died = true;
                            snake.GrowthFrames += settings.SnakeGrowth;
                            snake.Score++;
                        }
                    }
                    #endregion
                }


                // Broadcast new world state.

                string serializedWorld = "";
                foreach (Snake snake in world.snakes.Values)
                {
                    serializedWorld += JsonSerializer.Serialize(snake) + "\n";
                    snake.Join = false;
                }

                foreach (PowerUp powerUp in world.powerUps.Values)
                {
                    serializedWorld += JsonSerializer.Serialize(powerUp) + "\n";
                }

                foreach (SocketState socket in clientsToSnakes.Keys)
                {
                    Networking.Send(socket.TheSocket, serializedWorld);
                }

            }
        }
    }




    #region Update Helpers
    void KillSnake(Snake snake)
    {
        snake.Died = true;
        snake.IsAlive = false;
        snake.RespawnFrames = settings.RespawnRate;
        snake.Score = 0;
        snake.GrowthFrames = 0;

        // Spawn powerups along snake length.
        float remainingDistance = 0;
        for (int i = 0; i < snake.Body.Count - 1; i++)
        {
            Vector2D direction = snake.Body[i] - snake.Body[i + 1];
            direction.Normalize();
            remainingDistance += (float)(snake.Body[i] - snake.Body[i + 1]).Length();
            int j = 0;
            while (remainingDistance > settings.PowerUpsDroppedPerSnakeLength)
            {
                remainingDistance -= settings.PowerUpsDroppedPerSnakeLength;

                world.powerUps.Add(nextPowerUpId, new PowerUp(nextPowerUpId, snake.Body[i + 1] + direction * settings.PowerUpsDroppedPerSnakeLength * j));
                j++;
                nextPowerUpId++;
            }
        }
    }

    #region Collision Helpers
    private bool IsPowerUpCollidingWithWall(Vector2D respawnLocation)
    {
        foreach (Wall wall in world.walls.Values)
        {
            GetBoxMinMax(out Vector2D wallMin, out Vector2D wallMax, wall.StartPosition, wall.EndPosition, wallExtents + powerUpRadius);
            if (respawnLocation.X > wallMin.X && respawnLocation.Y > wallMin.Y &&
                respawnLocation.X < wallMax.X && respawnLocation.Y < wallMax.Y)
            {
                return true;
            }
        }
        return false;
    }
    private bool IsSnakeSegmentColliding(int id, Vector2D start, Vector2D end)
    {
        GetBoxMinMax(out Vector2D snakeMin, out Vector2D snakeMax, start, end, snakeExtents);
        foreach (Wall wall in world.walls.Values)
        {
            GetBoxMinMax(out Vector2D wallMin, out Vector2D wallMax, wall.StartPosition, wall.EndPosition, wallExtents);
            if (AreBoxesColliding(snakeMin, snakeMax, wallMin, wallMax))
            {
                return true;
            }
        }
        foreach (Snake snake in world.snakes.Values)
        {
            if (snake.Id == id || !snake.IsAlive)
            {
                continue;
            }
            for (int i = 0; i < snake.Body.Count - 1; i++)
            {
                if ((snake.Body[i] - snake.Body[i + 1]).Length() >= settings.UniverseSize)
                {
                    continue;
                }
                GetBoxMinMax(out Vector2D secondSnakeMin, out Vector2D secondSnakeMax, snake.Body[i], snake.Body[i + 1], snakeExtents);
                if (AreBoxesColliding(snakeMin, snakeMax, secondSnakeMin, secondSnakeMax))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private static bool AreBoxesColliding(Vector2D min1, Vector2D max1, Vector2D min2, Vector2D max2)
    {
        return min2.X < max1.X && min2.Y < max1.Y && max2.X > min1.X && max2.Y > min1.Y;
    }
    private void GetBoxMinMax(out Vector2D min, out Vector2D max, Vector2D start, Vector2D end, float extents)
    {
        min = new Vector2D(Math.Min(start.X, end.X) - extents, Math.Min(start.Y, end.Y) - extents);
        max = new Vector2D(Math.Max(start.X, end.X) + extents, Math.Max(start.Y, end.Y) + extents);
    }
    #endregion

    #region Random Helpers
    private List<Vector2D> GetRandomSnakeSpawnLocations()
    {
        List<Vector2D> snakeBody = new List<Vector2D>() { new Vector2D(), new Vector2D() };
        do
        {
            snakeBody[1] = GetRandomPosition();
            snakeBody[0] = snakeBody[1] + GetRandomDirection() * settings.SnakeStartLength;
        } while (IsSnakeSegmentColliding(-1, snakeBody[0], snakeBody[1]));
        return snakeBody;
    }
    private Vector2D GetRandomPosition()
    {
        Random random = new Random();
        return new Vector2D((random.NextDouble() - 0.5) * world.size, (random.NextDouble() - 0.5) * world.size);
    }
    private Vector2D GetRandomDirection()
    {
        switch (new Random().Next(4))
        {
            case 0:
                return new Vector2D(1, 0);
            case 1:
                return new Vector2D(0, 1);
            case 2:
                return new Vector2D(-1, 0);
            case 3:
                return new Vector2D(0, -1);
            default:
                throw new Exception();

        }
    }
    #endregion

    #endregion






    #region Networking
    private void OnClientConnected(SocketState state)
    {
        if (state.ErrorOccurred)
        {
            return;
        }


        Console.WriteLine("new client connecting...");
        state.OnNetworkAction = OnHandShakeReceived;
        Networking.GetData(state);
    }

    private void OnHandShakeReceived(SocketState state)
    {
        if (state.ErrorOccurred)
        {
            return;
        }


        // Get unique ID
        int id = 0;
        while (world.snakes.ContainsKey(id))
            id++;

        // Create new snake:
        Snake snake = new Snake(id, state.GetData(), GetRandomSnakeSpawnLocations());

        lock (world)
        {
            world.snakes.Add(id, snake);
            clientsToSnakes.Add(state, snake);
        }

        // Networking
        state.RemoveData(0, state.GetData().Length);
        state.OnNetworkAction = OnInputReceived;
        Networking.Send(state.TheSocket, $"{id}\n{world.size}\n");
        foreach (Wall wall in world.walls.Values)
        {
            Networking.Send(state.TheSocket, JsonSerializer.Serialize(wall) + "\n");
        }
        Networking.GetData(state);

        Console.WriteLine($"Client {id} connected");
    }

    private void OnInputReceived(SocketState state)
    {
        // Disconnect clients
        if (state.ErrorOccurred)
        {
            Snake snake = clientsToSnakes[state];
            snake.Disconnected = true;

            // Remove snake & inform other clients.
            lock (world)
            {
                clientsToSnakes.Remove(state);
                foreach (SocketState client in clientsToSnakes.Keys)
                {
                    Networking.Send(client.TheSocket, JsonSerializer.Serialize(snake) + "\n");
                }
                world.snakes.Remove(snake.Id);
            }


            Console.WriteLine($"Client {snake.Id} disconnected");
            return;
        }


        // Parse Input
        string[] input = state.GetData().Split("\n");
        if (input.Length <= 1)
        {
            return;
        }
        switch (input[input.Length - 2])
        {
            case "{\"moving\":\"left\"}":
                SetDirection(new Vector2D(-1, 0));
                break;
            case "{\"moving\":\"right\"}":
                SetDirection(new Vector2D(1, 0));
                break;
            case "{\"moving\":\"up\"}":
                SetDirection(new Vector2D(0, -1));
                break;
            case "{\"moving\":\"down\"}":
                SetDirection(new Vector2D(0, 1));
                break;
            case "{\"moving\":\"none\"}":
                state.RemoveData(0, state.GetData().Length);
                break;
            default:
                break;
        }
        Networking.GetData(state);

        void SetDirection(Vector2D direction)
        {
            // Is valid direction.
            if (clientsToSnakes[state].Direction.IsOppositeCardinalDirection(direction))
                return;
            clientsToSnakes[state].Direction = direction;
            state.RemoveData(0, state.GetData().Length);
        }
    }
    #endregion
}