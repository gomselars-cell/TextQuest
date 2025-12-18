using TextQuestGame.Model;
using TextQuestGame.Presenter;

namespace TextQuestGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // »спользуем ApplicationController дл€ управлени€ жизненным циклом
            using (var appController = new ApplicationController())
            {
                appController.Start();
            }
        }
    }
}