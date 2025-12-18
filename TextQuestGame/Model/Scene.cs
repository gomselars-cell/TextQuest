using System;
using System.Collections.Generic;
using System.Text;

namespace TextQuestGame.Model
{
    [Serializable]
    public class Scene : IScene
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public List<Choice> Choices { get; set; } = new List<Choice>();
        public string ImagePath { get; set; } // Добавлено в день 2
    }
}
