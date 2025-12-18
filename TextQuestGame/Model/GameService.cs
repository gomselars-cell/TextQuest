using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TextQuestGame.Model
{
    public class GameService : IGameService
    {
        private GameState _state = new GameState();
        private Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

        public GameService()
        {
            // Простейшие сцены для демо
            _scenes["start"] = new Scene
            {
                Id = "start",
                Text = "Вы стоите в темной комнате. Дверь слева и справа.",
                Choices = new List<Choice>
                {
                new Choice { Text = "Пойти налево", NextSceneId = "left" },
                new Choice { Text = "Пойти направо", NextSceneId = "right" }
                }
            };
            _scenes["left"] = new Scene { Id = "left", Text = "Вы нашли сокровище!", Choices = new List<Choice>() };
            _scenes["right"] = new Scene { Id = "right", Text = "Вы упали в яму. Конец игры.", Choices = new List<Choice>() };
        }

        public void SaveGame(string path)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_state, options);
            File.WriteAllText(path, json);
        }

        public void LoadGame(string path)
        {
            var json = File.ReadAllText(path);
            _state = JsonSerializer.Deserialize<GameState>(json);
        }

        public IScene GetCurrentScene()
        {
            return _scenes[_state.CurrentSceneId];
        }

        public void MakeChoice(int choiceIndex)
        {
            Scene scene = GetCurrentScene() as Scene;
            if (choiceIndex < scene.Choices.Count)
            {
                _state.CurrentSceneId = scene.Choices[choiceIndex].NextSceneId;
            }
        }
    }
}
