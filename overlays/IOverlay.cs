using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverlayProject.overlays
{
    public interface IOverlay
    {
        void Setup(Graphics gfx);

        void Update();
        void Draw(Graphics gfx);

        void DestroyGraphics(object sender, DestroyGraphicsEventArgs e);
    }
}
