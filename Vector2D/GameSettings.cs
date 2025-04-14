using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SnakeModel
{
    /// <summary>
    /// Class responsible for storing the settings of a snake game.
    /// </summary>
    [XmlRoot("GameSettings")]
    public class GameSettings
    {
        [XmlElement("MSPerFrame")]
        public int MSPerFrame { get; set; } = 30;

        [XmlElement("RespawnRate")]
        public int RespawnRate { get; set; } = 100;

        [XmlElement("UniverseSize")]
        public int UniverseSize { get; set; } = 2000;

        [XmlArray("Walls")]
        public List<Wall> Walls { get; set; } = new List<Wall>();

        [XmlElement("SnakeSpeed")]
        public float SnakeSpeed { get; set; } = 5;

        [XmlElement("SnakeStartLength")]
        public float SnakeStartLength { get; set; } = 120;

        [XmlElement("MaxPowerUps")]
        public int MaxPowerUps { get; set; } = 20;

        [XmlElement("MaxPowerUpDelay")]
        public int MaxPowerUpDelay { get; set; } = 75;

        [XmlElement("SnakeGrowth")]
        public int SnakeGrowth { get; set; } = 24;

        [XmlElement("PowerUpsDroppedPerSnakeLength")]
        public float PowerUpsDroppedPerSnakeLength { get; set; } = 50;


    }
}
