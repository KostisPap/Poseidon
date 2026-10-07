
namespace Poseidon.Shared;

public class NocCurrentModel : CurrentCalculator
{
    public NocCurrentModel(Run run, LoadCase loadCase)
        : base(run, loadCase)
    {
    }

    public override double ComputeAt(double negativeElevationDownFromSwl) => 0.0;

    public override string CurrentModel { get; protected set; } = "NOC";
}