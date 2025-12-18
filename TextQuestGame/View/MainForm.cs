using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TextQuestGame
{
    public partial class MainForm : Form
    {
        private FlowLayoutPanel choicePanel;
        private RichTextBox sceneText;
        private MenuStrip menu;

        public MainForm()
        {
            InitializeComponent();
            CreateControls();
        }

        private void CreateControls()
        {
            // Простейший интерфейс
            sceneText = new RichTextBox { Dock = DockStyle.Top, Height = 200 };
            choicePanel = new FlowLayoutPanel { Dock = DockStyle.Fill };

            menu = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Файл");
            fileMenu.DropDownItems.Add("Сохранить", null, (s, e) => OnSave?.Invoke());
            fileMenu.DropDownItems.Add("Загрузить", null, (s, e) => OnLoad?.Invoke());
            fileMenu.DropDownItems.Add("Новая игра", null, (s, e) => OnNewGame?.Invoke());
            menu.Items.Add(fileMenu);

            Controls.AddRange(new Control[] { choicePanel, sceneText, menu });
        }

        public void DisplayScene(string text, List<string> choices)
        {
            sceneText.Text = text;
            choicePanel.Controls.Clear();

            for (int i = 0; i < choices.Count; i++)
            {
                var button = new Button { Text = choices[i], Tag = i, Width = 200 };
                button.Click += (s, e) => ChoiceSelected?.Invoke((int)button.Tag);
                choicePanel.Controls.Add(button);
            }
        }

        // События для подключения к логике
        public event Action<int> ChoiceSelected;
        public event Action OnSave;
        public event Action OnLoad;
        public event Action OnNewGame;
    }
}
