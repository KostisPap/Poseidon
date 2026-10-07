using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poseidon.Shared
{
    public enum SpecType
    {
        JONSWAP,
        OCHIHUBBLE
    }

    public enum AnalysisType
    {
        ULS,
        FLS
    }
    public class LoadCase
    {
        public int seaStateFileNumber;
        public bool HasCwave => (hmax != 0 && tass != 0);
        public string lcNo { get; set; }
        public string dlc { get; set; }
        public string interfaceFileName { get; set; }
        //public string loadCaseDescription { get; set; }
        public AnalysisType analysisType { get; set; } //ULS or FLS
        //public string loadCaseFamily { get; set; }
        //public string loadCaseMethod { get; set; }
        //public string loadFactor { get; set; }
        //public string windModel { get; set; }
        public double windSpeed { get; set; }
        //public string turbulenceIntensity { get; set; }
        public double windShear { get; set; }
        //public string windSeed { get; set; }

        public double windDirection { get; set; }
        //public string nacelleDirection { get; set; }
        //public string airDensity { get; set; }
        //public string externalCondition { get; set; }
        //public string azimuthAngle { get; set; }
        //public string powerDWTOccur { get; set; }

        public double yawError { get; set; }
        //public string azimuthAngle { get; set; }
        //public string windModel { get; set; }

        public string superFile { get; set; }
        public double hs { get; set; }
        public double tp { get; set; }
        public double hmax { get; set; }
        public double tass { get; set; }
        public double waveSeed { get; set; }
        public double waveDirection { get; set; }

        //public double hsSwell { get; set; }
        //public double tpSwell { get; set; }
        //public double swellSeed { get; set; }
        //public double swellDirection { get; set; }

        public string currentModel { get; set; }
        //public double depthAveCurrSpeed { get; set; }

        public double timeSeriesInitialisation { get; set; }
        public double timeSeriesLengthAfterInitialisation { get; set; }

        //public double hoursDuringLifetime { get; set; }
        public double occurencesDuringLifetime { get; set; }

        public SpecType Spectrum => hs1 == 0 && hs2 == 0 ? SpecType.JONSWAP : SpecType.OCHIHUBBLE;
        public double hs1 { get; set; }
        public double tp1 { get; set; }
        public double lambda1 { get; set; }
        public double hs2 { get; set; }
        public double tp2 { get; set; }
        public double lambda2 { get; set; }
        public double sigA { get; set; }
        public double sigB { get; set; }
        public double gamma { get; set; }
        public double prob { get; set; }
        public double tz { get; set; }
        public string seaStateDescription { get; set; }
        public double waterDepthDiffference { get; set; }
        public double maxElevation { get; set; }


    }
}
