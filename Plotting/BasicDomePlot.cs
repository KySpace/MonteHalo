using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using D3D11 = SharpDX.Direct3D11;
using SharpDX.Mathematics;
using SharpDX.D3DCompiler;
using SharpDX.IO;

namespace MonteHalo.Graphics
{
    using UWPSharpDXViewport;
    public class BasicDomePlot : Element3D
    {
        public double Scale { get; set; }
        public bool ShowAxis { get; set; }
        public bool ShowRuler { get; set; }
        public double ScaleOfObject { get; set; }

        private IntPtr pointer;
        private int pointCount;
        private D3D11.DeviceContext context;
        private D3D11.Device device;
        private String path;
        private D3D11.Buffer scatterPointVertexBuffer;
        private D3D11.Buffer constantBuffer;
        private D3D11.InputLayout inputLayout;
        private D3D11.VertexShader vertexShader;
        private D3D11.PixelShader pixelShader;
        private SharpDX.D3DCompiler.ShaderSignature inputSignature;
        private D3D11.VertexBufferBinding vertexBufferBinding;
        private SharpDX.Matrix worldViewProj;
        public BasicDomePlot()
        {
            this.Scale = 1.0f;
        }
        public override void Initialize(DeviceManager deviceManager)
        {
            base.Initialize(deviceManager);
            // Setup local variables
            this.path = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Plotting");
            this.context = deviceManager.ContextDirect3D;
            this.device = deviceManager.DeviceDirect3D;
            this.pointCount = 0;
            this.pointer = IntPtr.Zero;
            this.ScaleOfObject = 5; //trial                

            this.SetShaders();
            this.InitializeConstantBuffer();
            this.InitializeScatterPointVertexBuffer();
            this.InitializeInputLayout();
            this.Initialized(this, EventArgs.Empty);
        }
        public override void Render(SwapChainTarget render)
        {
            base.Render(render);

            base.Render(render);

            double width = render.RenderTargetSize.Width;
            double height = render.RenderTargetSize.Height;

            // Set targets (This is mandatory in the loop) NOT REALLY SURE WHAT THIS MEANS
            this.context.OutputMerger.SetTargets(render.DepthStencilView, render.RenderTargetView);

            // Clear the views 
            //NOT REALLY SURE WHAT THIS MEANS
            this.context.ClearDepthStencilView(render.DepthStencilView, D3D11.DepthStencilClearFlags.Depth, 1.0f, 0);
            this.context.ClearRenderTargetView(render.RenderTargetView, Color.Black);
            //The last two parameters need to be altered
            this.worldViewProj = render.WVPMatrix;
            this.context.UpdateSubresource(ref this.worldViewProj, this.constantBuffer);
            //Binding IA Stage
            this.BindingIAStage();
            //
            this.context.Draw(this.pointCount, 0);
        }

