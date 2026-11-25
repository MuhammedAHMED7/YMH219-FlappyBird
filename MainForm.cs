using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace FlappyBird;

public sealed class MainForm : Form
{
    readonly Flight game = new();
    readonly System.Windows.Forms.Timer timer = new() { Interval = 16 };
    readonly Stopwatch clock = Stopwatch.StartNew();
    bool held;
    public MainForm()
    {
        Text = "Fırat Flight  /  Uçuş sahası";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        ClientSize = new(900,510);
        MinimumSize = new(520,350);
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.FromArgb(23,42,62);
        StartPosition = FormStartPosition.CenterScreen;
        timer.Tick += (_,_) => {
            var dt = Math.Min(0.05, clock.Elapsed.TotalSeconds);
            clock.Restart();
            game.Advance((float)dt);
            Invalidate();
        };
        timer.Start();
        KeyDown += (_,e) => {
            if (e.KeyCode == Keys.R) { game.Reset(); Invalidate(); }
            if (e.KeyCode == Keys.Space && !held) { held = true; Action(); }
        };
        KeyUp += (_,e) => { if(e.KeyCode == Keys.Space) held = false; };
        Deactivate += (_,_) => held = false;
        MouseDown += (_,e) => { if(e.Button == MouseButtons.Left) Action(); };
    }

    void Action() { if(game.State == Phase.Finished) game.Reset(); game.Flap(); Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        var scale = Math.Min(ClientSize.Width/900f, ClientSize.Height/510f);
        if(scale <= 0) return;
        g.TranslateTransform((ClientSize.Width-900*scale)/2,(ClientSize.Height-510*scale)/2);
        g.ScaleTransform(scale,scale);
        g.SetClip(new Rectangle(0,0,900,510));
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var sky = new LinearGradientBrush(new Rectangle(0,0,900,510),Color.FromArgb(91,160,183), Color.FromArgb(255,210,159),90);
        g.FillRectangle(sky,0,0,900,510);
        using var sun = new SolidBrush(Color.FromArgb(255,226,165));
        using var far = new SolidBrush(Color.FromArgb(139,171,172));
        using var near = new SolidBrush(Color.FromArgb(83,133,143));
        using var dark = new SolidBrush(Color.FromArgb(30,58,73));
        using var edge = new SolidBrush(Color.FromArgb(67,112,119));
        g.FillEllipse(sun,627,48,88,88);
        g.FillPolygon(far,new Point[]{new(0,334),new(119,231),new(201,280),new(344,174),new(495,299),new(604,226),new(734,332),new(846,242),new(900,283),new(900,475),new(0,475)});
        g.FillPolygon(near,new Point[]{new(0,370),new(144,319),new(263,367),new(452,281),new(590,358),new(756,294),new(900,365),new(900,475),new(0,475)});
        foreach(var gate in game.Gates)
        {
            float top=gate.Center-Flight.HalfGap, bottom=gate.Center+Flight.HalfGap;
            g.FillRectangle(dark,gate.X,0,Flight.GateWidth,top);
            g.FillRectangle(dark,gate.X,bottom,Flight.GateWidth,Flight.Ground-bottom);
            g.FillRectangle(edge,gate.X+9,0,7,top);
            g.FillRectangle(edge,gate.X+9,bottom,7,Flight.Ground-bottom);
            g.FillRectangle(sun,gate.X-3,top-7,Flight.GateWidth+6,7);
            g.FillRectangle(sun,gate.X-3,bottom,Flight.GateWidth+6,7);
        }
        var state=g.Save();
        g.TranslateTransform(Flight.BirdX,game.Height);
        g.RotateTransform(Math.Clamp(game.Velocity/13,-18,32));
        g.FillPolygon(sun,new Point[]{new(-26,-7),new(26,-13),new(4,15),new(-2,4),new(-15,9)});
        g.DrawLine(Pens.Sienna,-2,4,24,-12);
        g.Restore(state);
        g.FillRectangle(dark,0,474,900,36);
        TextAt(g,"FIRAT FLIGHT   /   KANYON ROTASI",18,481,11,Brushes.White);
        TextAt(g,"BOŞLUK   Uçuş     •     R   Yeni oyun",622,481,11,Brushes.White);
        g.FillRectangle(dark,20,17,135,41);
        TextAt(g,$"SKOR    {game.Score:00}",33,25,16,sun);
        if(game.State != Phase.Flying)
        {
            g.FillRectangle(dark,276,157,348,150);
            TextAt(g,game.State == Phase.Ready ? "Kanyon uçuşu" : "Oyun bitti",298,176,32,Brushes.White);
            string line=game.State == Phase.Ready ? "Geçitlerin arasından süzül." : $"Geçilen engel   {game.Score}";
            TextAt(g,line,300,232,14,sun);
            TextAt(g,"Boşluk veya tıklama ile uç",300,266,13,Brushes.White);
        }
    }

    static void TextAt(Graphics g,string text,float x,float y,float size,Brush color)
    {
        using var font = new Font("Segoe UI",size,FontStyle.Bold,GraphicsUnit.Pixel);
        g.DrawString(text,font,color,x,y);
    }
    protected override void Dispose(bool disposing) { if(disposing) timer.Dispose(); base.Dispose(disposing); }
}