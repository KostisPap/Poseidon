using System.Collections.Generic;
using MathNet.Numerics;


namespace Poseidon.Shared;

public class EcmCurrentModel : CurrentCalculator
{
    public EcmCurrentModel(Run run, LoadCase loadCase)
        : base(run, loadCase)
    {
    }

    public override double ComputeAt(double negativeElevationDownFromSwl) => Interpolate.Linear((IEnumerable<double>)new double[11]
    {
        1.0,
        0.9,
        0.8,
        0.7,
        0.6,
        0.5,
        0.4,
        0.3,
        0.2,
        0.1,
        0.0
    }, (IEnumerable<double>)new double[11]
    {
        1.3,
        1.3,
        1.2,
        1.1,
        1.1,
        1.0,
        1.0,
        1.0,
        0.9,
        0.8,
        0.7
    }).Interpolate(1.0 - -negativeElevationDownFromSwl / this.LAT);

    public override string CurrentModel { get; protected set; } = "ECM";
}