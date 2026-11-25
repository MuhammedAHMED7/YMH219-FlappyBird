namespace FlappyBird;

public sealed class MainForm : Form
{
    public MainForm()
    {
        Text = "FlappyBird";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
    }
}
