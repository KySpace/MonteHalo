using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MonteHalo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrialFrame : Page
    {
        private Graphics.TestPointPlot Plot;
        private Task trialTask;
        public TrialFrame()
        {
            this.InitializeComponent();            
        }
        private void initPointPlot_Initialized(object sender, EventArgs e)
        {
            this.Plot = sender as Graphics.TestPointPlot;
            this.trialTask = new Task(() => this.tempTest());
            this.trialTask.Start();
        }
        private void tempTest()
        {
            Tests.TempTest.test3(this.Plot as Graphics.TestPointPlot);
        } 
        private void Viewport_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.Viewport.Focus(FocusState.Programmatic);
        }
    }
}
