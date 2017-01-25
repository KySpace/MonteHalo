using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SharpDX.Mathematics;
using SharpDX;
using Input = Windows.UI.Xaml.Input;

namespace UWPSharpDXViewport
{
    namespace Camera
    {
        public class Camera : Element3D
        {
            //Public members that can be set.
            public ObserverMode ModeSetting;
            public float ScaleSetting;

            //Resources, settings, delegates.
            /// <summary>
            /// The viewport which contains this Camera.
            /// </summary>
            private SwapChainViewport viewport;
            /// <summary>
            /// KeyboardSettings, right now seems no use.
            /// </summary>
            private KeyboardSettings keySettings;
            /// <summary>
            /// Move look status, recording boolean values of current keyboard info.
            /// </summary>
            private MoveLookState moveLookState;
            /// <summary>
            /// A delegate that represent rendering move look updates,
            /// assigned with different methods in different modes.
            /// </summary>
            private MoveLookRender moveLookRender;


            //Camera modes.
            /// <summary>
            /// Erecting mode of the coordinate system.
            /// Currently only "always up" is supported.
            /// </summary>
            private ErectMode ereMode;
            public ErectMode EreMode
            {
                get { return this.ereMode; }
                set { this.ereMode = value; }
            }
            /// <summary>
            /// Observer mode, free look, target locked, or position locked.
            /// </summary>
            private ObserverMode obsmode;
            public ObserverMode ObsMode
            {
                get { return this.obsmode; }
                set
                {
                    obsmode = value;                   

                    //Free look operations.
                    switch (value)
                    {
                        case ObserverMode.Free:
                            this.moveLookRender = this.FreeRender;
                            break;
                        case ObserverMode.PositionLocked:
                            this.moveLookRender = this.PositionLockedRender;
                            break;
                        case ObserverMode.TargetLocked:
                            this.moveLookRender = this.TargetLockedRender;
                            break;
                    }

                }
            }
            /// <summary>
            /// Projection modee, orthographic or perspective.
            /// </summary>
            private ProjectionMode projMode;
            public ProjectionMode ProjMode
            {
                get { return this.projMode; }
                set
                {
                    this.projMode = value;                    
                }
            }

            //Orthographic projection width and height.
            private float ScreenWidth
            {
                get { return this.ScreenHeight * this.Aspect; }
                set { this.ScreenHeight = value / this.Aspect; }
            }
            private float ScreenHeight
            {
                get { return (this.Target - this.Eye).Length() * (float)Math.Tan(this.Fov); }
                set { this.Fov = (float)Math.Atan(value / (this.Target - this.Eye).Length()); }
            }

            //Mouse action parameters.
            /// <summary>
            /// The position of pointer on screen when it was last pressed.
            /// </summary>
            private Windows.Foundation.Point anchorPoint;
            /// <summary>
            /// The current position of pointer on screen.
            /// </summary>
            private Windows.Foundation.Point currentPoint;

            /// <summary>
            /// Maximum field of view allowed when zooming.
            /// </summary>
            private float MaxFov;
            /// <summary>
            /// Minimum field of view allowed when zooming.
            /// </summary>
            private float MinFov;
            /// <summary>
            /// Default field of view.
            /// </summary>
            private float DefFov;

            //Speed for all kinds of movement.
            /// <summary>
            /// Zoom rate, ratio per millisecond.
            /// </summary>
            public float ZoomRate;
            /// <summary>
            /// Camera moving speed, unit per millisecond.
            /// </summary>
            public float MoveSpeed;
            /// <summary>
            /// Default moving speed, radians per millisecond.
            /// </summary>
            public float RotationSpeed;

            /// <summary>
            /// Rough scale of the observed object, used to determine near, far plane. etc.
            /// </summary>
            public float Scale;


            //Camera geometry parameters.

