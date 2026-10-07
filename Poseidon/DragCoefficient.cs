using Poseidon.Shared;

namespace Poseidon
{
    /* Class */
    public class DragCoefficient
    {
        /* Method... */
        public double CalculateCd(double D, double U, double T, double elevation, Run run,  LoadCase loadCase)
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

            /* Steady flow drag coefficient */
            double cds = new double();
            /* Keulegan-Carpenter number */
            double kc = U * T / (D + mgThickness);
            double psi = new();
            double cd = new();
            switch (k / (D + mgThickness))
            {
                /* Smooth */
                case <= 1e-4:
                    cds = 0.65;
                    switch (kc / cds)
                    {
                        case < 0:
                            // Invalid
                            break;

                        case < 1.15:
                            psi = 1.75 - 1.3 * kc / cds;
                            break;

                        case < 3.1:
                            psi = 0.25;
                            break;

                        case < 18.5:
                            psi = 0.25 + 0.065 * (kc / cds - 3.1);
                            break;

                        case < 40:
                            psi = 1.25 - 0.007 * (kc / cds - 18.5);
                            break;

                        case < 100:
                            psi = 1.1 - 0.00167 * (kc / cds - 40);
                            break;

                        default:
                            psi = 1.0;
                            break;
                    }
                    cd = psi * cds;
                    break;
                /* Intermediate */
                case < 1e-2:
                    cds = (29 + 4 * Math.Log10(k / (D + mgThickness))) / 20;
                    double psiSmooth = new();
                    switch (kc / cds)
                    {
                        case < 0:
                            // Invalid
                            break;

                        case < 1.15:
                            psiSmooth = 1.75 - 1.3 * kc / 0.65;
                            break;

                        case < 3.1:
                            psiSmooth = 0.25;
                            break;

                        case < 18.5:
                            psiSmooth = 0.25 + 0.065 * (kc / 0.65 - 3.1);
                            break;

                        case < 40:
                            psiSmooth = 1.25 - 0.007 * (kc / 0.65 - 18.5);
                            break;

                        case < 100:
                            psiSmooth = 1.1 - 0.00167 * (kc / 0.65 - 40);
                            break;

                        default:
                            psiSmooth = 1.0;
                            break;
                    }

                    double psiRough = new();

                    switch (kc / cds)
                    {
                        case < 0:
                            // Invalid
                            break;

                        case < 0.7:
                            psiRough = 2 - 2.14 * kc / 1.05;
                            break;

                        case < 1.9:
                            psiRough = 0.5;
                            break;

                        case < 11.4:
                            psiRough = 0.5 + 0.105 * (kc / 1.05 - 1.9);
                            break;

                        case < 20:
                            psiRough = 1.5 - 0.029 * (kc / 1.05 - 11.4);
                            break;

                        case < 40:
                            psiRough = 1.25 - 0.0075 * (kc / 1.05 - 20);
                            break;

                        case < 100:
                            psiRough = 1.1 - 0.00167 * (kc / 1.05 - 40);
                            break;

                        default:
                            psiRough = 1.0;
                            break;
                    }
                    // Interpolation between the psiSmooth and psiRough
                    cd = psiSmooth*0.65 - (psiSmooth*0.65 - psiRough*1.05) * (0.65 - cds) / (0.65 - 1.05);
                    break;
                /* Rough */
                default:
                    cds = 1.05;
                    switch (kc / cds)
                    {
                        case < 0:
                            // Invalid
                            break;

                        case < 0.7:
                            psi = 2 - 2.14 * kc / cds;
                            break;

                        case < 1.9:
                            psi = 0.5;
                            break;

                        case < 11.4:
                            psi = 0.5 + 0.105 * (kc / cds - 1.9);
                            break;

                        case < 20:
                            psi = 1.5 - 0.029 * (kc / cds - 11.4);
                            break;

                        case < 40:
                            psi = 1.25 - 0.0075 * (kc / cds - 20);
                            break;

                        case < 100:
                            psi = 1.1 - 0.00167 * (kc / cds - 40);
                            break;

                        default:
                            psi = 1.0;
                            break;
                    }
                    cd = psi * cds;
                    break;
            }
            return cd;
        }
    }
}
