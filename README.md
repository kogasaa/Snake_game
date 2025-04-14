Made by Liam Healey & Bingkun Han


===============
|   Client    |
===============

====== FEATURES ======
- Leaderboard that shows all snakes' current scores
- Zoom slider found at the top of the window
- Dead snakes become transparent before respawning
- Player tag show Name | ID 
                    Score


====== POTENTIAL ISSUES ======
- One time we added 60 AI snakes to a game and it froze the entire computer, though we think it was an error with the AI client,
  as the AI's console had a bunch of exceptions in it.
- We never tested on Mac or Linux
- About and Help buttons are disabled when game is running






===============
|   Server    |
===============

====== FEATURES ======
- Snakes drop powerups when they die.
- Additional optional settings found in the .xml:
   - Kill Powerup Per Snake Length: the distance between powerups spawned when a snake dies.
   - Snake Speed: how far snakes move every frame
   - Snake Start Length: how long snakes start as
   - Max Power Ups: the max number of powerups that can spawn naturally
   - Max Powerup Delay: the maximum number of frames between natural powerup spawns
   - Snake Growth: how many frames of growth a snake gets when it eats a power up.
- Snakes wrap around the edge of the map.
- Settings located in the bin file of the snake server project.


====== POTENTIAL ISSUES ======
- AI pathing seems to often not work on our server, unsure if it is an issue with the AI client or our server.
    - AI clients sometimes crash
    - AI clients sometimes stop pathing without crashing
- We never tested on Mac or Linux.



====== Example settings ======
Feel free to copy paste these settings into your (as we can not commit files in the git ignore)
<GameSettings>
  <MSPerFrame>17</MSPerFrame>
  <RespawnRate>100</RespawnRate>
  <UniverseSize>2000</UniverseSize>
  <SnakeSpeed>4.0</SnakeSpeed>
  <MaxPowerUps>20</MaxPowerUps>
  <SnakeGrowth>30</SnakeGrowth>

  <Walls>
   
    <Wall>
      <ID>1</ID>
      <p1>
        <x>-975</x>
        <y>-975</y>
      </p1>
      <p2>
        <x>-975</x>
        <y>975</y>
      </p2>
    </Wall>
    <Wall>
      <ID>2</ID>
      <p1>
        <x>975</x>
        <y>975</y>
      </p1>
      <p2>
        <x>975</x>
        <y>-975</y>
      </p2>
    </Wall>
   
    <Wall>
      <ID>4</ID>
      <p1>
        <x>-425</x>
        <y>-425</y>
      </p1>
      <p2>
        <x>-275</x>
        <y>-425</y>
      </p2>
    </Wall>
    <Wall>
      <ID>5</ID>
      <p1>
        <x>-425</x>
        <y>-425</y>
      </p1>
      <p2>
        <x>-425</x>
        <y>-275</y>
      </p2>
    </Wall>
    <Wall>
      <ID>6</ID>
      <p1>
        <x>425</x>
        <y>-425</y>
      </p1>
      <p2>
        <x>275</x>
        <y>-425</y>
      </p2>
    </Wall>
    <Wall>
      <ID>7</ID>
      <p1>
        <x>425</x>
        <y>-425</y>
      </p1>
      <p2>
        <x>425</x>
        <y>-275</y>
      </p2>
    </Wall>
    <Wall>
      <ID>8</ID>
      <p1>
        <x>425</x>
        <y>425</y>
      </p1>
      <p2>
        <x>275</x>
        <y>425</y>
      </p2>
    </Wall>
    <Wall>
      <ID>9</ID>
      <p1>
        <x>425</x>
        <y>425</y>
      </p1>
      <p2>
        <x>425</x>
        <y>275</y>
      </p2>
    </Wall>
    <Wall>
      <ID>10</ID>
      <p1>
        <x>-425</x>
        <y>425</y>
      </p1>
      <p2>
        <x>-275</x>
        <y>425</y>
      </p2>
    </Wall>
    <Wall>
      <ID>11</ID>
      <p1>
        <x>-425</x>
        <y>425</y>
      </p1>
      <p2>
        <x>-425</x>
        <y>275</y>
      </p2>
    </Wall>
    <Wall>
      <ID>12</ID>
      <p1>
        <x>-50</x>
        <y>-275</y>
      </p1>
      <p2>
        <x>50</x>
        <y>-275</y>
      </p2>
    </Wall>
    <Wall>
      <ID>13</ID>
      <p1>
        <x>-50</x>
        <y>275</y>
      </p1>
      <p2>
        <x>50</x>
        <y>275</y>
      </p2>
    </Wall>
    <Wall>
      <ID>14</ID>
      <p1>
        <x>-225</x>
        <y>-50</y>
      </p1>
      <p2>
        <x>-225</x>
        <y>50</y>
      </p2>
    </Wall>
    <Wall>
      <ID>15</ID>
      <p1>
        <x>225</x>
        <y>-50</y>
      </p1>
      <p2>
        <x>225</x>
        <y>50</y>
      </p2>
    </Wall>
    <Wall>
      <ID>16</ID>
      <p1>
        <x>-975</x>
        <y>-200</y>
      </p1>
      <p2>
        <x>-775</x>
        <y>-200</y>
      </p2>
    </Wall>
    <Wall>
      <ID>17</ID>
      <p1>
        <x>-975</x>
        <y>200</y>
      </p1>
      <p2>
        <x>-775</x>
        <y>200</y>
      </p2>
    </Wall>
    <Wall>
      <ID>18</ID>
      <p1>
        <x>-775</x>
        <y>200</y>
      </p1>
      <p2>
        <x>-775</x>
        <y>100</y>
      </p2>
    </Wall>
    <Wall>
      <ID>19</ID>
      <p1>
        <x>-775</x>
        <y>-200</y>
      </p1>
      <p2>
        <x>-775</x>
        <y>-100</y>
      </p2>
    </Wall>
    <Wall>
      <ID>20</ID>
      <p1>
        <x>975</x>
        <y>-200</y>
      </p1>
      <p2>
        <x>775</x>
        <y>-200</y>
      </p2>
    </Wall>
    <Wall>
      <ID>21</ID>
      <p1>
        <x>975</x>
        <y>200</y>
      </p1>
      <p2>
        <x>775</x>
        <y>200</y>
      </p2>
    </Wall>
    <Wall>
      <ID>22</ID>
      <p1>
        <x>775</x>
        <y>200</y>
      </p1>
      <p2>
        <x>775</x>
        <y>100</y>
      </p2>
    </Wall>
    <Wall>
      <ID>23</ID>
      <p1>
        <x>775</x>
        <y>-200</y>
      </p1>
      <p2>
        <x>775</x>
        <y>-100</y>
      </p2>
    </Wall>
  </Walls>
</GameSettings>



