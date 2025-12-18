using System;
using System.Collections.Generic;
using System.Text;

namespace TextQuestGame.Model
{
    [Serializable]
    public class Choice
    {
        public string Text { get; set; }
        public string NextSceneId { get; set; }
        public string Condition { get; set; } // Добавлено в день 2
        public string Effect { get; set; }    // Добавлено в день 2
    }
}