            /// <summary>
            /// The position of the camera
            /// </summary>
            private Vector3 eye;
            private Vector3 Eye
            {
                get
                {
                    return this.eye;
                }
                set
                {
                    this.eye = value;
                    if (this.ObsMode == ObserverMode.TargetLocked)
                    {
                        this.to = this.Target - this.Eye;
                        this.to.Normalize();
                    }
                }
            }
            /// <summary>
            /// The looking spot of the camera
            /// </summary>
            private Vector3 target;
            private Vector3 Target
            {
                get
                {
                    return this.target;
                }
                set
                {
                    this.target = value;
                    
                    this.to = this.Target - this.Eye;
                    this.to.Normalize();
                    
                }
            }
            /// <summary>
            /// The up vector
            /// </summary>    
            private Vector3 erected;
            private Vector3 Erected
            {
                get
                {
                    return this.erected;
                }
                set
                {
                    this.erected = value;
                    this.erected.Normalize();
                    this.up = Vector3.Cross(this.To, Vector3.Cross(this.Erected, this.To));
                }
            }

            /// <summary>
            /// Same as Eye, camera position
            /// </summary>
            private Vector3 At
            {
                get
                {
                    return this.eye;
                }
                set
                {
                    this.Eye = value;
                }
            }
            /// <summary>
            /// The unit vector representing the direction looking at
            /// </summary>
            private Vector3 to;
            private Vector3 To
            {
                get
                {
                    return to;
                }
                set
                {
                    value.Normalize();
                    if (this.ObsMode != ObserverMode.TargetLocked)
                    {
                        float Distance = (this.Target - this.Eye).Length();
                        if (Distance == 0) Distance = Scale;
                        //To set the target according to the direction, keep the target in the same distance
                        this.target = Distance * value + this.Eye;
                    }
                    this.up = Vector3.Cross(this.To, Vector3.Cross(this.Erected, this.To));
                    this.to = value;
                }
            }
            /// <summary>
            /// The up vector of the camera
            /// </summary>
            private Vector3 up;
            private Vector3 Up
            {
                get
                {
                    return this.up;
                }
                set
                {
                    this.up = value;
                    this.up.Normalize();
                    this.erected = Vector3.Cross(Vector3.UnitX, Vector3.Cross(this.up, Vector3.UnitX));
                }
            }
            /// <summary>
            /// Unit vector that points to the left direction.
            /// </summary>
            private Vector3 Left
            {
                get
                {
                    var Result = Vector3.Cross(Up, To);
                    Result.Normalize();
                    return Result;
                }
            }


            //Projection parameters. 

            /// <summary>
            /// Field of view in y direction.
            /// </summary>
            private float fov;
            /// <summary>
            /// Field of view in y direction.
            /// </summary>
            private float Fov
            {
                get
                {
                    return this.fov;
                }
                set
                {
                    if (value > this.MaxFov)
                        this.fov = MaxFov;
                    else if (value < this.MinFov)
                        this.fov = MinFov;
                    else this.fov = value;
                }

            }
            /// <summary>
            /// View space width divided by hight.
            /// </summary>
            public float Aspect;
            /// <summary>
            /// Near plane distance.
            /// </summary>
            public float Near { get; set; }
            /// <summary>
            /// Far plane distance.
            /// </summary>
            public float Far { get; set; }

            //Projection matrices.
            /// <summary>
            /// View matrix.
            /// </summary>
            private Matrix View { get { return Matrix.LookAtRH(this.Eye, this.Target, this.Up); } }
            /// <summary>
            /// Projection matrix.
            /// </summary>
            private Matrix Proj
            {
                get
                {
                    switch (this.ProjMode)
                    {
                        case ProjectionMode.Orthographic:
                            return Matrix.OrthoRH(this.ScreenWidth, this.ScreenHeight, this.Near, this.Far);
                        case ProjectionMode.Perspective:
                            return Matrix.PerspectiveFovRH(this.Fov, this.Aspect, this.Near, this.Far);
                        //Incomplete exception handling
                        default:
                            return Matrix.Zero;
                    }
                }
            }
            /// <summary>
            /// World view projection matrix, the multiplication of view and projection matrix.
            /// </summary>
            public Matrix WorldViewProject
            {
                get
                {
                    Matrix wvp = Matrix.Multiply(this.View, this.Proj);
                    wvp.Transpose();
                    return wvp;
                }
            }


            



