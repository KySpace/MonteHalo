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
    public sealed partial class ParameterPage : Page
    {
        public ViewModels.SimulationPage.ParameterPage.ParameterPageViewModel viewModel { get; set; }
        public ParameterPage()
        {
            this.InitializeComponent();
            this.viewModel = new ViewModels.SimulationPage.ParameterPage.ParameterPageViewModel(ref Shell.SimulationProgram);
        }

        public static Action Start;
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            ParameterPage.Start();
            this.viewModel.Start();
        }
    }
}
