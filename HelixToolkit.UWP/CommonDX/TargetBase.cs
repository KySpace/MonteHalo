// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetBase.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   This class is an abstract class responsible to maintain a list of 
//   render task, render target view / depth stencil view and Direct2D 
//   render target.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace HelixToolkit.UWP.CommonDX
{
    /// <summary>
    /// This class is an abstract class responsible to maintain a list of 
    /// render task, render target view / depth stencil view and Direct2D 
    /// render target.
    /// </summary>
    /// <remarks>
    /// SharpDX CommonDX is inspired from the DirectXBase C++ class from Win8
    /// Metro samples, but the design is slightly improved in order to reuse 
    /// components more easily. 
    /// <see cref="DeviceManager"/> is responsible for device creation.
    /// <see cref="TargetBase"/> is responsible for rendering, render target 
    /// creation.
    /// Initialization and Rendering is event driven based, allowing a better
    /// reuse of different components.
    /// </remarks>
    public abstract class TargetBase : DisposeCollector
    {
        protected SharpDX.Direct3D11.RenderTargetView renderTargetView;
        protected SharpDX.Direct3D11.DepthStencilView depthStencilView;
        protected SharpDX.Direct2D1.Bitmap1 bitmapTarget;
        protected SharpDX.Direct3D11.Texture2D backBuffer;
        public SharpDX.Matrix WVPMatrix;
        public System.Diagnostics.Stopwatch Timer { get; private set; }

        public Camera.Camera Camera { get; protected set; }        

        /// <summary>
        /// Gets the <see cref="DeviceManager"/> attached to this instance.
        /// </summary>
        public DeviceManager DeviceManager { get; private set; }

        /// <summary>
        /// Gets the Direct3D RenderTargetView used by this target.
        /// </summary>
        public SharpDX.Direct3D11.RenderTargetView RenderTargetView { get { return renderTargetView; } }

        public SharpDX.Direct3D11.Texture2D BackBuffer { get { return backBuffer; } }

        /// <summary>
        /// Gets the Direct3D DepthStencilView used by this target.
        /// </summary>
        public SharpDX.Direct3D11.DepthStencilView DepthStencilView { get { return depthStencilView; } }

        /// <summary>
        /// Gets the Direct2D RenderTarget used by this target.
        /// </summary>
        public SharpDX.Direct2D1.Bitmap1 BitmapTarget2D { get { return bitmapTarget; } }

        /// <summary>
        /// Gets the bounds of the control linked to this render target
        /// </summary>
        public Windows.Foundation.Rect RenderTargetBounds { get; protected set; }

        /// <summary>
        /// Gets the size in pixels of the Direct3D RenderTarget
        /// </summary>
        public Windows.Foundation.Size RenderTargetSize { get { return new Windows.Foundation.Size(RenderTargetBounds.Width, RenderTargetBounds.Height); } }

        /// <summary>
        /// Gets the bounds of the control linked to this render target
        /// </summary>
        public Windows.Foundation.Rect ControlBounds { get; protected set; }
        
        /// <summary>
        /// Gets the current bounds of the control linked to this render target
        /// </summary>
        protected abstract Windows.Foundation.Rect CurrentControlBounds
        {
            get;
        }

        /// <summary>
        /// Event fired when size of the underlying control is changed
        /// </summary>
        public event Action<TargetBase> SizeChanged;

        /// <summary>
        /// Event fired when rendering is performed by this target
        /// </summary>
        public event Action<TargetBase> Render;

        /// <summary>
        /// Initializes a new <see cref="TargetBase"/> instance 
        /// </summary>
        public TargetBase()
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="deviceManager">The device manager</param>
        public virtual void Initialize(DeviceManager deviceManager)
        {
            this.DeviceManager = deviceManager;

            // If the DPI is changed, we need to perform a OnSizeChanged event
            deviceManager.OnDpiChanged -= HandleDpiChanged;
            deviceManager.OnDpiChanged += HandleDpiChanged;
        }

       

        /// <summary>
        /// Render all events registered on event <see cref="Render"/>
        /// </summary>
        public virtual void RenderAll()
        {
            if (Render != null)
                Render(this);
        }

        protected virtual void HandleDpiChanged(DeviceManager obj)
        {
            if (SizeChanged != null)
                SizeChanged(this);
        }
        
    }
}