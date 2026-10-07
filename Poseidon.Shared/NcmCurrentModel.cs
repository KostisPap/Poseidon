using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using Poseidon.Shared;

namespace Poseidon.Shared
{
    public class NcmCurrentModel : CurrentCalculator
    {
        private const double α = 0.142857142857143;
        private const double ucavg = 0.3;

        public NcmCurrentModel(Run run, LoadCase loadCase)
            : base(run, loadCase)
        {
        }

        public override double ComputeAt(double negativeElevationDownFromSwl)
        {
            double num1 = this.SWL + negativeElevationDownFromSwl;
            if (num1 == 0.0)
                num1 = 1E-05;
            double num2 = 0.9156 * (this.LoadCase.windSpeed * Math.Pow(10.0 / (155.6 - this.LoadCase.waterDepthDiffference), 0.1)) * 0.033 * (1.0 + negativeElevationDownFromSwl / 20.0);
            return (num2 < 0.0 ? 0.0 : num2) + 12.0 / 35.0 * Math.Pow(num1 / this.SWL, 1.0 / 7.0);
        }

        public override string CurrentModel { get; protected set; } = "NCM";
    }


}
