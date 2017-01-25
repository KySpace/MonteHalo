using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPSharpDXViewport
{
    using Windows.UI.Xaml;

    /// <summary>
    /// Base class for 3D elements.
    /// </summary>
    public abstract class Element3D : FrameworkElement
    {
        /// <summary>
        /// Current stop watch time in millisecond.
        /// </summary>
        protected long time;
        /// <summary>
        /// Time interval since last frame.
        /// </summary>
        protected long deltaTime;

        /// <summary>
        /// Initializes the element.
        /// </summary>
        /// <param name="deviceManager">The device manager.</param>
        public virtual void Initialize(DeviceManager deviceManager)
        {
        }

        /// <summary>
        /// Renders the element.
        /// </summary>
        /// <param name="render">The render.</param>
        public virtual void Render(SwapChainTarget render)
        {
            //To set time and time interval.
            this.deltaTime = render.Timer.ElapsedMilliseconds - this.time;
            this.time = render.Timer.ElapsedMilliseconds;
        }
    }
}

