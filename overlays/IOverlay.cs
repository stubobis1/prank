using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverlayProject.overlays
{
    public interface IOverlay
    {
        void Update();
        Byte[] Draw();

    }
}
