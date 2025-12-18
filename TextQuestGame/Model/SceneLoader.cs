using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TextQuestGame.Model;

namespace TextQuestGame
{
    public static class SceneLoader
    {
        private const string DefaultImage = "Images/default.jpg";

        public static Dictionary<string, Scene> LoadScenes(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Файл сцен не найден: {filePath}. Используются сцены по умолчанию.");
                    return GetDefaultScenes();
                }

                var json = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var scenes = JsonSerializer.Deserialize<List<Scene>>(json, options);

                // Валидация и обработка картинок
                ValidateAndFixScenes(scenes);

                // Создаём словарь для быстрого доступа
                var sceneDict = new Dictionary<string, Scene>();
                foreach (var scene in scenes)
                {
                    if (sceneDict.ContainsKey(scene.Id))
                    {
                        Console.WriteLine($"Предупреждение: Дублирующаяся сцена с ID: {scene.Id}");
                    }
                    sceneDict[scene.Id] = scene;
                }

                Console.WriteLine($"Загружено сцен: {sceneDict.Count}");
                return sceneDict;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки сцен: {ex.Message}");
                return GetDefaultScenes();
            }
        }

        private static void ValidateAndFixScenes(List<Scene> scenes)
        {
            if (scenes == null)
                return;

            foreach (var scene in scenes)
            {
                // Проверяем и исправляем картинку
                scene.ImagePath = ValidateSceneImage(scene.Id, scene.ImagePath);

                // Проверяем, что у всех выборов есть текст
                foreach (var choice in scene.Choices ?? new List<Choice>())
                {
                    if (string.IsNullOrEmpty(choice.Text))
                    {
                        choice.Text = "Продолжить...";
                        Console.WriteLine($"Предупреждение: У сцены '{scene.Id}' есть выбор без текста");
                    }
                }
            }
        }

        private static string ValidateSceneImage(string sceneId, string imagePath)
        {
            // Если путь не указан
            if (string.IsNullOrEmpty(imagePath))
            {
                Console.WriteLine($"Сцена '{sceneId}' без картинки. Используется картинка по умолчанию.");
                return DefaultImage;
            }

            // Проверяем расширение
            var extension = Path.GetExtension(imagePath).ToLower();
            if (extension != ".jpg" && extension != ".jpeg")
            {
                Console.WriteLine($"Сцена '{sceneId}': не-JPG формат '{imagePath}'. Используется картинка по умолчанию.");
                return DefaultImage;
            }

            // Проверяем существование файла
            if (File.Exists(imagePath))
                return imagePath;

            // Пробуем найти в папке Images
            var fileName = Path.GetFileName(imagePath);
            if (!string.IsNullOrEmpty(fileName))
            {
                var pathInImages = Path.Combine("Images", fileName);
                if (File.Exists(pathInImages))
                    return pathInImages;
            }

            Console.WriteLine($"Картинка для сцены '{sceneId}' не найдена: {imagePath}. Используется картинка по умолчанию.");
            return DefaultImage;
        }

        public static Dictionary<string, Scene> GetDefaultScenes()
        {
            var scenes = new Dictionary<string, Scene>();

            // Начальная сцена
            scenes["start"] = new Scene
            {
                Id = "start",
                Text = "Вы стоите в тёмной комнате. Перед вами две двери: левая и правая.",
                ImagePath = DefaultImage,
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Text = "Открыть левую дверь",
                        NextSceneId = "left_room",
                        Condition = "", // Без условий
                        Effect = "" // Без эффектов
                    },
                    new Choice
                    {
                        Text = "Открыть правую дверь",
                        NextSceneId = "right_room",
                        Condition = "",
                        Effect = ""
                    },
                    new Choice
                    {
                        Text = "Осмотреться",
                        NextSceneId = "look_around",
                        Condition = "",
                        Effect = "AddItem:Фонарик"
                    }
                }
            };

            // Другие сцены по умолчанию
            scenes["left_room"] = new Scene
            {
                Id = "left_room",
                Text = "Вы вошли в библиотеку. На столе лежит древняя книга.",
                ImagePath = DefaultImage,
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Text = "Взять книгу",
                        NextSceneId = "book_taken",
                        Condition = "",
                        Effect = "AddItem:Древняя книга"
                    },
                    new Choice
                    {
                        Text = "Вернуться обратно",
                        NextSceneId = "start",
                        Condition = "",
                        Effect = ""
                    }
                }
            };

            scenes["right_room"] = new Scene
            {
                Id = "right_room",
                Text = "Комната заперта. Вам нужен ключ.",
                ImagePath = DefaultImage,
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Text = "Использовать ключ",
                        NextSceneId = "secret_room",
                        Condition = "HasItem:Ключ",
                        Effect = "RemoveItem:Ключ"
                    },
                    new Choice
                    {
                        Text = "Вернуться обратно",
                        NextSceneId = "start",
                        Condition = "",
                        Effect = ""
                    }
                }
            };

            scenes["look_around"] = new Scene
            {
                Id = "look_around",
                Text = "В углу комнаты вы нашли старый фонарик.",
                ImagePath = DefaultImage,
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Text = "Вернуться к дверям",
                        NextSceneId = "start",
                        Condition = "",
                        Effect = ""
                    }
                }
            };

            Console.WriteLine("Используются сцены по умолчанию");
            return scenes;
        }
    }
}
