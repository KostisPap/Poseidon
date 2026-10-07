using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poseidon.Shared;

namespace Poseidon
{
    ///// <summary>
    ///// Class for current profile creation
    ///// </summary>
    //public class CurrentProfile
    //{

    //    /// <summary>
    //    /// Calculates the velocity at various elevations based on surface velocity
    //    /// </summary>
    //    /// <param name="elevation">Elevation relative to mudline and positive upwards</param>
    //    /// <param name="run"></param>
    //    /// <param name="loadCase"></param>
    //    /// <returns>Returns the current profile velocity at a given elevation based on the depth averaged current speed</returns>
    //    public  double VelocityAtElevationFromDepthAveragedSpeed(double elevation, Run run, LoadCase loadCase)
    //    {
    //        double windCurrentMaxDepth = run.Location.Depth + loadCase.waterDepthDiffference - 20;
    //        double Um_0 = 0.01 * loadCase.windSpeed * Math.Pow((10 + loadCase.waterDepthDiffference - run.MSL) / (run.HubElevation), loadCase.windShear); // IEC Eq. (5)
    //        double Um_0_factor;
    //        if ( windCurrentMaxDepth < elevation && elevation < run.Location.Depth + loadCase.waterDepthDiffference)
    //        { Um_0_factor = (1 + (elevation - (run.Location.Depth + loadCase.waterDepthDiffference)) / 20); }
    //        else { Um_0_factor = 0; }

    //        double percentWaterColumn = elevation / (run.Location.Depth + loadCase.waterDepthDiffference);
    //        int waterColumnIndex = (int) (percentWaterColumn * 10.0);

    //        double currentProfileVelocity = 0;
    //        if ( loadCase.currentModel == "NCM")
    //        { currentProfileVelocity = Um_0 * Um_0_factor +
    //                   8.0 / 7 * run.NCMDepthAveCurrSpeed * Math.Pow(elevation / (run.Location.Depth + loadCase.waterDepthDiffference),
    //                                                                 1.0 / 7.0);
    //        }
    //        else if (loadCase.currentModel == "ECM")
    //        {
    //            if (loadCase.dlc == "DLC61" || loadCase.dlc == "DLC35")
    //            {
    //                currentProfileVelocity = waterColumnIndex switch
    //                {
    //                    >= 10 => run.ECMProfile_50yr[^1],
    //                    _ => run.ECMProfile_50yr[waterColumnIndex] +
    //                    (run.ECMProfile_50yr[waterColumnIndex + 1] - run.ECMProfile_50yr[waterColumnIndex]) * (percentWaterColumn * 10.0 - waterColumnIndex)
    //                };
    //            }
    //            else if (loadCase.dlc == "DLC63" || loadCase.dlc == "DLC71" || loadCase.dlc == "DLC82")
    //            {
    //                currentProfileVelocity = waterColumnIndex switch
    //                {
    //                    >= 10 => run.ECMProfile_1yr[^1],
    //                    _ => run.ECMProfile_1yr[waterColumnIndex] +
    //                    (run.ECMProfile_1yr[waterColumnIndex + 1] - run.ECMProfile_1yr[waterColumnIndex]) * (percentWaterColumn * 10.0 - waterColumnIndex)
    //                };
    //            }
    //        }

    //        return currentProfileVelocity;
    //    }
    //}
}
