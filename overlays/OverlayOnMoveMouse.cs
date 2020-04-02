using GameOverlay.Drawing;
using GameOverlay.Windows;
using OverlayProject.Properties;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Image = GameOverlay.Drawing.Image;

namespace OverlayProject.overlays
{
    public class OverlayOnMoveMouse : IOverlayItem
    {

        public float MouseDistanceToActivate;
        public int MillisForReactivate;
        public int MillisPerActivation;

        public Stream AudioStream;
        public SoundPlayer Player;

        public System.Drawing.Bitmap SourceBitmap;
        public System.Drawing.Imaging.ImageFormat SourceBitmapFormat = System.Drawing.Imaging.ImageFormat.Png;
        public Image OverlayImage;

        public int OverlayImageWidth;
        public int OverlayImageHeight;
        Point displayPos;

        public OverlayOnMoveMouse(
            System.Drawing.Bitmap imageSource = null,
            Stream audioSource = null,
            float mouseDistanceToActivate = 50f,
            int millisPerActivation = 3000,
            int millisForReactivate = 5000)
        {
            this.MouseDistanceToActivate = mouseDistanceToActivate;
            this.MillisPerActivation = millisPerActivation;
            this.MillisForReactivate = millisForReactivate;
            AudioStream = audioSource ?? Resources.default_wav;
            SourceBitmap = imageSource ?? Resources.default_png;
        }

        public Overlay Parent;
        public void Setup(Overlay parent, Graphics gfx)
        {
            this.Parent = parent;
            Player = new SoundPlayer(AudioStream);

            MemoryStream stream = new MemoryStream();
            SourceBitmap.Save(stream, SourceBitmapFormat);

            OverlayImageHeight = SourceBitmap.Height;
            OverlayImageWidth = SourceBitmap.Width;
            OverlayImage = new Image(gfx, stream.ToArray());

            //player = new SoundPlayer("C:\\source\\c#\\overlay\\overlay1\\what.wav");


            lastPos.X = Cursor.Position.X;
            lastPos.Y = Cursor.Position.Y;



            displayPos.X = (Screen.PrimaryScreen.Bounds.Width / 2f) - (OverlayImageWidth / 2f);
            displayPos.Y = (Screen.PrimaryScreen.Bounds.Height / 2f) - (OverlayImageHeight / 2f);
        }


        public void Draw(Graphics gfx)
        {
            if (isTriggered)
            {
                gfx.DrawImage(OverlayImage, displayPos);
            }
        }

        public void Update()
        {
            ManageTrigger();
        }





        Vector2 lastPos;
        public bool isTriggered;
        public DateTime TimeToNextTrigger;
        public DateTime TimeToStopTrigger;

        private void ManageTrigger()
        {
            bool moved = MouseMoved();
            
            if (!isTriggered && moved && DateTime.Now > TimeToNextTrigger)
            {
                Trigger();
            }
            else if (isTriggered && DateTime.Now > TimeToStopTrigger)
            {
                StopTrigger();
            }
        }

        private bool MouseMoved()
        {
            bool moved = false;
            var currentPos = new Vector2(Cursor.Position.X, Cursor.Position.Y);

            if (Vector2.Distance(currentPos, lastPos) > MouseDistanceToActivate)
            {
                moved = true;
            }

            lastPos.X = Cursor.Position.X;
            lastPos.Y = Cursor.Position.Y;

            return moved;
        }


        public void Trigger()
        {
            Player.Play();
            TimeToStopTrigger = DateTime.Now.AddMilliseconds(MillisPerActivation);
            isTriggered = true;
        }
        public void StopTrigger()
        {
            Player.Stop();
            TimeToNextTrigger = DateTime.Now.AddMilliseconds(MillisForReactivate);
            isTriggered = false;
        }

        public void DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            DisposeResources();
        }

        private void DisposeResources()
        {
            Player.Dispose();
            //AudioStream.Dispose();
            OverlayImage.Dispose();
        }
    }
}
