using System;
using System.Collections.Generic;
using System.Text;

namespace TextQuestGame.Presenter
{
    public class ApplicationController : IDisposable
    {
        private GamePresenter _presenter;
        private IGameService _gameService;
        private IGameView _view;

        public void Start()
        {
            try
            {
                // Проверяем наличие необходимых файлов
                CheckRequiredFiles();

                // Создаём модель
                _gameService = new GameService();

                // Создаём представление
                _view = new MainForm();

                // Создаём презентер
                _presenter = new GamePresenter(_view, _gameService);

                // Запускаем приложение
                Application.Run(_view as Form);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка запуска: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private void CheckRequiredFiles()
        {
            // Проверяем наличие файла сцен
            if (!File.Exists("scenes.json"))
            {
                // Создаём файл сцен по умолчанию
                CreateDefaultScenesFile();
            }

            // Проверяем наличие папки для картинок
            if (!Directory.Exists("Images"))
            {
                Directory.CreateDirectory("Images");
                MessageBox.Show("Создана папка Images для картинок. Добавьте туда файлы .jpg для сцен.",
                    "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CreateDefaultScenesFile()
        {
            var defaultScenes = @"[
  {
    ""Id"": ""start"",
    ""Text"": ""Вы стоите в тёмной комнате. Перед вами две двери: левая и правая. На полу лежит старый фонарик."",
    ""ImagePath"": ""Images/default.jpg"",
    ""Choices"": [
      {
        ""Text"": ""Взять фонарик и пойти налево"",
        ""NextSceneId"": ""left_room"",
        ""Condition"": """",
        ""Effect"": ""AddItem:Фонарик""
      },
      {
        ""Text"": ""Взять фонарик и пойти направо"",
        ""NextSceneId"": ""right_room"",
        ""Condition"": """",
        ""Effect"": ""AddItem:Фонарик""
      },
      {
        ""Text"": ""Проигнорировать фонарик и осмотреться"",
        ""NextSceneId"": ""look_around"",
        ""Condition"": """",
        ""Effect"": """"
      }
    ]
  },
  {
    ""Id"": ""left_room"",
    ""Text"": ""Вы вошли в библиотеку. На столе лежит древняя книга и блестящий ключ."",
    ""ImagePath"": ""Images/default.jpg"",
    ""Choices"": [
      {
        ""Text"": ""Взять книгу"",
        ""NextSceneId"": ""book_taken"",
        ""Condition"": """",
        ""Effect"": ""AddItem:Древняя книга""
      },
      {
        ""Text"": ""Взять ключ"",
        ""NextSceneId"": ""key_taken"",
        ""Condition"": """",
        ""Effect"": ""AddItem:Ключ""
      },
      {
        ""Text"": ""Вернуться обратно"",
        ""NextSceneId"": ""start"",
        ""Condition"": """",
        ""Effect"": """"
      }
    ]
  },
  {
    ""Id"": ""right_room"",
    ""Text"": ""Комната заперта. На двери висит большой замок."",
    ""ImagePath"": ""Images/default.jpg"",
    ""Choices"": [
      {
        ""Text"": ""Попытаться открыть дверь"",
        ""NextSceneId"": ""door_locked"",
        ""Condition"": """",
        ""Effect"": """"
      },
      {
        ""Text"": ""Использовать ключ"",
        ""NextSceneId"": ""secret_room"",
        ""Condition"": ""HasItem:Ключ"",
        ""Effect"": ""RemoveItem:Ключ""
      },
      {
        ""Text"": ""Вернуться обратно"",
        ""NextSceneId"": ""start"",
        ""Condition"": """",
        ""Effect"": """"
      }
    ]
  }
]";

            File.WriteAllText("scenes.json", defaultScenes);
            Console.WriteLine("Создан файл scenes.json с начальными сценами");
        }

        public void Dispose()
        {
            // Очистка ресурсов, если нужно
            _presenter = null;
            _gameService = null;
            _view = null;
        }
    }
}
