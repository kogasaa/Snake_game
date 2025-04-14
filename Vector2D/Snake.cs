using SnakeGame;
using System.Text.Json.Serialization;

namespace SnakeModel
{
    public class Snake
    {
        [JsonConstructor]
        public Snake() { }

        public Snake(int iD, string name, List<Vector2D> snakeBody)
        {
            Id = iD;
            Name = name;
            Body = snakeBody;
            Join = true;
            Disconnected = false;
            IsAlive = true;
            Direction = snakeBody.Last() - snakeBody.First();
            Direction.Normalize();
            
        }

        [JsonPropertyName("snake")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public List<Vector2D> Body { get; set; } = new List<Vector2D>();

        [JsonPropertyName("dir")]
        public Vector2D Direction { get; set; } = new Vector2D();

        [JsonPropertyName("alive")]
        public bool IsAlive { get; set; } = false;

        [JsonPropertyName("died")]
        public bool Died { get; set; } = false;

        [JsonPropertyName("dc")]
        public bool Disconnected { get; set; }

        [JsonPropertyName("join")]
        public bool Join { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonIgnore]
        public int RespawnFrames { get; set; }

        [JsonIgnore]
        public int GrowthFrames{ get; set; }
    }
}