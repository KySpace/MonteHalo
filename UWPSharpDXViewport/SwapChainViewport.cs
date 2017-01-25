using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPSharpDXViewport
{
    using System.Diagnostics;      
    using Windows.Graphics.Display;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Renders the contained 3-D content within the 2-D layout bounds of the Viewport3DX element.
    /// </summary>
    [ContentProperty(Name = "Items")]
    public sealed class SwapChainViewport : ItemsControl
    {
        /// <summary>
        /// The image source target
        /// </summary>
        private SwapChainTarget d3dTarget;
        /// <summary>
        /// The device manager
        /// </summary>
        private DeviceManager deviceManager;



        //Constructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="SwapChainViewport" /> class.
        /// </summary>
        public SwapChainViewport()
        {
            this.DefaultStyleKey = typeof(SwapChainViewport);
            this.Loaded += this.Viewport3DXLoaded;
            
        }


        /// <summary>
        /// Renders all content on the CompositionTarget.Rendering event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void CompositionTargetRendering(object sender, object e)
        {
            this.d3dTarget.RenderAll();
        }

        /// <summary>
        /// Changes the dpi of the device manager when the DisplayProperties.LogicalDpi has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void HandleEffectiveDpiChanged(DisplayInformation displayInfo, object sender)
        {
            this.deviceManager.Dpi = displayInfo.LogicalDpi;
        }

        /// <summary>
        /// Called after the size of the Viewport changeds
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">the parameters containing size info before and after the change</param>
        private void HandleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.d3dTarget.HandleSizeChanged(e.NewSize.Width, e.NewSize.Height);
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="deviceManager">The device manager.</param>
        private void Initialize(DeviceManager deviceManager)
        {
            foreach (Element3D e in this.Items)
            {
                e.Initialize(deviceManager);
            }
        }

        /// <summary>
        /// Renders the model.
        /// </summary>
        /// <param name="render">The render.</param>
        private void Render(SwapChainTarget render)
        {
            foreach (Element3D e in this.Items)
            {
                e.Render(render);
            }
        }

        /// <summary>
        /// Creates the device manager and image source when the viewport is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Viewport3DXLoaded(object sender, RoutedEventArgs e)
        {
            this.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            this.Arrange(new Windows.Foundation.Rect(0, 0, this.MinWidth, this.MinHeight));
            var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            int pixelWidth = (int)(this.ActualWidth * logicalDpi / 96.0);
            int pixelHeight = (int)(this.ActualHeight * logicalDpi / 96.0);

            // Safely dispose any previous instance
            // Creates a new DeviceManager (Direct3D, Direct2D, DirectWrite, WIC)
           this.deviceManager = new DeviceManager();

            // Use CoreWindowTarget as the rendering target (Initialize SwapChain, RenderTargetView, DepthStencilView, BitmapTarget)
            this.d3dTarget = new SwapChainTarget((SwapChainPanel)this.ItemsPanelRoot, this);

            this.deviceManager.Initialize += this.d3dTarget.Initialize;
            this.deviceManager.Initialize += this.Initialize;

            this.d3dTarget.Render += this.Render;

            // Initialize the device manager and all registered deviceManager.OnInitialize 
            this.deviceManager.OnInitialize(DisplayInformation.GetForCurrentView().LogicalDpi, this);

            // Setup rendering callback
            CompositionTarget.Rendering += this.CompositionTargetRendering;

            // Callback on SizeChanged
            this.SizeChanged += this.HandleSizeChanged;

            // Callback on DpiChanged
            DisplayInformation.GetForCurrentView().DpiChanged += this.HandleEffectiveDpiChanged;


        }
    }
}
