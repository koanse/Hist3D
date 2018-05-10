using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace Hist3d
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            double[] arrLX = { 1, 4, 2, 4, 6, 6, 4 }, arrLY = { 3, 7, 1, 4, 4, 5, 3 };
            //double[,] matrZ = { { 1, 2, 3 }, { 2, 4, 1 }, { 2, 4, 5 } };
            double[,] matrZ = { { 1, 1, 2, 5, 4, 6, 4 }, { 1, 1.5, 1, 5, 3, 5, 4 },
                                  { 1, 1, 2, 3, 4, 2, 1 }, { 1, 2, 3, 3, 2, 5, 3 },
                                  { 2, 4, 3, 2, 4, 2, 2 }, { 4, 3, 2,5, 3, 2, 5 },
                                  { 2, 4, 3, 1, 1, 2, 1 }
                              };
            bool[,] matrT = new bool[,]
            {
                { true, true, true, true, true, true, true },
                { true, true, true, true, true, true, true },
                { true, true, false, false, false, true, true },
                { true, true, false, false, false, true, true },
                { true, true, false, false, false, true, true },
                { true, true, true, true, true, true, true },
                { true, true, true, true, true, true, true },
            };
            using (H3DForm frm = new H3DForm(arrLX, arrLY, matrZ, matrT, -1))
            {
                frm.Show();
                frm.InitializeGraphics();
                Application.Run(frm);
            }
        }
    }
}
