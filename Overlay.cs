﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using OverlayProject.overlays;
using OverlayProject.Properties;

namespace OverlayProject
{
    public class Overlay
    {

        public GraphicsWindow Window;
        public Graphics Graphics;

        public Dictionary<string, SolidBrush> Brushes;
        public Dictionary<string, Font> Fonts;
        public Dictionary<string, Image> Images;

        public List<IOverlayItem> Overlays = new List<IOverlayItem>();

        public Overlay()
        {
            Brushes = new Dictionary<string, SolidBrush>();
            Fonts = new Dictionary<string, Font>();
            Images = new Dictionary<string, Image>();

            Graphics = new Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = false,
                WindowHandle = IntPtr.Zero
            };

            Window = new GraphicsWindow(Graphics)
            {
                IsTopmost = true,
                IsVisible = true,
                FPS = 60,
                X = 0,
                Y = 0,
                Width = Screen.PrimaryScreen.Bounds.Width,
                Height = Screen.PrimaryScreen.Bounds.Height
            };



            Window.SetupGraphics += _window_SetupGraphics;
            Window.DestroyGraphics += _window_DestroyGraphics;
            Window.DrawGraphics += _window_DrawGraphics;
        }

        ~Overlay()
        {
            Window.Dispose();
            Graphics.Dispose();
        }

        public void Run()
        {
            Window.Create();
            Window.Join();
        }

        private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            var gfx = e.Graphics;
            Setup(gfx);
        }

        private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            foreach (var pair in Brushes) pair.Value.Dispose();
            foreach (var pair in Fonts) pair.Value.Dispose();
            foreach (var pair in Images) pair.Value.Dispose();
            DestoryGraphics(sender, e);
        }

       
        private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            Draw(gfx);
        }



        private void Setup(Graphics gfx)
        {
            foreach (var o in Overlays)
            {
                o.Setup(this, gfx);
            }
        }

        private void Draw(Graphics gfx)
        {

            Update();
            gfx.ClearScene();
            foreach (var o in Overlays)
            {
                o.Draw(gfx);
            }


        }

        private void Update()
        {
            foreach (var o in Overlays)
            {
                o.Update();
            }
        }

        private void DestoryGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            foreach (var o in Overlays)
            {
                o.DestroyGraphics(sender,e);
            }
        }

    }
}


