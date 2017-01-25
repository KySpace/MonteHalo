using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonteHalo.PhysicalModels;
using MonteHalo.GeoMath;

namespace MonteHalo.Models
{
    public enum ColorMode { MonoChromatic, Sunlight, Uniform }
    public enum PolorizationMode { Polarized, Unpolarized }
    public class Simulation : NotificationBase
    {
        protected Graphics.BasicDomePlot plotResources;
        public static readonly int MaximumTimes = 10000000;
        public static readonly int ProgressSteps = 1000;
        private int totalSimulationTimes;
        public int TotalSimulationTimes
        {
            get { return this.totalSimulationTimes; }
            set
            {
                if (value > Simulation.MaximumTimes)
                    value = Simulation.MaximumTimes;
                else if (value < 0)
                    value = 0;
                else
                    this.SetProperty(ref this.totalSimulationTimes, value);
                this.stepLength = this.TotalSimulationTimes / Simulation.ProgressSteps;
            }
        }
        private int stepLength;

        private int currentSimulationTimes;
        public int CurrentSimulationTimes
        {
            get { return this.currentSimulationTimes; }
            set
            {
                this.currentSimulationTimes = value;
                if (value - this.lastSimulationTimes > this.stepLength)
                    this.ProgressPermillage =
                        this.CurrentSimulationTimes * Simulation.ProgressSteps / this.TotalSimulationTimes;
            }
        }
        private int lastSimulationTimes;
        private int progressPermillage;
        public int ProgressPermillage
        {
            get { return this.progressPermillage; }
            set
            {
                if (value <= Simulation.ProgressSteps && value > 0)
                    this.SetProperty(ref this.progressPermillage, value);
            }
        }

        public UnitVector SunDirection { get; set; }
        public ColorMode ColorMode;
        public PolorizationMode PolorMode;

        public double RefrIndex { get; set; }

        protected System.Threading.CancellationTokenSource tokenSource;
        protected Task SimulationTask;

        public bool IsPrepared = false;
        public virtual void Prepare()
        {
            this.tokenSource = new System.Threading.CancellationTokenSource();
        }
        public virtual void Start()
        {
            if (this.SimulationTask != null) SimulationTask.Start();
        }
        public virtual void Suspend()
        {
            //Incomplete
            if (this.SimulationTask != null && this.SimulationTask.Status == TaskStatus.Running) ;
        }
        public virtual void Stop()
        {
            this.tokenSource.Cancel();
        }
        public virtual void Clear()
        {
            //this.Stop();
            if (this.plotResources != null)
                this.plotResources.ClearBuffer();
            this.CurrentSimulationTimes = this.lastSimulationTimes
                = this.ProgressPermillage = 0; 
        }
        public virtual void SetPlotResources(Graphics.BasicDomePlot plot)
        {
            this.plotResources = plot;
        }
    }
    public class HexagonalSimulation : Simulation
    {
        public OrientationRequirement OrientationMode { get; set; }
        public double MaxSize { get; set; }
        public double Rad1 { get; set; }
        public double Rad2 { get; set; }
        public double Rad3 { get; set; }
        public double Height { get; set; }

        private HexaPrism crystal;
        public override void Prepare()
        {
            base.Prepare();                   
            this.SimulationTask = Task.Factory.StartNew(
                () => this.SimulationMethod()
            , this.tokenSource.Token
            );            
        }
        private void SimulationMethod()
        {
            this.crystal = new HexaPrism(
                this.Height, this.MaxSize, this.Rad1, this.Rad2, this.Rad3
                );
            UnitVector Output = new UnitVector();
            SharpDX.Vector3 Point;
            for (int i = 0; i < this.TotalSimulationTimes; i++)
            {
                Output = -this.crystal.SingleScattering(this.SunDirection, this.OrientationMode);
                Point = Output.ToVect3;
                this.plotResources.UpdateVertexBuffer(new Graphics.ScatterVertex(
                    Point, SharpDX.Color4.White
                    ));
                this.CurrentSimulationTimes = i;
            }
        }
        
    }
}
