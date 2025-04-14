using SnakeClientController;
using SnakeModel;

namespace SnakeGame;

public partial class MainPage : ContentPage
{
    private SnakeClientController.SnakeClientController controller = new SnakeClientController.SnakeClientController();

    public MainPage()
    {
        controller.onErrorOccurred += ErrorOccurred;
        controller.onWorldUpdated += OnFrame;
        InitializeComponent();
        graphicsView.Invalidate();
        worldPanel.controller = controller;

    }

    void OnTapped(object sender, EventArgs args)
    {
        keyboardHack.Focus();
    }

    void ErrorOccurred()
    {
        Dispatcher.Dispatch(delegate
        {
            connectButton.IsEnabled = true;
            aboutButton.IsEnabled = true;
            helpButton.IsEnabled = true;
            DisplayAlert("Error", "Connection error occurred, You have been disconnected", "OK");
        });
    }

    void OnTextChanged(object sender, TextChangedEventArgs args)
    {
        Entry entry = (Entry)sender;
        String text = entry.Text.ToLower();
        if (text == "w")
        {
            controller.SendSnakeMoveInput(SnakeModel.SnakeDirection.up);
        }
        else if (text == "a")
        {
            controller.SendSnakeMoveInput(SnakeModel.SnakeDirection.left);
        }
        else if (text == "s")
        {
            controller.SendSnakeMoveInput(SnakeModel.SnakeDirection.down);
        }
        else if (text == "d")
        {
            controller.SendSnakeMoveInput(SnakeModel.SnakeDirection.right);
        }
        entry.Text = "";
    }


    /// <summary>
    /// Event handler for the connect button
    /// We will put the connection attempt interface here in the view.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ConnectClick(object sender, EventArgs args)
    {
        if (serverText.Text == "")
        {
            DisplayAlert("Error", "Please enter a server address", "OK");
            return;
        }
        if (nameText.Text == "")
        {
            DisplayAlert("Error", "Please enter a name", "OK");
            return;
        }
        if (nameText.Text.Length > 16)
        {
            DisplayAlert("Error", "Name must be less than 16 characters", "OK");
            return;
        }
        controller.ConnectToServer(serverText.Text, nameText.Text);
        connectButton.IsEnabled = false;
        aboutButton.IsEnabled = false;
        helpButton.IsEnabled = false;
        keyboardHack.Focus();
    }

    /// <summary>
    /// Use this method as an event handler for when the controller has updated the world
    /// </summary>
    public void OnFrame()
    {
        Dispatcher.Dispatch(() =>
        {
            graphicsView.Invalidate();

            // Update leaderboard
            try { leaderBoardText.Text = "";        
                lock (controller.world)
                {
                    IEnumerable<Snake> sortedSnakes = controller.world.snakes.Values.OrderByDescending(snake => snake.Score);

                    foreach (Snake snake in sortedSnakes)
                    {

                        leaderBoardText.Text += $"{snake.Name}  |  {snake.Score}\n";
                    }
                }
            }
            catch { }
        });
    }

    private void ControlsButton_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Controls",
                     "W:\t\t Move up\n" +
                     "A:\t\t Move left\n" +
                     "S:\t\t Move down\n" +
                     "D:\t\t Move right\n",
                     "OK");
    }

    private void AboutButton_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("About",
      "SnakeGame solution\nArtwork by Jolie Uk and Alex Smith\nGame design by Daniel Kopta and Travis Martin\n" +
      "Implementation by Liam Healey and Bingkun Han\n" +
        "CS 3500 Fall 2022, University of Utah", "OK");
    }

    private void ContentPage_Focused(object sender, FocusEventArgs e)
    {
        if (!connectButton.IsEnabled)
            keyboardHack.Focus();
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (worldPanel != null)
            worldPanel.zoom = (float)e.NewValue;
    }
}