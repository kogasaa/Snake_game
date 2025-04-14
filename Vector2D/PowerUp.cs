using SnakeGame;
using System.Text.Json.Serialization;

namespace SnakeModel
{
    public class PowerUp
    {
        [JsonConstructor]
        public PowerUp() { }


        public PowerUp(int id, Vector2D location)
        {
            Id = id;
            Location = location;
        }

        [JsonPropertyName("power")]
        public int Id { get; set; }

        [JsonPropertyName("loc")]
        public Vector2D Location { get; set; } = new Vector2D();

        [JsonPropertyName("died")]
        public bool Died { get; set; }
    }
}