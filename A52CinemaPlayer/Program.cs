using LibVLCSharp.Shared;

namespace A52CinemaPlayer;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            Core.Initialize();
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Impossible d'initialiser le moteur VLC.\nInstalle VLC 64 bits, puis relance le lecteur.\n\n" + ex.Message,
                "A52 Cinema Player", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
