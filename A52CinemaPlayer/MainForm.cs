using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;

namespace A52CinemaPlayer;

public sealed class MainForm : Form
{
    private readonly LibVLC _vlc;
    private readonly MediaPlayer _player;
    private readonly VideoView _video;
    private readonly Label _status;
    private readonly TrackBar _volume;
    private readonly ComboBox _output;
    private readonly OpenFileDialog _open;
    private bool _fullscreen;

    public MainForm()
    {
        Text = "A52 Cinema Player V2";
        MinimumSize = new Size(900, 600);
        Size = new Size(1150, 760);
        BackColor = Color.FromArgb(22, 24, 29);
        ForeColor = Color.White;

        _vlc = new LibVLC("--no-video-title-show");
        _player = new MediaPlayer(_vlc);
        _video = new VideoView { Dock = DockStyle.Fill, MediaPlayer = _player, BackColor = Color.Black };

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        Controls.Add(root);
        root.Controls.Add(_video, 0, 0);

        var controls = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true,
            BackColor = Color.FromArgb(34, 37, 44), Padding = new Padding(8)
        };
        root.Controls.Add(controls, 0, 1);

        AddButton(controls, "Ouvrir", (_, _) => OpenMedia());
        AddButton(controls, "Lecture / Pause", (_, _) =>
        {
            if (_player.IsPlaying) _player.Pause();
            else _player.Play();
        });
        AddButton(controls, "Arrêter", (_, _) => _player.Stop());
        AddButton(controls, "Plein écran", (_, _) => ToggleFullscreen());

        controls.Controls.Add(new Label { Text = "Volume", AutoSize = true, Padding = new Padding(8, 10, 0, 0) });
        _volume = new TrackBar { Minimum = 0, Maximum = 200, Value = 100, Width = 130, TickFrequency = 25 };
        _volume.Scroll += (_, _) => _player.Volume = _volume.Value;
        controls.Controls.Add(_volume);

        controls.Controls.Add(new Label { Text = "Sortie", AutoSize = true, Padding = new Padding(8, 10, 0, 0) });
        _output = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 115 };
        _output.Items.AddRange(new object[] { "Auto", "Stéréo", "5.1", "7.1" });
        _output.SelectedIndex = 0;
        controls.Controls.Add(_output);

        _status = new Label
        {
            Dock = DockStyle.Fill, Text = "Prêt — ouvrez un fichier vidéo.",
            TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(12, 0, 0, 0),
            BackColor = Color.FromArgb(17, 19, 23), ForeColor = Color.Gainsboro
        };
        root.Controls.Add(_status, 0, 2);

        _player.Playing += (_, _) => SetStatus("Lecture en cours");
        _player.Paused += (_, _) => SetStatus("En pause");
        _player.Stopped += (_, _) => SetStatus("Arrêté");
        _player.EncounteredError += (_, _) => SetStatus("Erreur de lecture — vérifiez le fichier et les codecs VLC.");

        _open = new OpenFileDialog
        {
            Filter = "Fichiers vidéo|*.mkv;*.mp4;*.avi;*.mov;*.ts;*.m2ts;*.vob;*.mpg;*.mpeg|Tous les fichiers|*.*"
        };

        var menu = new MenuStrip();
        var audio = new ToolStripMenuItem("Audio");
        audio.DropDownItems.Add("Informations sur les traitements", null, (_, _) =>
            MessageBox.Show(
                "AC-3/E-AC-3 : décodage par le moteur VLC.\n\n" +
                "DRC, Dialnorm, Lt/Rt, Surround EX, Headphone et Bass Management sont prévus dans l'architecture, " +
                "mais ne sont pas encore implémentés comme traitements DSP dans cette version.",
                "État des fonctions", MessageBoxButtons.OK, MessageBoxIcon.Information));
        menu.Items.Add(audio);
        MainMenuStrip = menu;
        Controls.Add(menu);
        menu.BringToFront();
    }

    private static void AddButton(Control parent, string text, EventHandler click)
    {
        var b = new Button { Text = text, AutoSize = true, Height = 34, FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(55, 60, 70), ForeColor = Color.White, Margin = new Padding(4) };
        b.Click += click;
        parent.Controls.Add(b);
    }

    private void OpenMedia()
    {
        if (_open.ShowDialog(this) != DialogResult.OK) return;
        using var media = new Media(_vlc, new Uri(_open.FileName));
        _player.Play(media);
        SetStatus(Path.GetFileName(_open.FileName));
    }

    private void ToggleFullscreen()
    {
        _fullscreen = !_fullscreen;
        if (_fullscreen)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }
        else
        {
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.Sizable;
        }
    }

    private void SetStatus(string value)
    {
        if (InvokeRequired) { BeginInvoke(() => _status.Text = value); return; }
        _status.Text = value;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _player.Dispose();
        _vlc.Dispose();
        base.OnFormClosed(e);
    }
}
