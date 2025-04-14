using SnakeGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeModel
{
    public class World
    {
        public readonly Dictionary<int, Wall> walls = new Dictionary<int, Wall>();
        public readonly Dictionary<int, Snake> snakes = new Dictionary<int, Snake>();
        public readonly Dictionary<int, PowerUp> powerUps = new Dictionary<int, PowerUp>();
        public readonly int size;

        
        public World(int size) 
        { 
            this.size = size;
        }

        
    }
}
