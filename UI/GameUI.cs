using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using NNN.Entities.Events;
using NNN.Systems;

namespace NNN.UI;

public class GameUI
{
    private readonly Desktop _desktop;
    private readonly EventBus _eventBus;
    private Label _highScoreLabel;

    public GameUI(EventBus eventBus)
    {
        _eventBus = eventBus;
        _desktop = new Desktop();
        SetupUI();
    }

    private void SetupUI()
    {     
        _eventBus.Subscribe<NewHighestScoreEvent>(OnNewHighScore);
        var spawnKnowledgeButton = Button(10,10, "New Knowledge Orb", new KnowledgeOrbSpawnEvent { Position = new Vector2(100, 100) });
        var spawnAlienButton = Button(60, 10, "New Alien Orb", new AlienOrbSpawnEvent { Position = new Vector2(100, 100) });

        _highScoreLabel = new Label
        {
            Id = "label",
            Text = "No current high score",
            Width = 180,
            Height = 40,
            Left = 10,
            Top = 800
        };

        _desktop.Widgets.Add(spawnKnowledgeButton);
        _desktop.Widgets.Add(spawnAlienButton);
        _desktop.Widgets.Add(_highScoreLabel);
    }

    private void OnNewHighScore(NewHighestScoreEvent highScore)
    {
        _highScoreLabel.Text = $"current high score: {highScore.HighScore.ToString("F2")}";
    }

    private Button Button<T>(int top, int left, string buttonTitle, T eventMessage)
    {
        var button = Myra.Graphics2D.UI.Button.CreateTextButton(buttonTitle);
        
        button.Width = 180;
        button.Height = 40;
        button.Left = left;
        button.Top = top;

        button.Content.HorizontalAlignment = HorizontalAlignment.Center;
        button.Content.VerticalAlignment = VerticalAlignment.Center;

        button.Click += (s, a) =>
        {
            _eventBus.Publish(eventMessage);
        };
        return button;
    }

    public void Draw()
    {
        _desktop.Render();
    }
}