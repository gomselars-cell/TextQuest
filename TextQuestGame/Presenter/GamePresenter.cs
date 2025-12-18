using System;
using System.Collections.Generic;
using System.Text;

namespace TextQuestGame.Presenter
{
    public class GamePresenter
    {
        private readonly IGameService _game;
        private readonly MainForm _view;

        public GamePresenter(MainForm view, IGameService game)
        {
            _view = view;
            _game = game;

            _view.ChoiceSelected += OnChoiceSelected;
            _view.OnSave += () => _game.SaveGame("save.json");
            _view.OnLoad += () => { _game.LoadGame("save.json"); UpdateView(); };
            _view.OnNewGame += () => { /* Reset game */ UpdateView(); };

            UpdateView();
        }

        private void OnChoiceSelected(int index)
        {
            _game.MakeChoice(index);
            UpdateView();
        }

        private void UpdateView()
        {
            var scene = _game.GetCurrentScene();
            _view.DisplayScene(scene.Text,
                scene.Choices.Select(c => c.Text).ToList());
        }
    }
}