        private void InitializeConstantBuffer()
        {
            //Remove previous buffer
            if (this.constantBuffer != null)
                this.constantBuffer.Dispose();
            this.constantBuffer = new D3D11.Buffer(
                this.device,
                Utilities.SizeOf<SharpDX.Matrix>(),
                D3D11.ResourceUsage.Default,
                D3D11.BindFlags.ConstantBuffer,
                D3D11.CpuAccessFlags.None,
                D3D11.ResourceOptionFlags.None,
                0
                );
        }
        private void InitializeScatterPointVertexBuffer()
        {
            if (this.scatterPointVertexBuffer != null)
                this.scatterPointVertexBuffer.Dispose();
            D3D11.BufferDescription vertexBufferDesc = new D3D11.BufferDescription(
                Utilities.SizeOf<ScatterVertex>() * 10000000,
                D3D11.ResourceUsage.Dynamic,
                D3D11.BindFlags.VertexBuffer,
                D3D11.CpuAccessFlags.Write,
                D3D11.ResourceOptionFlags.None,
                Utilities.SizeOf<ScatterVertex>()//Does it work? If not, try
                                                 //System.Runtime.InteropServices.Marshal
                );
            this.scatterPointVertexBuffer = new D3D11.Buffer(this.device, vertexBufferDesc);
            this.vertexBufferBinding = new D3D11.VertexBufferBinding(
                scatterPointVertexBuffer,
                Utilities.SizeOf<ScatterVertex>(),
                0
                );
        }
        private void SetShaders()
        {
            byte[] vertexShaderByteCode = ShaderBytecode.CompileFromFile(this.path + "\\Transf_VS.fx", "VS", "vs_5_0");
            this.vertexShader = new D3D11.VertexShader(
                device,
                vertexShaderByteCode
                );
            this.inputSignature = new ShaderSignature(vertexShaderByteCode);
            this.pixelShader = new D3D11.PixelShader(
                device,
                ShaderBytecode.CompileFromFile(this.path + "\\Pixel_PS.fx", "PS", "ps_5_0")
                );
            //byte[] vertexShaderByteCode = NativeFile.ReadAllBytes(this.path + "\\Transf_VS.fxo");
            //this.vertexShader = new D3D11.VertexShader(this.device, vertexShaderByteCode);
            //this.inputSignature = new ShaderSignature(vertexShaderByteCode);
            //byte[] pixelShaderByteCode = NativeFile.ReadAllBytes(this.path + "\\Pixel_PS.fxo");
            //this.pixelShader = new D3D11.PixelShader(this.device, pixelShaderByteCode);
        }
        private void InitializeInputLayout()
        {

            this.inputLayout = new D3D11.InputLayout(
                this.device,
                inputSignature,
                new SharpDX.Direct3D11.InputElement[]
                {
                        new D3D11.InputElement(
                            "Position",
                            0,
                            SharpDX.DXGI.Format.R32G32B32_Float,
                            0
                            ),
                        new D3D11.InputElement(
                            "Color",
                            0,
                            SharpDX.DXGI.Format.R32G32B32A32_Float,//I'm not very sure whether or why I should use this option
                            0
                            )
                }
                );
        }
        private void BindingIAStage()
        {
            this.context.InputAssembler.InputLayout = this.inputLayout;
            this.context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.PointList;
            this.context.InputAssembler.SetVertexBuffers(
                0,
                this.vertexBufferBinding
                );
            this.context.VertexShader.Set(this.vertexShader);
            this.context.PixelShader.Set(this.pixelShader);
            this.context.VertexShader.SetConstantBuffer(0, this.constantBuffer);
        }

        /// <summary>
        /// This is the method to update the buffer everytime a new point is generated.
        /// If the performance is not satisfying, consider using a equivalent method which can plot a bunch of points for once
        /// </summary>
        /// <param name="data">point data in ScatterVertex struct</param>
        public void UpdateVertexBuffer(ScatterVertex data)
        {
            DataBox dataBox =
            this.context.MapSubresource(
                scatterPointVertexBuffer,
                0,
                D3D11.MapMode.WriteNoOverwrite,
                D3D11.MapFlags.None
                );
            if (this.pointer == IntPtr.Zero)
                this.pointer = dataBox.DataPointer;// + this.pointCount * Utilities.SizeOf<ScatterVertex>();
            this.pointer = Utilities.WriteAndPosition(this.pointer, ref data);
            
            this.context.UnmapSubresource(scatterPointVertexBuffer, 0);
            this.pointCount++;
        }
        public void UpdateVertexBuffer(ref ScatterVertex[] array)
        {
            DataBox dataBox =
            this.context.MapSubresource(
                scatterPointVertexBuffer,
                0,
                D3D11.MapMode.WriteNoOverwrite,
                D3D11.MapFlags.None
                );
            var pointer = dataBox.DataPointer;
            for (int i = 0; i < array.Length; i++)
            {
                pointer = Utilities.WriteAndPosition(pointer, ref array[i]);
                this.pointCount++;
            }
            this.context.UnmapSubresource(scatterPointVertexBuffer, 0);

        }
        public void ClearBuffer()
        {
            DataBox dataBox =
            this.context.MapSubresource(
                scatterPointVertexBuffer,
                0,
                D3D11.MapMode.WriteDiscard,
                D3D11.MapFlags.None
                );
            this.pointer = dataBox.DataPointer;
            this.context.UnmapSubresource(scatterPointVertexBuffer, 0);
            this.pointCount = 0;
        }
        public event EventHandler Initialized;
    }
}
