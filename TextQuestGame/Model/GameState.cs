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
        public List<string> Inventory { get; set; } = new List<string>();
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();

        public void Clear()
        {
            // Сбрасываем текущую сцену
            CurrentSceneId = "start";

            // Очищаем инвентарь
            if (Inventory != null)
            {
                Inventory.Clear();
            }
            else
            {
                Inventory = new List<string>();
            }

            // Очищаем переменные
            if (Variables != null)
            {
                Variables.Clear();
            }
            else
            {
                Variables = new Dictionary<string, object>();
            }
        }
    }
}
