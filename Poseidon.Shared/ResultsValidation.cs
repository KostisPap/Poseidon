using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Poseidon.Shared
{
    public class ResultsValidation
    {
        public  double Timestep = 0.2;
        //public double[] Time = Enumerable.Range(0,4000).Select(i => (double)i * Timestep).ToArray();
        //public double[] Diameters = Enumerable.Range(100, 200).Select(i => (double)i * Time
        //
        //
        //
        //).ToArray();
        //public double[] Velocity = Enumerable.Range(10, 1000).Select(i => (double)i * Timestep).ToArray();
        //public double[] Period = Enumerable.Range(10, 1000).Select(i => (double)i * Timestep).ToArray();
        //public double[] Elevation = Enumerable.Range(10, 1000).Select(i => (double)i * Timestep).ToArray();


        //public double[] CheckCm(Run run)
        //{
        //    double[] cmArray = Enumerable.Range(0,100).Select(i => InertiaCoefficient.CalculateCm(Diameters[i], Velocity[i], Period[i], Elevation[i], run)).ToArray();
        //    return cmArray;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="CdLayers"></param>
        /// <param name="MaxEta"></param>
        /// <param name="Umaxs"></param>
        /// <param name="Tz"></param>
        /// <param name="Elevations"></param>
        /// <param name="Diameters"></param>
        /// <param name="Deltas"></param>
        /// <param name="Flow"></param>
        /// <param name="Cds"></param>
        /// <param name="Cms"></param>
        /// <param name="Velocities"></param>
        /// <param name="StDevVelo"></param>
        /// <param name="CurrVelo"></param>
        /// <param name="run"></param>
        /// <param name="loadCase"></param>
        /// <param name="cdCoeffs2"></param>
        public void ValidationFileIrregular(string FilePath, List<CdLayer> CdLayers, double MaxEta, double[] Umaxs,
            double[] Tz, double[] Elevations, double[] Diameters, double[] Deltas, string[] Flow, double[] Cds,
            double[] Cms, double[][] Velocities, double[] StDevVelo, double[] CurrVelo, Run run, LoadCase loadCase,
            double[] cdCoeffs2)
        {
            string content = $"     Max elevation = {MaxEta,10:N3} m\n" +
                "     Index       Btm       Top       Mid       Dia     Delta      Flow       RMS SigniVelo  CurrVelo        Tz        Cd        Cm        Cd(StDev*4)\n" +
                string.Join(Environment.NewLine, Enumerable.Range(0, 10).Select(x => $"{x,10:00}{CdLayers[x].Z2 + run.Location.Depth + loadCase.waterDepthDiffference,10:N4}{CdLayers[x].Z1 + run.Location.Depth + loadCase.waterDepthDiffference,10:N4}{Elevations[x]+run.Location.Depth + loadCase.waterDepthDiffference,10:N4}{Diameters[x],10:N4}{Deltas[x],10:E2}{Flow[x]}{Umaxs[x],10:N4}{StDevVelo[x],10:N4}{CurrVelo[x],10:N4}{Tz[x],10:N4}{Cds[x],10:N7}{Cms[x],10:N7}{cdCoeffs2[x],10:N7}")) + "\n\n\n" +
                "****************************************************** Velocity TimeHistories *****************************************************\n" +
                "        Time     Layer00     Layer01     Layer02     Layer03     Layer04     Layer05     Layer06     Layer07     Layer08     Layer09\n" +
                string.Join(Environment.NewLine, Enumerable.Range(0, Velocities[0].Length).Select(x => $"{x*Timestep,12:N2}{Velocities[0][x],12:N5}{Velocities[1][x],12:N5}{Velocities[2][x],12:N5}{Velocities[3][x],12:N5}{Velocities[4][x],12:N5}{Velocities[5][x],12:N5}{Velocities[6][x],12:N5}{Velocities[7][x],12:N5}{Velocities[8][x],12:N5}{Velocities[9][x],12:N5}"));

            File.WriteAllText(FilePath.Replace(".INP", "_QA.dat"), content);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="CdLayers"></param>
        /// <param name="MaxEta"></param>
        /// <param name="Umaxs"></param>
        /// <param name="Tz"></param>
        /// <param name="Elevations"></param>
        /// <param name="Diameters"></param>
        /// <param name="Deltas"></param>
        /// <param name="Flow"></param>
        /// <param name="Cds"></param>
        /// <param name="Cms"></param>
        /// <param name="Velocities"></param>
        /// <param name="run"></param>
        /// <param name="loadCase"></param>
        public void ValidationFileConstrained(string FilePath, List<CdLayer> CdLayers, double MaxEta, double[] Umaxs, double[] CurrVelo, double[] Tz, double[] Elevations, double[] Diameters, double[] Deltas, string[] Flow, double[] Cds, double[] Cms, Run run, LoadCase loadCase)
        {
            string content = $"     Max elevation = {MaxEta,10:N3} m\n" +
                "     Index       Btm       Top       Mid       Dia     Delta      Flow  Velocity  CurrVelo        Tz        Cd        Cm\n" +
                string.Join(Environment.NewLine, Enumerable.Range(0, 10).Select(x => $"{x,10:00}{CdLayers[x].Z2 + run.Location.Depth + loadCase.waterDepthDiffference,10:N4}{CdLayers[x].Z1 + run.Location.Depth + loadCase.waterDepthDiffference,10:N4}{Elevations[x] + run.Location.Depth + loadCase.waterDepthDiffference,10:N4}{Diameters[x],10:N4}{Deltas[x],10:E2}{Flow[x]}{Umaxs[x],10:N4}{CurrVelo[x],10:N4}{Tz[x],10:N4}{Cds[x],10:N7}{Cms[x],10:N7}"));

            File.WriteAllText(FilePath.Replace(".INP", "_QA.dat"), content);

        }

    }
}
