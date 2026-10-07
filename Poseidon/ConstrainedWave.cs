using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Poseidon.Shared;


namespace Poseidon
{
    public class ConstrainedWave
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="run"></param>
        /// <param name="loadCase"></param>
        public  void ReadMaxElevationFromListFile(string FilePath, Run run, LoadCase loadCase)
        {

            string[] fileLines = File.ReadAllLines(FilePath);
            foreach (string line in fileLines)
            {
                if (line.StartsWith("   1 STREAM FUN"))
                {
                    string[] items = line[15..].Split( " ", StringSplitOptions.RemoveEmptyEntries );

                    loadCase.maxElevation = double.Parse(items[5]);
                }
            }
        }
        /// <summary>
        /// Reads the kinematics from the LIS file and returns the velocity
        /// </summary>
        /// <param name="FilePath">Path to LIS file</param>
        /// <param name="Case"> Zero (0) to return velocity at max acceleration or other to return absolute max velocity </param>
        /// <returns></returns>
        public  double[] ReadCrestKinematicsFromListFile(string FilePath, int Case)
        {

            string fileContent = File.ReadAllText(FilePath);

            int pointsKPRT = Regex.Matches(fileContent, "KPRT").Count - 1;
            
            string[] fileLines = File.ReadAllLines(FilePath);

            int[] kinematicsHeaderIndex = Enumerable.Range(0, fileLines.Length).Where(x => fileLines[x].StartsWith(" NUMBER   XP          YP          ZP          ELEVATION   VX          VY          VZ          AX          AY          AZ")).ToArray();
            double[] phaseTotalVelocities = new double[36];
            double[] phaseAccelerations = new double[36];

            int j = 0;
            for (int i = kinematicsHeaderIndex[0] + 3 + (pointsKPRT-1) * 3; i < fileLines.Length ; i+= pointsKPRT * 3 + 7)
            {
                phaseTotalVelocities[j] = double.Parse(fileLines[i + 2].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]);
                phaseAccelerations[j] = double.Parse(fileLines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[8]);

                if (fileLines[i + 3].StartsWith("1DATE:"))
                { i += 2; }

                j++;
            }

            double[] velocitiesKPRT = new double[pointsKPRT];

            int maxVelocityPhase = Array.IndexOf(phaseTotalVelocities, phaseTotalVelocities.Max());
            int minVelocityPhase = Array.IndexOf(phaseTotalVelocities, phaseTotalVelocities.Min());

            int maxAccelerationPhase = Array.IndexOf(phaseAccelerations, phaseAccelerations.Max());
            int minAccelerationPhase = Array.IndexOf(phaseAccelerations, phaseAccelerations.Min());


            int extractionPhase;
            if (Case != 0)
            {
                if (phaseTotalVelocities[maxVelocityPhase] >= Math.Abs(phaseTotalVelocities[minVelocityPhase]))
                {
                    extractionPhase = maxVelocityPhase;
                }
                else
                {
                    extractionPhase = minVelocityPhase;
                }
            }
            else
            {
                if (Math.Abs(phaseTotalVelocities[maxAccelerationPhase]) >= Math.Abs(phaseTotalVelocities[minAccelerationPhase]))
                {
                    extractionPhase = maxAccelerationPhase;
                }
                else
                {
                    extractionPhase = minAccelerationPhase;
                }
            }


            /* Iterate for each KPRT point to build arrays of kinematics at each KPRT elevation */
            for (int i = 0; i < pointsKPRT; i++)
            {
                velocitiesKPRT[i] = double.Parse(fileLines[kinematicsHeaderIndex[extractionPhase] + 3 + (i * 3 + 2)].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]);
            }


            return velocitiesKPRT;
        }

    }
}
