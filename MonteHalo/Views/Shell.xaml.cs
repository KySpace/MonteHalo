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

namespace MonteHalo.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public Shell()
        {
            this.InitializeComponent();
            Shell.SimulationProgram = new Models.HexagonalSimulation();
        }

        public static Models.HexagonalSimulation SimulationProgram;


        private void Simulation_Checked(object sender, RoutedEventArgs e)
        {
            this.MyFrame.Navigate(typeof(SimulationPage.SimulationPage));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            this.HamburgerPane.IsPaneOpen = !this.HamburgerPane.IsPaneOpen;
        }
    }
}
