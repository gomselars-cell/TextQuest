using System;
using System.Collections.Generic;
using System.Text;
using TextQuestGame.Model;

namespace TextQuestGame
{
    public interface IScene
    {
        string Id { get; }
        string Text { get; }
        List<Choice> Choices { get; }
    }

    public interface IGameState
    {
        string CurrentSceneId { get; set; }
        List<string> Inventory { get; }
    }

    public interface IGameService
    {
        void SaveGame(string path);
        void LoadGame(string path);
        IScene GetCurrentScene();
        void MakeChoice(int choiceIndex);
    }
}
