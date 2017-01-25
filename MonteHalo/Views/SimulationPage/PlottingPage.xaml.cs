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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MonteHalo.Views.SimulationPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlottingPage : Page
    {
        public PlottingPage()
        {
            this.InitializeComponent();
        }

        private void Plot_Initialized(object sender, EventArgs e)
        {
            Shell.SimulationProgram.SetPlotResources(sender as Graphics.BasicDomePlot);
            //Tests.InitPointTest.Test1(sender as Graphics.BasicDomePlot);
            //Tests.ConversionTest.Test2(sender as Graphics.BasicDomePlot);
            //Tests.AlgorithmTest.RandomDirection.Test1(sender as Graphics.BasicDomePlot);
            //Tests.AlgorithmTest.RandomGenerators.TriangularRandom(sender as Graphics.BasicDomePlot);
        }

        private void Viewport_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.Viewport.Focus(FocusState.Programmatic);
        }
    }
}
