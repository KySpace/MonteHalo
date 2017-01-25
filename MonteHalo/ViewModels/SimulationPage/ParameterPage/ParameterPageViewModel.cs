using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MonteHalo.PhysicalModels;
namespace MonteHalo.ViewModels.SimulationPage.ParameterPage
{
    public class ParameterPageViewModel : Models.NotificationBase
    {
        public readonly string Title = "Parameters";
        public readonly string CrystalTitle = "Crystal";
        public readonly string SizeMaxText = "Maximum Size";
        public readonly string Rad1Text = "Radius 1";
        public readonly string Rad2Text = "Radius 2";
        public readonly string Rad3Text = "Radius 3";
        public readonly string HeightText = "Height";
        public readonly string LightTitle = "Light";
        public readonly string OrientationTypeText = "Orientation";
        public readonly string SolarAltitudeText = "Altitude of the Sun";
        public readonly string ColorMode = "Color Mode";
        public readonly string SimulationTimesText = "Total simulation times";
        
        public ParameterPageViewModel(ref Models.HexagonalSimulation simulationProgram)
        {
            this.simulation = simulationProgram;
            this.Orientation = new ObservableCollection<OrientationViewModel>
            {
                new OrientationViewModel(OrientationRequirement.Column),
                new OrientationViewModel(OrientationRequirement.Lowitz),
                new OrientationViewModel(OrientationRequirement.Parry),
                new OrientationViewModel(OrientationRequirement.Plate),
                new OrientationViewModel(OrientationRequirement.Random)
            };
            this.selectedOrientationIndex = -1;
            this.progressPermillage = 0;
            this.sunAltitude = 0;
            this.maxSize = this.rad1 = this.rad2 = this.rad3 = this.height = 0;
            this.PropertyChanged += ParameterPageViewModel_PropertyChanged;
            this.simulation.PropertyChanged += Simulation_PropertyChanged;
            this.IsReady = false;
        }

        private void Simulation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.ProgressPermillage = this.simulation.ProgressPermillage;
        }

        private void ParameterPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsReady") return;
            this.IsReady = true;
            if (this.selectedOrientationIndex == -1)
                this.IsReady = false;            
            else this.simulation.OrientationMode = this.SelectedOrientation.Mode;
            if (this.sunAltitude > 90 || this.sunAltitude < -90)
                this.IsReady = false;
            else this.simulation.SunDirection =
                    new GeoMath.UnitVector(0, 90 - this.sunAltitude, GeoMath.AngleUnit.Degree);
            if (this.simulationTimesTotal < 100 || this.simulationTimesTotal > 10000000)
                this.IsReady = false;
            else this.simulation.TotalSimulationTimes = this.SimulationTimesTotal;
            if (
                this.rad1 + this.rad2 < this.maxSize / 2 ||
                this.rad2 + this.rad3 < this.maxSize / 2 ||
                this.rad3 + this.rad1 < this.maxSize / 2 ||
                this.rad1 < -this.maxSize / 2 ||
                this.rad2 < -this.maxSize / 2 ||
                this.rad3 < -this.maxSize / 2 ||
                this.rad1 > this.maxSize ||
                this.rad2 > this.maxSize ||
                this.rad3 > this.maxSize ||
                this.height < 0
                ) this.IsReady = false;
            else
            {
                this.simulation.Rad1 = this.rad1;
                this.simulation.Rad2 = this.rad2;
                this.simulation.Rad3 = this.rad3;
                this.simulation.MaxSize = this.maxSize;
                this.simulation.Height = this.height;
            }
        }

        public bool isReady;
        public bool IsReady
        {
            get { return this.isReady; }
            set { SetProperty(ref this.isReady, value); }
        }

        public Models.HexagonalSimulation simulation;
        public ObservableCollection<OrientationViewModel> Orientation { get; set; }
        //private OrientationViewModel selectedOrientation;
        private int selectedOrientationIndex;
        public int SelectedOrientationIndex
        {
            get { return this.selectedOrientationIndex; }
            set
            {
                if (SetProperty(ref this.selectedOrientationIndex, value))
                    OnPropertyChanged(nameof(this.SelectedOrientationIndex));
            }
        }

        public OrientationViewModel SelectedOrientation
        {
            get { return this.Orientation[this.SelectedOrientationIndex]; }
            //set { SetProperty(ref this.selectedOrientation, value); }
        }

        private int progressPermillage;
        public int ProgressPermillage
        {
            get { return this.progressPermillage; }
            private set { SetProperty(ref this.progressPermillage, value); }
        }

        private double sunAltitude;
        public double SunAltitude
        {
            get { return this.sunAltitude; }
            set { SetProperty(ref this.sunAltitude, value); }
        }

        private int simulationTimesTotal;
        public int SimulationTimesTotal
        {
            get { return this.simulationTimesTotal; }
            set { SetProperty(ref this.simulationTimesTotal, value); }
        }

        private double maxSize;
        public double MaxSize
        {
            get { return this.maxSize; }
            set { SetProperty(ref this.maxSize, value); }
        }
        private double rad1;
        public double Rad1
        {
            get { return this.rad1; }
            set { SetProperty(ref this.rad1, value); }
        }
        private double rad2;
        public double Rad2
        {
            get { return this.rad2; }
            set { SetProperty(ref this.rad2, value); }
        }
        private double rad3;
        public double Rad3
        {
            get { return this.rad3; }
            set { SetProperty(ref this.rad3, value); }
        }
        private double height;
        public double Height
        {
            get { return this.height; }
            set { SetProperty(ref this.height, value); }
        }

        public void Start()
        {
            this.simulation.Clear();
            this.simulation.Prepare();
            //this.simulation.Start();
        }
        public void Stop()
        {
            this.simulation.Stop();
        }
    }
    public class OrientationViewModel
    {
        public readonly PhysicalModels.OrientationRequirement Mode;
        public readonly string Name;
        public readonly string Description;
        public OrientationViewModel(PhysicalModels.OrientationRequirement mode)
        {
            this.Mode = mode;
            switch(mode)
            {
                case OrientationRequirement.Column:
                    this.Name = "Column";
                    break;
                case OrientationRequirement.Lowitz:
                    this.Name = "Lowitz";
                    break;
                case OrientationRequirement.Parry:
                    this.Name = "Parry";
                    break;
                case OrientationRequirement.Plate:
                    this.Name = "Plate";
                    break;
                case OrientationRequirement.Random:
                    this.Name = "Random";
                    break;
            }

        }
    }
}
