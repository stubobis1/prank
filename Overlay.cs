using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using OverlayProject.Properties;

namespace OverlayProject
{
    public class Overlay
    {

        private readonly GraphicsWindow _window;
        private readonly Graphics _graphics;

        private readonly Dictionary<string, SolidBrush> _brushes;
        private readonly Dictionary<string, Font> _fonts;
        private readonly Dictionary<string, Image> _images;

        static SoundPlayer player;
        static Image face;

        public Overlay()
        {
            _brushes = new Dictionary<string, SolidBrush>();
            _fonts = new Dictionary<string, Font>();
            _images = new Dictionary<string, Image>();

            _graphics = new Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = false,
                WindowHandle = IntPtr.Zero
            };

            _window = new GraphicsWindow(_graphics)
            {
                IsTopmost = true,
                IsVisible = true,
                FPS = 60,
                X = 0,
                Y = 0,
                Width = Screen.PrimaryScreen.Bounds.Width,
                Height = Screen.PrimaryScreen.Bounds.Height
            };



            _window.SetupGraphics += _window_SetupGraphics;
            _window.DestroyGraphics += _window_DestroyGraphics;
            _window.DrawGraphics += _window_DrawGraphics;
        }

        ~Overlay()
        {
            _window.Dispose();
            _graphics.Dispose();
        }

        public void Run()
        {

            _window.Create();

            _window.Join();
        }

        private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            var gfx = e.Graphics;
            if (e.RecreateResources) return;
            _fonts.Add("arial", gfx.CreateFont("Arial", 14));
            Setup(gfx);
        }

        private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            foreach (var pair in _brushes) pair.Value.Dispose();
            foreach (var pair in _fonts) pair.Value.Dispose();
            foreach (var pair in _images) pair.Value.Dispose();
        }

        private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            Draw(gfx);
            //gfx.DrawText(_fonts["arial"], 22, _brushes["white"], 20, 20, $"FPS: {gfx.FPS}");
            //gfx.DrawRectangle(_brushes["white"], 20, 60, 400, 400, 1.0f);

            //gfx.DrawText(_fonts["arial"], 22, _brushes["white"], 40, 100, $"Count: {e.FrameCount}");
            //gfx.DrawText(_fonts["arial"], 22, _brushes["white"], 40, 130, $"Time: {e.FrameTime}");
            //gfx.DrawText(_fonts["arial"], 22, _brushes["white"], 40, 160, $"Delta: {e.DeltaTime}");
        }



        private const float amontToMoveMouse = 60f;
        private void Setup(Graphics gfx)
        {
            //Screen.PrimaryScreen.Bounds = "";

            _brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
            _brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
            _brushes["background"] = gfx.CreateSolidBrush(0, 0, 0, 0f);

            lastPos.X = Cursor.Position.X;
            lastPos.Y = Cursor.Position.Y;

            // fonts don't need to be recreated since they are owned by the font factory and not the drawing device
            //face = new Image(_graphics, "C:\\source\\c#\\overlay\\overlay1\\face.png");

            MemoryStream stream = new MemoryStream();
            Resources.face.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            face = new Image(_graphics, stream.ToArray());
        
            //player = new SoundPlayer("C:\\source\\c#\\overlay\\overlay1\\what.wav");
            player = new SoundPlayer(Resources.what);


            width = Screen.PrimaryScreen.Bounds.Width;
            height = Screen.PrimaryScreen.Bounds.Height;

            displayPos.X = width / 2f;
            displayPos.Y = height / 2f;
        }
        static int width;
        static int height;

        Vector2 displayPos;


        private void Draw(Graphics gfx)
        {

            ManageAbleToShow();
            gfx.ClearScene(_brushes["background"]);
            if (showFace)
            {
                gfx.DrawImage(face, displayPos.X, displayPos.Y, 1);
            }
        }
    
        Vector2 lastPos;
        static bool showFace;
        private bool ManageAbleToShow()
        {
            bool moved = false;

            var currentPos = new Vector2(Cursor.Position.X, Cursor.Position.Y);

        
            if (Vector2.Distance(currentPos, lastPos) > amontToMoveMouse)
            {
                moved = true;
            }

            lastPos.X = Cursor.Position.X;
            lastPos.Y = Cursor.Position.Y;


            if (moved && canplay)
            {
                Thread spookThread = new Thread(new ThreadStart(PlaySpook));
                spookThread.Start();
            }
            return showFace;
        }

        static int timeInbetween = 5000;
        static bool canplay = true;
        public static void PlaySpook()
        {
        
            canplay = false;

            player.Play(); //Sound
            showFace = true; //Image

            Thread.Sleep(3000);

            showFace = false; //Stop Image

            Thread.Sleep(timeInbetween);
            canplay = true; // Allow next play
        }

    }
}


