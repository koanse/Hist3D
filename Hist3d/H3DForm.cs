using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Hist3d
{
    public partial class H3DForm : Form
    {
        private Device device = null;
        float[] arrLX, arrLY, arrLZ;
        float[,] matrZ;
        bool[,] matrT;
        public float zMax;
        public H3DForm(double[] arrLX, double[] arrLY, double[,] matrF, bool[,] matrT, double zMax)
        {
            InitializeComponent();
            this.arrLX = new float[arrLX.Length];
            this.arrLY = new float[arrLY.Length];
            this.matrZ = new float[matrF.GetLength(0), matrF.GetLength(1)];
            this.matrT = matrT;
            this.zMax = (float)zMax;
            for (int i = 0; i < matrF.GetLength(0); i++)
            {
                this.arrLX[i] = (float)arrLX[i];
                for (int j = 0; j < matrF.GetLength(1); j++)
                {
                    this.arrLY[j] = (float)arrLY[j];
                    this.matrZ[i, j] = (float)matrF[i, j];
                }
            }
        }
        public void InitializeGraphics()
        {
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;
            presentParams.EnableAutoDepthStencil = true;
            presentParams.AutoDepthStencilFormat = DepthFormat.D16;
            device = new Device(0, DeviceType.Hardware, this,
                CreateFlags.SoftwareVertexProcessing, presentParams);
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);
                SetupCamera();
                device.BeginScene();
                DrawHist(arrLX, arrLY, matrZ, matrT, -1, Matrix.RotationZ(0));
                DrawHist(arrLX, arrLY, matrZ, matrT, -1, Matrix.RotationY((float)Math.PI / 2) *
                    Matrix.Translation(new Vector3(-1.3f, 0, 2.5f)));
                DrawHist(arrLX, arrLY, matrZ, matrT, -1, Matrix.RotationX((float)-Math.PI / 2) *
                    Matrix.Translation(new Vector3(0, -1.3f, 2.5f)));
                device.EndScene();
                device.Present();
            }
            catch { }
        }
        void SetupCamera()
        {
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 20, 1, 1, 100);
            device.Transform.View = Matrix.LookAtLH(new Vector3(13, 17, 15), new Vector3(0, 0, 1), new Vector3(0, 0, 1));
            device.RenderState.CullMode = Cull.None;
        }
        void DrawHist(float[] arrLX, float[] arrLY, float[,] matrZ, bool[,] matrT, float zMax, Matrix mWorld)
        {
            List<CustomVertex.PositionTextured> listV = new List<CustomVertex.PositionTextured>();
            List<CustomVertex.PositionColored> listVL = new List<CustomVertex.PositionColored>();
            float xSum = 0, ySum = 0;
            for (int i = 0; i < arrLX.Length; i++)
                xSum += arrLX[i];
            for (int i = 0; i < arrLY.Length; i++)
                ySum += arrLY[i];
            float x0 = 0, y0 = 0;
            if (zMax == -1)
            {
                zMax = 0;
                for (int i = 0; i < matrZ.GetLength(0); i++)
                    for (int j = 0; j < matrZ.GetLength(1); j++)
                        if (matrZ[i, j] > zMax)
                            zMax = matrZ[i, j];
            }
            for (int i = 0; i < arrLX.Length; i++)
            {
                float dx = arrLX[i] / xSum;
                y0 = 0;
                for (int j = 0; j < arrLY.Length; j++)
                {
                    float dy = arrLY[j] / ySum, dz = matrZ[i, j] / zMax;
                    CustomVertex.PositionTextured[] arrV;
                    if (!matrT[i, j])
                    {
                        CustomVertex.PositionTextured[] arrV1 = new CustomVertex.PositionTextured[]
                    {
                        new CustomVertex.PositionTextured(x0, y0, 0, 0, 0),
                        new CustomVertex.PositionTextured(x0 + dx, y0, 0, dx, 0),
                        new CustomVertex.PositionTextured(x0 + dx, y0, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0, y0, 0, 0, 0),
                        new CustomVertex.PositionTextured(x0 + dx, y0, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0, y0, dz, 0, dy),

                        new CustomVertex.PositionTextured(x0 + dx, y0, 0, 0, 0),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, 0, 0, dy),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0 + dx, y0, 0, 0, 0),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0 + dx, y0, dz, dx, 0),

                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, 0, 0, 0),
                        new CustomVertex.PositionTextured(x0, y0 + dy, 0, dx, 0),
                        new CustomVertex.PositionTextured(x0, y0 + dy, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, 0, 0, 0),
                        new CustomVertex.PositionTextured(x0, y0 + dy, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, 0, dy),

                        new CustomVertex.PositionTextured(x0, y0 + dy, 0, 0, 0),
                        new CustomVertex.PositionTextured(x0, y0, 0, 0, dy),
                        new CustomVertex.PositionTextured(x0, y0, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0, y0 + dy, 0, 0, 0),
                        new CustomVertex.PositionTextured(x0, y0, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0, y0 + dy, dz, dx, 0),

                        new CustomVertex.PositionTextured(x0, y0, dz, 0, 0),
                        new CustomVertex.PositionTextured(x0 + dx, y0, dz, dx, 0),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0, y0, dz, 0, 0),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, dx, dy),
                        new CustomVertex.PositionTextured(x0, y0 + dy, dz, 0, dy)
                    };
                        arrV = arrV1;
                    }
                    else
                    {
                        CustomVertex.PositionTextured[] arrV2 = new CustomVertex.PositionTextured[]
                    {
                        new CustomVertex.PositionTextured(x0, y0, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0, dz, 1, 1),

                        new CustomVertex.PositionTextured(x0 + dx, y0, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0, dz, 1, 1),

                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0 + dy, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0 + dy, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0 + dy, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, 1, 1),

                        new CustomVertex.PositionTextured(x0, y0 + dy, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0 + dy, 0, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0 + dy, dz, 1, 1),

                        new CustomVertex.PositionTextured(x0, y0, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0 + dx, y0 + dy, dz, 1, 1),
                        new CustomVertex.PositionTextured(x0, y0 + dy, dz, 1, 1)
                    };
                        arrV = arrV2;
                    }
                    int lc1 = Color.Black.ToArgb();
                    CustomVertex.PositionColored[] arrVL = new CustomVertex.PositionColored[]
                    {
                        new CustomVertex.PositionColored(x0, y0, 0, lc1),
                        new CustomVertex.PositionColored(x0 + dx, y0, 0, lc1),
                        new CustomVertex.PositionColored(x0 + dx, y0 + dy, 0, lc1),
                        new CustomVertex.PositionColored(x0, y0 + dy, 0, lc1),
                        new CustomVertex.PositionColored(x0, y0, dz, lc1),
                        new CustomVertex.PositionColored(x0 + dx, y0, dz, lc1),
                        new CustomVertex.PositionColored(x0 + dx, y0 + dy, dz, lc1),
                        new CustomVertex.PositionColored(x0, y0 + dy, dz, lc1)
                    };
                    int[] arrIL = { 0, 1, 1, 2, 2, 3, 3, 0, 0, 4, 1, 5, 2, 6, 3, 7, 4, 5, 5, 6, 6, 7, 7, 4 };
                    for (int k = 0; k < arrIL.Length; k++)
                        listVL.Add(arrVL[arrIL[k]]);
                    listV.AddRange(arrV);
                    y0 += dy;
                }
                x0 += dx;
            }
            device.Transform.World = mWorld;
            device.VertexFormat = CustomVertex.PositionTextured.Format;
            Texture t = new Texture(device, Properties.Resources.Tex1, 0, Pool.Managed);
            device.SetTexture(0, t);
            device.RenderState.Lighting = false;
            device.DrawUserPrimitives(PrimitiveType.TriangleList, listV.Count / 3, listV.ToArray());
            device.VertexFormat = CustomVertex.PositionColored.Format;
            device.DrawUserPrimitives(PrimitiveType.LineList, listVL.Count / 2, listVL.ToArray());
            int lc2 = Color.Black.ToArgb();
            CustomVertex.PositionColored[] arrVL2 = new CustomVertex.PositionColored[]
                {
                    new CustomVertex.PositionColored(0, 0, 0, lc2),
                    new CustomVertex.PositionColored(1.2f, 0, 0, lc2),
                    new CustomVertex.PositionColored(0, 0, 0, lc2),
                    new CustomVertex.PositionColored(0, 1.2f, 0, lc2),
                    new CustomVertex.PositionColored(0, 0, 0, lc2),
                    new CustomVertex.PositionColored(0, 0, 1.2f, lc2)
                };
            device.DrawUserPrimitives(PrimitiveType.LineList, 3, arrVL2);
        }
    }
}
