using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poseidon.Shared
{
    public class Run
    {
        public double Timestep { get; set; }
        public double AnalysisDuration { get; set; }
        public double RoughnessMG { get; set; }
        public double RoughnessPainted { get; set; }
        public double MSL { get; set; }
        public double SplashUpper { get; set; }
        public double SplashLower { get; set; }
        public double InterfaceElevation { get; set; }
        public double HubElevation { get; set; }
        public Location Location { get; set; }
        public List<MarineGrowthElevation> MarineGrowth { get; set; }
        public double[] ECMProfile_1yr { get; set; }

        public double[] ECMProfile_50yr { get; set; }

        public double NCMDepthAveCurrSpeed { get; set; }
        public double ConstrainedWaveTime { get; set; }
        public UpperAnodeBracelet UpperBracelet { get; set; }
        public LowerAnodeBracelet LowerBracelet { get; set; }



        public void Serialize(string filePath)
        {
            string output = System.Text.Json.JsonSerializer.Serialize(this);
            File.WriteAllText(filePath, output);
        }

        public static Run Deserialize(string filePath)
        {
            string content = File.ReadAllText(filePath);
            return System.Text.Json.JsonSerializer.Deserialize<Run>(content);
        }

        public List<CdLayer> GetCdProfileElevations(double maxSurfaceElevation, LoadCase loadCase)
        {
            /* Identify the elevation of the conical transition */
            double coneTop = new(); double coneBot = new();
            /* Variable with distance from SWL */
            double runningElevation = InterfaceElevation - loadCase.waterDepthDiffference;
            bool coneFlag = false;
            foreach (var can in Location.MPGeometry)
            {
                if (coneFlag == false & can.DiaTop != can.DiaBot) { coneTop = runningElevation; coneFlag = true; }
                runningElevation -= can.Length;
                if (coneFlag == true & can.DiaTop != can.DiaBot) { coneBot = runningElevation; }
                if (coneFlag == true & can.DiaTop == can.DiaBot) { break; }
            }

            /* List with the drag profile elevations */
            List<double> cdProfileElevations = new List<double>();

            /* Input the number of layers of the drag profile (max 10) */
            int iTop = 2;
            int iCone = 4;
            int iBot = 4; // minimum 2
            /* Evaluation of the Drag profile elevations wrt SWL */
            cdProfileElevations.Add(maxSurfaceElevation + 0.01);
            if (cdProfileElevations[0] > MarineGrowth[1].Upper - loadCase.waterDepthDiffference)
            {
                cdProfileElevations.Add(MarineGrowth[1].Upper - loadCase.waterDepthDiffference);
                iTop++; iCone--;
                for (int i = cdProfileElevations.Count; i < iTop; i++) { cdProfileElevations.Add(cdProfileElevations[1] + (coneTop - cdProfileElevations[1]) / (iTop-1) * (i-1)); }
            }
            else
            {
                for (int i = cdProfileElevations.Count; i < iTop; i++) { cdProfileElevations.Add(cdProfileElevations[0] + (coneTop - cdProfileElevations[0]) / iTop * i); }
            }
            for (int i = cdProfileElevations.Count; i < iTop + iCone; i++) { cdProfileElevations.Add(coneTop + (coneBot - coneTop) / iCone * (i - iTop)); }
            //for (int i = iTop + iCone; i < iTop + iCone + iBot + 1; i++) { cdProfileElevations[i] = coneBot + (-run.Locations[0].Depth - coneBot) / iBot * (i - (iTop + iCone)); }
            //for (int i = cdProfileElevations.Count; i < iTop + iCone + iBot; i++){ cdProfileElevations.Add(coneBot + (MarineGrowth[0].Upper - loadCase.waterDepthDiffference - coneBot) / (iBot - 1) * (i - (iTop + iCone))); }
            cdProfileElevations.Add(coneBot - loadCase.waterDepthDiffference);
            cdProfileElevations.Add(MarineGrowth[0].Upper - loadCase.waterDepthDiffference);
            cdProfileElevations.Add(this.LowerBracelet.TopElevationMudline - (this.Location.Depth + loadCase.waterDepthDiffference));
            cdProfileElevations.Add(this.LowerBracelet.BottomElevationMudline - (this.Location.Depth + loadCase.waterDepthDiffference));

            cdProfileElevations.Add(-Location.Depth - loadCase.waterDepthDiffference);


            /* Array with the MP diameter at the drag profile layer mid-point */
            double[] cdProfileDiameters = new double[10];
            for (int i = 0; i < cdProfileElevations.Count - 1; i++)
            {
                /* Check cd profile elevations relative to top anode cage and move the cd layer elevations to include layers at the anode cages */
                if (cdProfileElevations[i] > -loadCase.waterDepthDiffference - UpperBracelet.TopElevationLAT &&
                    cdProfileElevations[i + 1] < -loadCase.waterDepthDiffference - UpperBracelet.BottomElevationLAT &&
                    i <= iTop + 1)
                {
                    cdProfileElevations[i + 1] = -loadCase.waterDepthDiffference - UpperBracelet.TopElevationLAT;
                    cdProfileElevations[i + 2] = -loadCase.waterDepthDiffference - UpperBracelet.BottomElevationLAT;
                }
                else if (cdProfileElevations[i] > -loadCase.waterDepthDiffference - UpperBracelet.TopElevationLAT &&
                    cdProfileElevations[i+1] < -loadCase.waterDepthDiffference - UpperBracelet.BottomElevationLAT &&
                    i > iTop + 1)
                {
                    cdProfileElevations[i] = -loadCase.waterDepthDiffference - UpperBracelet.TopElevationLAT;
                    cdProfileElevations[i+1] = -loadCase.waterDepthDiffference - UpperBracelet.BottomElevationLAT;
                }
                else if (cdProfileElevations[i] > -loadCase.waterDepthDiffference - UpperBracelet.TopElevationLAT &&
                         cdProfileElevations[i+1] < -loadCase.waterDepthDiffference - UpperBracelet.TopElevationLAT &&
                         cdProfileElevations[i+1] > -loadCase.waterDepthDiffference -   UpperBracelet.BottomElevationLAT)
                {
                    cdProfileElevations[i+1] = -loadCase.waterDepthDiffference - UpperBracelet.TopElevationLAT;
                    cdProfileElevations[i+2] = -loadCase.waterDepthDiffference - UpperBracelet.BottomElevationLAT;
                }
                else if (cdProfileElevations[i] < -loadCase.waterDepthDiffference - UpperBracelet.TopElevationLAT &&
                         cdProfileElevations[i + 1] > -loadCase.waterDepthDiffference - UpperBracelet.BottomElevationLAT)
                {
                    cdProfileElevations[i] = -loadCase.waterDepthDiffference - UpperBracelet.TopElevationLAT;
                    cdProfileElevations[i + 1] = -loadCase.waterDepthDiffference - UpperBracelet.BottomElevationLAT;
                }

                if (i < iTop) { cdProfileDiameters[i] = Location.MPGeometry[0].DiaTop; }
                else if (i >= iTop & i < iTop + iCone)
                {
                    cdProfileDiameters[i] = Location.MPGeometry[0].DiaTop + (Location.MPGeometry[^1].DiaTop - Location.MPGeometry[0].DiaTop) * (cdProfileElevations[iTop] - (cdProfileElevations[i+1] + cdProfileElevations[i]) / 2) / (coneTop - coneBot);
                }
                else { cdProfileDiameters[i] = Location.MPGeometry[^1].DiaTop; }
            }

            /* From the arrays, populate a list of Cd layers for the location */
            List<CdLayer> cdLayers = new List<CdLayer>();

            for (int i = 0; i < cdProfileDiameters.Length; i++)
            {
                CdLayer cdLayer = new CdLayer();
                cdLayer.Z1 = cdProfileElevations[i];
                cdLayer.Z2 = cdProfileElevations[i+1];
                cdLayer.Dia = cdProfileDiameters[i];

                if (Math.Abs(cdProfileElevations[i] + UpperBracelet.TopElevationLAT + loadCase.waterDepthDiffference) < 1E-6 &&
                    Math.Abs(cdProfileElevations[i+1] + UpperBracelet.BottomElevationLAT + loadCase.waterDepthDiffference) < 1E-6)
                {
                    cdLayer.IsUpperBraceletLayer = true;
                }
                if (Math.Abs(cdProfileElevations[i] + Location.Depth + loadCase.waterDepthDiffference - LowerBracelet.TopElevationMudline) < 1E-6 &&
                    Math.Abs(cdProfileElevations[i + 1] + Location.Depth + loadCase.waterDepthDiffference - LowerBracelet.BottomElevationMudline) < 1E-6) 
                {
                    cdLayer.IsBottomBraceletLayer = true;
                }
                cdLayers.Add(cdLayer);
            }

            //Location.CdProfileLayers = cdLayers;

            return cdLayers;
        }
    }

    public class MarineGrowthElevation
    {
        public double Lower { get; set; }
        public double Upper { get; set; }
        public double Roughness { get; set; }
        public double Density { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public double Depth { get; set; }
        public List<MPSection> MPGeometry { get; set; }
        //public List<CdLayer> CdProfileLayers { get; set; }
        public RayleighDampingData RayleighDamping { get; set; }
        public string FEMFileName { get; set; }
    }

    public class MPSection
    {
        public double Index;
        public double Length;
        public double Top { get; set; }
        public double Btm { get; set; }
        public double DiaTop { get; set; }
        public double DiaBot { get; set; }
        public double Thickness;
    }

    public class CdLayer
    {
        public bool IsBottomBraceletLayer = false;
        public bool IsUpperBraceletLayer = false;

        public double Z1 { get; set; }
        public double Z2 { get; set; }
        public double Dia { get; set; }
        //public double CDN { get; set; }
        //public double CDL { get; set; }
        //public double CMN { get; set; }
    }

    public class RayleighDampingData
    {
        public double FLSStiffnessCoeff { get; set; }
        public double FLSMassCoeff { get; set; }
        public double ULSStiffnessCoeff { get; set; }
        public double ULSMassCoeff { get; set; }
    }
    public class UpperAnodeBracelet
    {
        public double TopElevationLAT { get; set; }
        public double MidElevationLAT { get; set; }
        public double BottomElevationLAT { get; set; }
        public double AdditionalDiameter { get; set; }
        public double Height { get; set; }
        public double MGThickness { get; set; }
        public double MGRoughness { get; set; }
        public double MGDensity { get; set; }
        public double CdFactor { get; set; }

    }
    public class LowerAnodeBracelet
    {
        public double TopElevationMudline { get; set; }
        public double MidElevationMudline { get; set; }
        public double BottomElevationMudline { get; set; }
        public double AdditionalDiameter { get; set; }
        public double Height { get; set; }
        public double MGThickness { get; set; }
        public double MGRoughness { get; set; }
        public double MGDensity { get; set; }
        public double CdFactor { get; set; }

    }
}
