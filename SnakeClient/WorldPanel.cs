using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using IImage = Microsoft.Maui.Graphics.IImage;
#if MACCATALYST
using Microsoft.Maui.Graphics.Platform;
#else
using Microsoft.Maui.Graphics.Win2D;
#endif
using Color = Microsoft.Maui.Graphics.Color;
using System.Reflection;
using Microsoft.Maui;
using System.Net;
using Font = Microsoft.Maui.Graphics.Font;
using SizeF = Microsoft.Maui.Graphics.SizeF;

using SnakeModel;
using SnakeClientController;
using System;
using Windows.ApplicationModel.Background;

namespace SnakeGame;
public class WorldPanel : IDrawable
{
    private IImage wallImage;
    private IImage backgroundImage;

    private World world => controller.world;
    public SnakeClientController.SnakeClientController controller;
    public float zoom = 1;

    private bool initializedForDrawing = false;



    private IImage loadImage(string name)
    {
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        string path = "SnakeClient.Resources.Images";
        using (Stream stream = assembly.GetManifestResourceStream($"{path}.{name}"))
        {
#if MACCATALYST
            return PlatformImage.FromStream(stream);
#else
            return new W2DImageLoadingService().FromStream(stream);
#endif
        }
    }

    public WorldPanel()
    {
    }

    private void InitializeDrawing()
    {
        wallImage = loadImage( "wallsprite.png" );
        backgroundImage = loadImage( "background.png" );
        initializedForDrawing = true;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (controller.playerID == null)
            return;
        lock (world)
        {
            if (!initializedForDrawing)
                InitializeDrawing();

            // undo previous transformations from last frame
            canvas.ResetState();


            // Get player location
            if (!world.snakes.TryGetValue(controller.playerID ?? -1, out Snake playerSnake) || playerSnake.Body.Count == 0)
                return;
            Vector2D playerLocation = playerSnake.Body.Last();


            // Follow the player & zoom
            canvas.Translate((float)(dirtyRect.Width / 2f - playerLocation.X * zoom), (float)(dirtyRect.Height / 2f - playerLocation.Y * zoom));
            canvas.Scale(zoom, zoom);


            // Background
            canvas.DrawImage(backgroundImage, world.size / -2f, world.size / -2f, world.size, world.size);


            // Powerups
            canvas.FillColor = Colors.Red;
            foreach (PowerUp powerUp in world.powerUps.Values)
            {
                Draw(powerUp.Location, delegate { canvas.FillEllipse(-8, -8, 16f, 16f); });
            }


            // Walls
            foreach (Wall wall in world.walls.Values)
            {
                Vector2D offset = (wall.EndPosition - wall.StartPosition);
                offset.Normalize();
                offset *= 50;
                for (int i = 0; i <= (wall.EndPosition - wall.StartPosition).Length() / 50; i++)
                {
                    Draw(wall.StartPosition + offset * i, delegate { canvas.DrawImage(wallImage, -25, -25, 50, 50); });
                }
            }


            // Snakes
            canvas.StrokeSize = 10;
            canvas.StrokeLineCap = LineCap.Round;
            foreach (Snake snake in world.snakes.Values)
            {
                
                Random rng = new Random(snake.Id);
                canvas.StrokeColor = Color.FromHsva(rng.Next(255), 255, 255, snake.IsAlive ? 255: 10);
                for (int i = 0; i < snake.Body.Count - 1; i++)
                {
                    
                    Vector2D start = snake.Body[i];
                    Vector2D end = snake.Body[i + 1];
                    Vector2D direction = end - start;

                    if (direction.Length() >= world.size)
                    {
                        continue;
                    }
                    Draw(start, delegate { canvas.DrawLine(0, 0, (float)direction.X, (float)direction.Y); });
                }
            }

            // Player Labels
            canvas.FontColor = Colors.White;
            canvas.FontSize = 20;
            foreach (Snake snake in world.snakes.Values)
            {
                Vector2D headPosition = snake.Body[snake.Body.Count - 1];
                if (snake.IsAlive)
                    Draw(headPosition + new Vector2D(0, 20), delegate { canvas.DrawString(snake.Name + " | " + snake.Id + "\n" + snake.Score, 0f, 0f, HorizontalAlignment.Center); });
                
            }
        }
        
        // Utility function for drawing things at locations.
        void Draw(Vector2D location, Action drawFunction)
        { 
            canvas.SaveState();
            canvas.Translate((float)location.X, (float)location.Y);
            drawFunction();
            canvas.RestoreState();
        }
    }


}
