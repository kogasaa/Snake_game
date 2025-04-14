using NetworkUtil;
using SnakeModel;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace SnakeClientController
{
    public class SnakeClientController
    {
        public World? world = null;
        private SocketState? connectionState = null;
        public int? playerID = null;
        public event Action? onWorldUpdated;
        public event Action? onErrorOccurred;
        

        public void ConnectToServer(string serverAddress, string playerName)
        {
            Networking.ConnectToServer(OnConnected, serverAddress, 11000);
            void OnConnected(SocketState state)
            {
                if (state.ErrorOccurred)
                {
                    ErrorOccurred();
                    return;
                }


                connectionState = state;
                state.OnNetworkAction = OnDataReceived;
                Networking.GetData(connectionState);
                Networking.Send(connectionState.TheSocket, playerName);
            }
            
        }

        private void ErrorOccurred()
        {
            Disconnect();
            onErrorOccurred?.Invoke();
        }

        private void OnDataReceived(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                ErrorOccurred();
                return;
            }

            // Parse data
            string[] data = state.GetData().Split("\n");
            int numParsedCharacters = 0;
            for (int i = 0; i < data.Length - 1; i++)
            {
                numParsedCharacters += data[i].Length + 1;

                // Initialize ID
                if (playerID == null)
                {
                    playerID = int.Parse(data[i]);
                    continue;
                }

                // Initialize World Size
                if (world == null)
                {
                    world = new World(int.Parse(data[i]));
                    continue;
                }


                lock (world)
                {
                    // Walls
                    if (data[i].StartsWith("{\"wall"))
                    {
                        Wall wall = JsonSerializer.Deserialize<Wall>(data[i])!;
                        world.walls[wall.Id] = wall;
                    }

                    // Powerups
                    if (data[i].StartsWith("{\"power"))
                    {
                        PowerUp powerUp = JsonSerializer.Deserialize<PowerUp>(data[i])!;
                        if (powerUp.Died)
                        {
                            world.powerUps.Remove(powerUp.Id);
                        }
                        else
                        {
                            world.powerUps[powerUp.Id] = powerUp;
                        }
                    }

                    // Snakes
                    if (data[i].StartsWith("{\"snake"))
                    {
                        Snake snake = JsonSerializer.Deserialize<Snake>(data[i])!;
                        if (snake.Disconnected)
                        {
                            world.snakes.Remove(snake.Id);
                        }
                        else
                        {
                            world.snakes[snake.Id] = snake;
                        }
                    }
                }
            }
            state.RemoveData(0, numParsedCharacters);
            
            onWorldUpdated?.Invoke();
            Networking.GetData(state);
        }

        public void SendSnakeMoveInput(SnakeDirection direction)
        {
            if(connectionState == null)
            {
                ErrorOccurred();
                return;
            }
            Networking.Send(connectionState.TheSocket, $"{{\"moving\":\"{direction}\"}}\n");
        }

        public void Disconnect()
        {
            if (world != null)
            {
                lock (world)
                {
                    playerID = null;
                    world = null;

                }
            }
            connectionState?.TheSocket.Close();
            connectionState = null;
        }
    }
}