using System;
using System.Collections.Generic;
using System.Text;
using TextQuestGame.Model;

namespace TextQuestGame
{
    public interface IScene
    {
        // Основные свойства сцены
        string Id { get; }
        string Text { get; }
        List<Choice> Choices { get; }

        // Картинка сцены (добавлено в день 2)
        string ImagePath { get; }
    }

    public interface IGameState
    {
        // Основные свойства состояния игры
        string CurrentSceneId { get; set; }

        // Инвентарь (предметы)
        List<string> Inventory { get; set; }

        // Переменные игры (здоровье, деньги, флаги и т.д.)
        Dictionary<string, object> Variables { get; set; }

        // Метод для очистки состояния (опционально)
        void Clear();
    }

    public interface IGameService
    {
        // Основные методы игры
        void SaveGame(string path);
        void LoadGame(string path);
        IScene GetCurrentScene();
        void MakeChoice(int choiceIndex);
        void Reset();

        // Методы для работы с инвентарём
        void AddItem(string item);
        void RemoveItem(string item);
        bool HasItem(string item);
        List<string> GetInventory();

        // Методы для работы с переменными
        void SetVariable(string name, object value);
        object GetVariable(string name);
        T GetVariable<T>(string name, T defaultValue = default);
        bool HasVariable(string name);

        // Методы для работы с картинками
        string GetCurrentSceneImagePath();
        string GetFullImagePath(string relativePath);

        // Вспомогательные методы
        List<string> GetAvailableChoices();
        string GetGameInfo();
    }

    public interface IGameView
    {
        // События от пользовательского интерфейса
        event Action<int> ChoiceSelected;
        event Action SaveRequested;
        event Action LoadRequested;
        event Action NewGameRequested;

        // Методы для обновления интерфейса
        void DisplayScene(Scene scene);
        void UpdateGameInfo(string info);
        void ShowMessage(string message);
        void ShowError(string error);
        void UpdateInventory(List<string> inventory);
    }
}
