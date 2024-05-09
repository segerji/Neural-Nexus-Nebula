using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using NNN.Entities.Events;
using NNN.Systems;

namespace NNN.UI;

public class GameUI
{
    private readonly Desktop _desktop;
    private readonly EventBus _eventBus;

    public GameUI(EventBus eventBus)
    {
        _eventBus = eventBus;
        _desktop = new Desktop();
        SetupUI();
    }

    private void SetupUI()
    {
        var button = Button.CreateTextButton("click me");

        button.Width = 100;
        button.Height = 40;
        button.Left = 10;
        button.Top = 10;

        button.Content.HorizontalAlignment = HorizontalAlignment.Center;
        button.Content.VerticalAlignment = VerticalAlignment.Center;

        button.Click += (s, a) =>
        {
            _eventBus.Publish(new KnowledgeOrbSpawnEvent { Position = new Vector2(100, 100) });
        };

        _desktop.Widgets.Add(button);
    }

    public void Draw()
    {
        _desktop.Render();
    }
}