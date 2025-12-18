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
    }
}
