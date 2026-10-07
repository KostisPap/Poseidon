using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Poseidon.Shared;

namespace Poseidon
{
    /* Class that represents single wave components */
    public class WaveComponent
    {
        /* The attributes of the wave component object */
        public readonly double Amplitude, Omega, Phase, WaveNo, SpecDensity;

        public readonly double Depth, Kd, SinhKd, OmegaAmplitude;
        /* Method that produces the wave component properties from solver .lis file line */
        public WaveComponent(string line, Run run, LoadCase loadCase)
        {
            /* Input line from solver .lis file is split */
            string[] cells = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            
            /* First [0] item is the index and is truncated */
            Amplitude = Convert.ToDouble(cells[1]);
            Omega = Convert.ToDouble(cells[2]);
            Phase = Convert.ToDouble(cells[3]);
            WaveNo = Convert.ToDouble(cells[4]);
            SpecDensity = Convert.ToDouble(cells[5]);
            Depth = run.Location.Depth + loadCase.waterDepthDiffference;
            Kd = WaveNo * Depth;
            SinhKd = Math.Sinh(Kd);
            OmegaAmplitude = Omega * Amplitude;
        }

        /* Method that calculates the surface elevation of a single component at a time and location */
        /* TODO: calculation only works at coordinate (0,0), implement for any point in horizontal plane */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetSurfaceElevationNoStretch(double t, double X = 0)
        {
            var t_var = GetPhase(t, X);
            double eta = Amplitude * Math.Cos(t_var);

            return eta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetPhase(double t, double X)
        {
            double t_var = -Omega * t + WaveNo * X + Phase;
            return t_var;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Method that calculates the horizontal velocity of a single component at a time and location. calculation only works at coordinate (0,0), implement for any point in horizontal plane.
        /// </summary>
        /// <param name="t">Time in seconds.</param>
        /// <param name="eta">Surface elevation with respect to SWL.</param>
        /// <param name="z">Elevation (z <= 0) of query coordinate with respect to SWL.</param>
        /// <param name="X">The X coordinate.</param>
        /// <returns></returns>
        public double GetHorizontalKinematicsAt(double t, double eta, double z = 0, double X = 0)
        {
            /* Cosine term variable */
            var t_var = GetPhase(t, X);
            /* Hyperbolic Cosine term variable I */
            double zPrime = Depth * (Depth + z) / (Depth + eta) - Depth;
            /* Hyperbolic Cosine term variable II */
            double zPrimeD = Math.Min(zPrime, z) + Depth;
            ///* Hyperbolic Sine term variable */
            //double kd = WaveNo * depth;

            /* Horizontal particle velocity */
            double u = OmegaAmplitude * Math.Cosh(WaveNo * zPrimeD) * Math.Cos(t_var) / SinhKd;

            /* For large WaveNo numbers, kd becomes large and Sinh term is non computable */
            /* the following checks that the component velocity is a valid number */
            if (Double.IsNaN(u)) { return 0; }
            else { return u; }
        }
    }

}