            //Event handlers.

            private void Viewport_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            {
                var Pointer = e.GetCurrentPoint(this.viewport);
                int Delta = Pointer.Properties.MouseWheelDelta;
                Windows.Foundation.Point Point = Pointer.Position;

                if (Delta > 0)
                    this.Fov *= 1 - this.deltaTime * (this.ZoomRate - 1);
                else if (Delta < 0)
                    this.Fov *= 1 + this.deltaTime * (this.ZoomRate - 1);
            }

            private void Viewport_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            {
                this.moveLookState.Dragging = false;
            }

            private void Viewport_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            {
                if (this.moveLookState.Dragging)
                {
                    this.currentPoint = e.GetCurrentPoint(this.viewport).Position;
                }
            }

            private void Viewport_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            {
                this.moveLookState.Dragging = true;
                this.anchorPoint = e.GetCurrentPoint(this.viewport).Position;
            }

            private void Viewport_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
            {
                var Key = e.Key;
                switch (Key)
                {
                    case Windows.System.VirtualKey.W:
                        this.moveLookState.ForwardKey = false;
                        break;
                    case Windows.System.VirtualKey.S:
                        this.moveLookState.BackwardKey = false;
                        break;
                    case Windows.System.VirtualKey.A:
                        this.moveLookState.LeftwardKey = false;
                        break;
                    case Windows.System.VirtualKey.D:
                        this.moveLookState.RightwardKey = false;
                        break;
                    case Windows.System.VirtualKey.Space:
                        this.moveLookState.UpwardKey = false;
                        break;
                    case Windows.System.VirtualKey.Control:
                        this.moveLookState.DownwardKey = false;
                        break;
                    case Windows.System.VirtualKey.Up:
                        this.moveLookState.UpTurnKey = false;
                        break;
                    case Windows.System.VirtualKey.Down:
                        this.moveLookState.DownTurnKey = false;
                        break;
                    case Windows.System.VirtualKey.Right:
                        this.moveLookState.RightTurnKey = false;
                        break;
                    case Windows.System.VirtualKey.Left:
                        this.moveLookState.LeftTurnKey = false;
                        break;
                    case Windows.System.VirtualKey.M:
                        this.moveLookState.ZoomIn = false;
                        break;
                    case Windows.System.VirtualKey.N:
                        this.moveLookState.ZoomOut = false;
                        break;

                }
            }

            private void Viewport_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
            {
                var Key = e.Key;
                switch (Key)
                {
                    case Windows.System.VirtualKey.W:
                        this.moveLookState.ForwardKey = true;
                        this.moveLookState.BackwardKey = false;
                        break;
                    case Windows.System.VirtualKey.S:
                        this.moveLookState.BackwardKey = true;
                        this.moveLookState.ForwardKey = false;
                        break;
                    case Windows.System.VirtualKey.A:
                        this.moveLookState.LeftwardKey = true;
                        this.moveLookState.RightwardKey = false;
                        break;
                    case Windows.System.VirtualKey.D:
                        this.moveLookState.RightwardKey = true;
                        this.moveLookState.LeftwardKey = false;
                        break;
                    case Windows.System.VirtualKey.Space:
                        this.moveLookState.UpwardKey = true;
                        this.moveLookState.DownwardKey = false;
                        break;
                    case Windows.System.VirtualKey.Control:
                        this.moveLookState.DownwardKey = true;
                        this.moveLookState.UpwardKey = false;
                        break;
                    case Windows.System.VirtualKey.Up:
                        this.moveLookState.UpTurnKey = true;
                        this.moveLookState.DownTurnKey = false;
                        break;
                    case Windows.System.VirtualKey.Down:
                        this.moveLookState.DownTurnKey = true;
                        this.moveLookState.UpTurnKey = false;
                        break;
                    case Windows.System.VirtualKey.Right:
                        this.moveLookState.RightTurnKey = true;
                        this.moveLookState.LeftTurnKey = false;
                        break;
                    case Windows.System.VirtualKey.Left:
                        this.moveLookState.LeftTurnKey = true;
                        this.moveLookState.RightTurnKey = false;
                        break;
                    case Windows.System.VirtualKey.M:
                        this.moveLookState.ZoomIn = true;
                        this.moveLookState.ZoomOut = false;
                        break;
                    case Windows.System.VirtualKey.N:
                        this.moveLookState.ZoomOut = true;
                        this.moveLookState.ZoomIn = false;
                        break;

                }
            }

