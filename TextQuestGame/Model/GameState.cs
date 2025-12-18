using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextQuestGame.Model
{
    [Serializable]
    public class GameState : IGameState
    {
        public string CurrentSceneId { get; set; } = "start";
        public List<string> Inventory { get; } = new List<string>();
    }
}
