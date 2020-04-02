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
    public class OverlayOnMoveMouse : IOverlay
    {

        public float MouseDistanceToActivate;
        public DateTime TimeForReactivate;
        public DateTime TimePerActivation;

        public Stream AudioStream;
        public SoundPlayer Player;

        public System.Drawing.Bitmap SourceBitmap;
        public System.Drawing.Imaging.ImageFormat SourceBitmapFormat = System.Drawing.Imaging.ImageFormat.Png;
        public Image OverlayImage;

        public int OverlayImageWidth;
        public int OverlayImageHeight;
        Vector2 displayPos;

        public OverlayOnMoveMouse(
            System.Drawing.Bitmap imageSource = null,
            Stream audioSource = null)
        {
            AudioStream = audioSource == null ? Resources.what : audioSource;
            SourceBitmap = imageSource == null ? Resources.face : imageSource;
        }

        public void Setup(Graphics gfx)
        {
            Player = new SoundPlayer(AudioStream);
            MemoryStream stream = new MemoryStream();
            SourceBitmap.Save(stream, SourceBitmapFormat);

            OverlayImageHeight = SourceBitmap.Height;
            OverlayImageWidth = SourceBitmap.Width;
            OverlayImage = new Image(gfx, stream.ToArray());

            //player = new SoundPlayer("C:\\source\\c#\\overlay\\overlay1\\what.wav");


            lastPos.X = Cursor.Position.X;
            lastPos.Y = Cursor.Position.Y;



            displayPos.X = Screen.PrimaryScreen.Bounds.Width / 2f;
            displayPos.Y = Screen.PrimaryScreen.Bounds.Height / 2f;
        }


        public void Draw(Graphics gfx)
        {
            if (isTriggered)
            { 

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
            TimeToStopTrigger = DateTime.Now.AddMilliseconds(TimePerActivation.Millisecond);
            isTriggered = true;
        }
        public void StopTrigger()
        {
            Player.Stop();
            TimeToNextTrigger = DateTime.Now.AddMilliseconds(TimeForReactivate.Millisecond);
            isTriggered = false;
        }

        public void DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            Player.Dispose();
            //AudioStream.Dispose();
            OverlayImage.Dispose();
        }
    }
}
