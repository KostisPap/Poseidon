using Poseidon.Shared;

namespace Poseidon
{
    /* Class that reads the wave component .lis file returns the timehistory of horizontal velocities */
    public class ReadComponentFile
    {
        /* Property of the wave component file */
        public readonly double[] SurfaceElevations;
        public double[] HorizontalVelocities;
        public double Timestep = 0.2;
        //GetHorizontalKinematicsAt(double t, double depth, double eta, double z = 0, double X = 0)

        /* Global variables */

        /* Create time axis array for the analysis */
        private double[] time;

        /* WaveComponents array with the wave components found in the .lis file */
        WaveComponent[] waveComponents;

        /* Constructor that receives path to .lis file populates the elevation timehistory */
        public ReadComponentFile(string filePath, Run run,  LoadCase loadCase)
        {
            /* Read .lis file line by line excluding header first line */
            string[] content = File.ReadAllLines(filePath)[1..];
            /* WaveComponents array with the wave components found in the .lis file */
            waveComponents = content.Select(x => new WaveComponent(x, run, loadCase)).Where(h => h.WaveNo < 15).ToArray();


            /* Create time axis array for the analysis */
            int nt = (int)((loadCase.timeSeriesInitialisation + loadCase.timeSeriesLengthAfterInitialisation) / Timestep);
            
            time = Enumerable.Range(0, nt).Select(x => x * Timestep).ToArray();

            /* Array with the surface elevation timehistory created by the */
            /* summation of the elevations of each wave component found in waveComponents array */
            double[] surfaceElevations = time.Select(t => waveComponents.Sum(x => x.GetSurfaceElevationNoStretch(t))).ToArray();

            /* The internal arrays content are passed on to the object properties */
            SurfaceElevations = surfaceElevations;
        }

        /* Method that evaluates the kinematics at elevation z below the still water elevation */
        public void ComputeKinematics(double locationDepth, double z, Run run, LoadCase loadCase)
        {
            /* Creation of the array to store the horizontal velocity time histories */
            double[] horizontalVelocities = new double[time.Length];

            if (z > 0) {  z = 0; }

            
            int nt = time.Length;
            int start = (int)(loadCase.timeSeriesInitialisation / Timestep);
            /* Iterate for every time step. (int)(200.0 / run.Timestep) to truncate the first 200 seconds of the analysis */
            for (int i = start; i < nt; i++)
            {
                double t = time[i];
                double surface = SurfaceElevations[i];
                //int counter = 0;

                /* Iterate for every wave component */
                foreach (WaveComponent wave in waveComponents)
                {
                    //counter++;
                    //if (counter == 1845)
                    //{
                    //    Console.WriteLine("breakpoint reached");
                    //}

                    /* Summation of the velocities from each component to get total velocity */
                    /* Input: water depth and plane coordinates of velocity evaluation (default to zero) */
                    horizontalVelocities[i] += wave.GetHorizontalKinematicsAt(t, surface, z);
                }
            }
            HorizontalVelocities = horizontalVelocities;
        }
    }
}