            //Main methods.

            public Camera() { }
            

            public override void Initialize(DeviceManager deviceManager)
            {
                base.Initialize(deviceManager);
                this.viewport = deviceManager.viewport;
                this.viewport.KeyDown += Viewport_KeyDown;
                this.viewport.KeyUp += Viewport_KeyUp;
                this.viewport.PointerPressed += Viewport_PointerPressed;
                this.viewport.PointerMoved += Viewport_PointerMoved;
                this.viewport.PointerReleased += Viewport_PointerReleased;
                this.viewport.PointerWheelChanged += Viewport_PointerWheelChanged;
                this.Aspect = (float)this.viewport.ActualWidth / (float)this.viewport.ActualHeight;
                this.viewport.SizeChanged += Viewport_SizeChanged;




                if (this.Scale == 0) this.Scale = 5.0f;
                if (this.ZoomRate == 0) this.ZoomRate = 1.002f;                                
                
                switch (this.ObsMode)
                {
                    case ObserverMode.PositionLocked:
                        this.Eye = Vector3.Zero;
                        this.To = Vector3.UnitX;
                        break;
                    case ObserverMode.Free:
                        if (this.Eye == this.Target) this.Eye = this.Scale * 5.0f * Vector3.UnitX + this.Target;
                        this.To = this.Target - this.Eye;                      
                        break;
                    case ObserverMode.TargetLocked:
                        if (this.Eye == this.Target) this.Eye = this.Scale * 5.0f * Vector3.UnitX + this.Target;
                        break;
                    //Incomplete exception handling
                    default: break;
                }
                

                if (this.MaxFov == 0) this.MaxFov = (float)Math.PI / 2.0f;
                if (this.MinFov == 0) this.MinFov = (float)Math.PI / 50.0f;
                if (this.DefFov == 0) this.DefFov = this.Scale / (this.Eye - this.Target).Length() * 1.5f;
                if (this.Fov == 0) this.Fov = this.DefFov;

                if (this.Near == 0) this.Near = this.Scale / 1000.0f;
                if (this.Far == 0) this.Far = this.Scale * 1000.0f;

                if (this.Up == default(Vector3)) this.Up = Vector3.UnitZ;
                if (this.EreMode == default(ErectMode)) this.EreMode = ErectMode.AlwaysUp;
                if (this.MoveSpeed == 0) this.MoveSpeed = this.Scale / 1000.0f * 1.5f;
                if (this.RotationSpeed == 0) this.RotationSpeed = 2 * (float)Math.PI / 1000.0f / 20.0f;

                this.ResetMoveLookStates();                                    
            }

            private void Viewport_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
            {
                this.Aspect = (float)this.viewport.ActualWidth / (float)this.viewport.ActualHeight;
            }

            public override void Render(SwapChainTarget render)
            {
                base.Render(render);

                this.moveLookRender();
                //Update the matrix in viewport.
                render.WVPMatrix = this.WorldViewProject;
            }


