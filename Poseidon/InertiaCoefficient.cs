using Poseidon.Shared;

namespace Poseidon
{
    public class InertiaCoefficient
    {
        /* Method... */
        public double CalculateCm(double D, double U, double T, double elevation, Run run, LoadCase loadCase)
        {
            /* Variable with the can MG thickness */
            double mgThickness = new();
            /* Iterate the marine growth profile layers and identify the applicable surface roughness */
            foreach (MarineGrowthElevation mgLayer in run.MarineGrowth)
            {
                if (elevation <= mgLayer.Upper - loadCase.waterDepthDiffference && elevation >= mgLayer.Lower - loadCase.waterDepthDiffference)
                { mgThickness = 2 * mgLayer.Roughness; }
            }

            /* Variable with the Surface roughness with MG */
            double k = new();
            if (mgThickness == 0) { k = run.RoughnessPainted; }
            else { k = run.RoughnessMG; }

            /* Keulegan-Carpenter number */
            double kc = U * T / (D + mgThickness);
            double delta = k / (D + mgThickness);

            /* Steady flow drag coefficient */
            double cds = delta switch
            {
                <= 1e-4 => 0.65,
                >= 1e-2 => 1.05,
                _ => (29 + 4 * Math.Log10(delta)) / 20
            };

            /* Coefficient for rough flow */
            double cmRough = (kc / 1.05) switch
            {
                < 0 => 0,
                < 7 => 2,
                < 17 => 2.56 - 0.08 * (kc / 1.05),
                _ => 1.2
            };
            /* Coefficient for smooth flow */
            double cmSmooth = (kc / 0.65) switch
            {
                < 0 => 0,
                < 9 => 2,
                < 19 => 2.36 - 0.04 * (kc / 0.65),
                _ => 1.6
            };

            /* Coefficient for smooth flow Interpolation between the cmSmooth and cmRough */
            double cm = cds switch
            {
                <= 0.65 => cmSmooth,
                >= 1.05 => cmRough,
                _ => cmSmooth + (cmRough - cmSmooth) * (cds - 0.65) / (1.05 - 0.65)

            };

            return cm;
        }
    }
}
