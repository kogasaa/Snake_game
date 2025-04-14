using SnakeGame;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace SnakeModel
{

    public class Wall
    {
        
        [XmlElement("ID")]
        [JsonPropertyName("wall")]
        public int Id { get; set; }

        [XmlElement("p1")]
        [JsonPropertyName("p1")]
        public Vector2D StartPosition { get; set; } = new Vector2D();

        [XmlElement("p2")]
        [JsonPropertyName("p2")]
        public Vector2D EndPosition { get; set; } = new Vector2D();
    }
}