            private void Turn(float pitch, float yaw)
            {
                //Matrix3x3 Rotation = Matrix3x3.RotationYawPitchRoll
                ////I am not very sure whether this is transforming the vector or the coordinate system.
                ////But I'm expecting to change the vector itself here.
                //this.To = Vector3.Transform(this.To, Rotation);
                this.To += yaw * this.Up + pitch * this.Left;
            }
            private void Survey(float lon, float lat)
            {
                Vector3 Temp = (lat * this.Up + lon * this.Left + this.At);
                Temp.Normalize();
                this.At = (this.At - this.Target).Length() * Temp + Target;
            }
            private void ResetMoveLookStates()
            {
                this.moveLookState = new MoveLookState()
                {
                    UpwardKey = false,
                    DownwardKey = false,
                    ForwardKey = false,
                    BackwardKey = false,
                    LeftwardKey = false,
                    RightwardKey = false,
                    UpTurnKey = false,
                    DownTurnKey = false,
                    LeftTurnKey = false,
                    RightTurnKey = false,
                    ZoomIn = false,
                    ZoomOut = false,
                    Dragging = false
                };
            }
            private void TargetLockedRender()
            {
                float Lon = 0, Lat = 0;
                if ((this.Target - this.Eye).Length() > this.Near * 5)
                {
                    if (this.moveLookState.UpwardKey && (this.Eye - this.Target).Length() > (this.Near + this.Scale))
                        this.Eye += (this.Target - this.Eye) * this.MoveSpeed * this.deltaTime;
                    if (this.moveLookState.DownwardKey && (this.Eye - this.Target).Length() < (this.Far / 1.5f))
                        this.Eye -= (this.Target - this.Eye) * this.MoveSpeed * this.deltaTime;
                }
                if (this.moveLookState.ForwardKey && (this.Eye - this.Target).Length() < (this.Far / 1.5f))
                    Lat = this.MoveSpeed * this.deltaTime;
                if (this.moveLookState.BackwardKey && (this.Eye - this.Target).Length() < (this.Far / 1.5f))
                    Lat = -this.MoveSpeed * this.deltaTime;
                if (this.moveLookState.LeftwardKey && (this.Eye - this.Target).Length() < (this.Far / 1.5f))
                    Lon = this.MoveSpeed * this.deltaTime;
                if (this.moveLookState.RightwardKey && (this.Eye - this.Target).Length() < (this.Far / 1.5f))
                    Lon = -this.MoveSpeed * this.deltaTime;
                if (Lat != 0 || Lon != 0)
                    Survey(Lon, Lat);
            }
            private void PositionLockedRender()
            {
                float Pitch = 0, Yaw = 0;

                if (this.moveLookState.ForwardKey)
                    Yaw = this.RotationSpeed * this.deltaTime;
                if (this.moveLookState.BackwardKey)
                    Yaw = -this.RotationSpeed * this.deltaTime;
                if (this.moveLookState.LeftwardKey)
                    Pitch = this.RotationSpeed * this.deltaTime;
                if (this.moveLookState.RightwardKey)
                    Pitch = -this.RotationSpeed * this.deltaTime;

                if (this.moveLookState.UpTurnKey)
                    Yaw = this.RotationSpeed * this.deltaTime;
                if (this.moveLookState.DownTurnKey)
                    Yaw = -this.RotationSpeed * this.deltaTime;
                if (this.moveLookState.LeftTurnKey)
                    Pitch = this.RotationSpeed * this.deltaTime;
                if (this.moveLookState.RightTurnKey)
                    Pitch = -this.RotationSpeed * this.deltaTime;

                this.Turn(Pitch, Yaw);
            }
            private void FreeRender()
            {
                float Pitch = 0, Yaw = 0;
                if (this.moveLookState.UpwardKey)
                    this.Eye += this.Up * this.deltaTime * this.MoveSpeed;
                if (this.moveLookState.DownwardKey)
                    this.Eye -= this.Up * this.deltaTime * this.MoveSpeed;
                if (this.moveLookState.ForwardKey)
                    this.Eye += this.To * this.deltaTime * this.MoveSpeed;
                if (this.moveLookState.BackwardKey)
                    this.Eye -= this.To * this.deltaTime * this.MoveSpeed;
                if (this.moveLookState.LeftwardKey)
                    this.Eye += this.Left * this.deltaTime * this.MoveSpeed;
                if (this.moveLookState.RightwardKey)
                    this.Eye -= this.Left * this.deltaTime * this.MoveSpeed;


                if (this.moveLookState.UpTurnKey)
                    Yaw = this.RotationSpeed * this.deltaTime;
                if (this.moveLookState.DownTurnKey)
                    Yaw = -this.RotationSpeed * this.deltaTime;
                if (this.moveLookState.LeftTurnKey)
                    Pitch = this.RotationSpeed * this.deltaTime;
                if (this.moveLookState.RightTurnKey)
                    Pitch = -this.RotationSpeed * this.deltaTime;

                this.Turn(Pitch, Yaw);

            }
            //private void ResetKeys()
            //{
            //    this.keySettings = new KeySettings()
            //    {
            //        UpwardKey = Windows.System.VirtualKey.Space,
            //        DownwardKey = Windows.System.VirtualKey.Control,
            //        FowardKey = Windows.System.VirtualKey.W,
            //        BackwardKey = Windows.System.VirtualKey.S,
            //        LeftwardKey = Windows.System.VirtualKey.A,
            //        RightwardKey = Windows.System.VirtualKey.D,
            //        UpTurnKey = Windows.System.VirtualKey.Up,
            //        DownTurnKey = Windows.System.VirtualKey.Down,
            //        RightTurnKey = Windows.System.VirtualKey.Right,
            //        LeftTurnKey = Windows.System.VirtualKey.Left,
            //        ZoomIn = Windows.System.VirtualKey.M,
            //        ZoomOut = Windows.System.VirtualKey.N
            //    };
            //}
        }


