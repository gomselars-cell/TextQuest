using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TextQuestGame.Model;

namespace TextQuestGame
{
    public class GameService : IGameService
    {
        private const string DefaultImagePath = "Images/default.jpg";
        private const string DefaultScenesFile = "scenes.json";

        private GameState _state;
        private Dictionary<string, Scene> _scenes;
        private readonly string _scenesFilePath;

        // Конструкторы
        public GameService() : this(DefaultScenesFile) { }

        public GameService(string scenesFilePath)
        {
            _scenesFilePath = scenesFilePath;
            Reset();
        }

        #region Инициализация и сброс игры

        public void Reset()
        {
            // Всегда создаём новое состояние игры
            _state = new GameState
            {
                CurrentSceneId = "start",
                Inventory = new List<string>(),
                Variables = new Dictionary<string, object>()
            };

            // Устанавливаем начальные переменные
            _state.Variables["health"] = 100;
            _state.Variables["money"] = 0;

            // Загружаем сцены
            LoadScenes();
        }

        private void LoadScenes()
        {
            try
            {
                _scenes = SceneLoader.LoadScenes(_scenesFilePath);

                // Проверяем, что начальная сцена существует
                if (!_scenes.ContainsKey(_state.CurrentSceneId))
                {
                    Console.WriteLine($"Предупреждение: Начальная сцена '{_state.CurrentSceneId}' не найдена. Используется первая сцена.");
                    _state.CurrentSceneId = _scenes.Keys.First();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки сцен: {ex.Message}");
                _scenes = SceneLoader.GetDefaultScenes();
            }
        }

        private void InitializeDefaultVariables()
        {
            // Можно установить начальные переменные игры
            // Например: _state.Variables["health"] = 100;
            // Или: _state.Variables["difficulty"] = "normal";

            // Установим здоровье по умолчанию
            if (!_state.Variables.ContainsKey("health"))
            {
                _state.Variables["health"] = 100;
            }

            // Установим деньги по умолчанию
            if (!_state.Variables.ContainsKey("money"))
            {
                _state.Variables["money"] = 0;
            }
        }

        #endregion

        #region Основные методы игры

        public IScene GetCurrentScene()
        {
            if (_scenes.TryGetValue(_state.CurrentSceneId, out var scene))
            {
                return scene;
            }

            Console.WriteLine($"Ошибка: Сцена '{_state.CurrentSceneId}' не найдена!");
            return new Scene
            {
                Id = "error",
                Text = "Ошибка: сцена не найдена!",
                ImagePath = DefaultImagePath,
                Choices = new List<Choice>()
            };
        }

        public void MakeChoice(int choiceIndex)
        {
            var scene = GetCurrentScene() as Scene;

            // Проверяем корректность индекса
            if (scene == null || choiceIndex < 0 || choiceIndex >= scene.Choices.Count)
            {
                Console.WriteLine($"Неверный выбор: {choiceIndex}");
                return;
            }

            var choice = scene.Choices[choiceIndex];

            // Проверяем условие (если есть)
            if (!CheckChoiceCondition(choice))
            {
                Console.WriteLine($"Условие для выбора '{choice.Text}' не выполнено");
                return;
            }

            // Применяем эффект (если есть)
            if (!string.IsNullOrEmpty(choice.Effect))
            {
                ApplyEffect(choice.Effect);
            }

            // Меняем сцену
            if (!string.IsNullOrEmpty(choice.NextSceneId))
            {
                _state.CurrentSceneId = choice.NextSceneId;
            }
            else
            {
                Console.WriteLine($"Ошибка: у выбора '{choice.Text}' не указана следующая сцена");
            }
        }

        #endregion

        #region Инвентарь

        public void AddItem(string item)
        {
            if (string.IsNullOrEmpty(item))
                return;

            if (!_state.Inventory.Contains(item))
            {
                _state.Inventory.Add(item);
                Console.WriteLine($"Добавлен предмет: {item}");
            }
        }

        public void RemoveItem(string item)
        {
            if (_state.Inventory.Contains(item))
            {
                _state.Inventory.Remove(item);
                Console.WriteLine($"Удалён предмет: {item}");
            }
        }

        public bool HasItem(string item)
        {
            return _state.Inventory.Contains(item);
        }

        public List<string> GetInventory()
        {
            return new List<string>(_state.Inventory); // Возвращаем копию
        }

        #endregion

        #region Переменные игры

        public void SetVariable(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                return;

            _state.Variables[name] = value;
            Console.WriteLine($"Установлена переменная: {name} = {value}");
        }

        public object GetVariable(string name)
        {
            if (_state.Variables.TryGetValue(name, out var value))
            {
                return value;
            }
            return null;
        }

        public T GetVariable<T>(string name, T defaultValue = default)
        {
            var value = GetVariable(name);
            if (value is T typedValue)
            {
                return typedValue;
            }

            // Попытка конвертации
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public bool HasVariable(string name)
        {
            return _state.Variables.ContainsKey(name);
        }

        #endregion

        #region Условия и эффекты

        private bool CheckChoiceCondition(Choice choice)
        {
            if (string.IsNullOrEmpty(choice.Condition))
                return true; // Условия нет → разрешено

            // Разбираем условие (простой формат: "Тип:Параметр1:Параметр2")
            var parts = choice.Condition.Split(':');
            if (parts.Length == 0)
                return true;

            var conditionType = parts[0];

            switch (conditionType.ToLower())
            {
                case "hasitem":
                    if (parts.Length > 1)
                        return HasItem(parts[1]);
                    break;

                case "notHasItem":
                    if (parts.Length > 1)
                        return !HasItem(parts[1]);
                    break;

                case "variable":
                    if (parts.Length > 2)
                    {
                        var value = GetVariable(parts[1]);
                        return value?.ToString() == parts[2];
                    }
                    break;

                case "variableGreater":
                    if (parts.Length > 2)
                    {
                        var variable = GetVariable(parts[1]);
                        if (int.TryParse(variable?.ToString(), out int varValue) &&
                            int.TryParse(parts[2], out int targetValue))
                        {
                            return varValue > targetValue;
                        }
                    }
                    break;

                case "variableLess":
                    if (parts.Length > 2)
                    {
                        var variable = GetVariable(parts[1]);
                        if (int.TryParse(variable?.ToString(), out int varValue) &&
                            int.TryParse(parts[2], out int targetValue))
                        {
                            return varValue < targetValue;
                        }
                    }
                    break;
            }

            return false;
        }

        private void ApplyEffect(string effect)
        {
            if (string.IsNullOrEmpty(effect))
                return;

            var parts = effect.Split(':');
            if (parts.Length == 0)
                return;

            var effectType = parts[0];

            switch (effectType.ToLower())
            {
                case "additem":
                    if (parts.Length > 1)
                        AddItem(parts[1]);
                    break;

                case "removeitem":
                    if (parts.Length > 1)
                        RemoveItem(parts[1]);
                    break;

                case "setvariable":
                    if (parts.Length > 2)
                        SetVariable(parts[1], parts[2]);
                    break;

                case "incrementvariable":
                    if (parts.Length > 1)
                    {
                        var currentValue = GetVariable<int>(parts[1], 0);
                        SetVariable(parts[1], currentValue + 1);
                    }
                    break;

                case "decrementvariable":
                    if (parts.Length > 1)
                    {
                        var currentValue = GetVariable<int>(parts[1], 0);
                        SetVariable(parts[1], currentValue - 1);
                    }
                    break;

                case "combineitems":
                    if (parts.Length > 3)
                    {
                        var item1 = parts[1];
                        var item2 = parts[2];
                        var resultItem = parts[3];

                        if (HasItem(item1) && HasItem(item2))
                        {
                            RemoveItem(item1);
                            RemoveItem(item2);
                            AddItem(resultItem);
                        }
                    }
                    break;

                default:
                    Console.WriteLine($"Неизвестный эффект: {effect}");
                    break;
            }
        }

        #endregion

        #region Работа с картинками

        public string GetCurrentSceneImagePath()
        {
            var scene = GetCurrentScene() as Scene;
            return GetFullImagePath(scene?.ImagePath);
        }

        public string GetFullImagePath(string relativePath)
        {
            // Если путь не указан или пустой, используем картинку по умолчанию
            if (string.IsNullOrEmpty(relativePath))
                return DefaultImagePath;

            // Прямой путь
            if (File.Exists(relativePath))
                return relativePath;

            // Пробуем в папке Images
            var pathInImages = Path.Combine("Images", relativePath);
            if (File.Exists(pathInImages))
                return pathInImages;

            // Или берём только имя файла и ищем в Images
            var fileName = Path.GetFileName(relativePath);
            if (!string.IsNullOrEmpty(fileName))
            {
                pathInImages = Path.Combine("Images", fileName);
                if (File.Exists(pathInImages))
                    return pathInImages;
            }

            // Если ничего не нашли, используем картинку по умолчанию
            Console.WriteLine($"Картинка не найдена: {relativePath}");
            return DefaultImagePath;
        }

        #endregion

        #region Сохранение и загрузка

        public void SaveGame(string path)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    // Для корректной сериализации Dictionary<string, object>
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                var json = JsonSerializer.Serialize(_state, options);
                File.WriteAllText(path, json);

                Console.WriteLine($"Игра сохранена в: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения игры: {ex.Message}");
                throw new Exception($"Не удалось сохранить игру: {ex.Message}", ex);
            }
        }

