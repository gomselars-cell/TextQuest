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

            var form = new MainForm();
            var game = new GameService();
            var presenter = new GamePresenter(form, game);

            Application.Run(form);
        }
    }
}