        public enum ProjectionMode { Orthographic, Perspective };
        public enum ObserverMode { Free, TargetLocked, PositionLocked }
        public enum ErectMode { AlwaysUp, EnableTurning }

        public struct MoveLookState
        {
            //Keyboard action parameters.
            public bool UpwardKey;
            public bool DownwardKey;
            public bool ForwardKey;
            public bool BackwardKey;
            public bool LeftwardKey;
            public bool RightwardKey;

            public bool UpTurnKey;
            public bool DownTurnKey;
            public bool LeftTurnKey;
            public bool RightTurnKey;

            //Zooming
            public bool ZoomIn;
            public bool ZoomOut;

            //MouseActions
            public bool Dragging;

            public MoveLookState(bool value)
            {
                this.UpwardKey = false;
                this.DownwardKey = false;
                this.ForwardKey = false;
                this.BackwardKey = false;
                this.LeftwardKey = false;
                this.RightwardKey = false;
                this.UpTurnKey = false;
                this.DownTurnKey = false;
                this.LeftTurnKey = false;
                this.RightTurnKey = false;
                this.ZoomIn = false;
                this.ZoomOut = false;
                this.Dragging = false;
            }

        }

        class KeyboardSettings
        {
            public Windows.System.VirtualKey UpwardKey;
            public Windows.System.VirtualKey DownwardKey;
            public const Windows.System.VirtualKey FowardKey = Windows.System.VirtualKey.W;
            public Windows.System.VirtualKey BackwardKey;
            public Windows.System.VirtualKey LeftwardKey;
            public Windows.System.VirtualKey RightwardKey;
            public Windows.System.VirtualKey UpTurnKey;
            public Windows.System.VirtualKey DownTurnKey;
            public Windows.System.VirtualKey LeftTurnKey;
            public Windows.System.VirtualKey RightTurnKey;
            public Windows.System.VirtualKey ZoomIn;
            public Windows.System.VirtualKey ZoomOut;
        }
        delegate void MoveLookRender();
    }

}