        public void LoadGame(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Файл сохранения не найден: {path}");
                }

                var json = File.ReadAllText(path);
                var options = new JsonSerializerOptions
                {
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                _state = JsonSerializer.Deserialize<GameState>(json, options);

                // Восстанавливаем связи, если нужно
                if (string.IsNullOrEmpty(_state.CurrentSceneId))
                {
                    _state.CurrentSceneId = "start";
                }

                Console.WriteLine($"Игра загружена из: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки игры: {ex.Message}");
                throw new Exception($"Не удалось загрузить игру: {ex.Message}", ex);
            }
        }

        #endregion

        #region Вспомогательные методы

        public List<string> GetAvailableChoices()
        {
            var scene = GetCurrentScene() as Scene;
            var availableChoices = new List<string>();

            foreach (var choice in scene.Choices)
            {
                if (CheckChoiceCondition(choice))
                {
                    availableChoices.Add(choice.Text);
                }
            }

            return availableChoices;
        }

        public string GetGameInfo()
        {
            var inventoryText = _state.Inventory.Any()
                ? $"Инвентарь: {string.Join(", ", _state.Inventory)}"
                : "Инвентарь пуст";

            var variablesText = _state.Variables.Any()
                ? $"Переменные: {string.Join(", ", _state.Variables.Select(v => $"{v.Key}: {v.Value}"))}"
                : "";

            return $"{inventoryText}\n{variablesText}".Trim();
        }

        #endregion
    }
}
