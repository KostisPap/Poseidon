using System;
using System.Runtime.CompilerServices;
using Poseidon.Shared;

namespace Poseidon.Shared;

public abstract class CurrentCalculator
{
    protected readonly Run Run;
    protected readonly LoadCase LoadCase;

    public double LAT => this.Run.Location.Depth;

    public double SWL => this.LAT + this.LoadCase.waterDepthDiffference;

    protected CurrentCalculator(Run run, LoadCase loadCase)
    {
        this.Run = run;
        this.LoadCase = loadCase;
    }

    public abstract double ComputeAt(double negativeElevationDownFromSwl);

    public double ComputeAtFromMudline(double elevationFromMudline) => this.ComputeAt(elevationFromMudline - this.SWL);

    public abstract string CurrentModel { get; protected set; }

    public string GetSolverString()
    {

        if (this.GetType() == typeof(NocCurrentModel))
            return "C   " + this.CurrentModel + " No Current";

        double h0 = 0.0;
        double h1 = this.SWL * 1.0 / 11.0;
        double h2 = this.SWL * 2.0 / 11.0;
        double h3 = this.SWL * 3.0 / 11.0;
        double h4 = this.SWL * 4.0 / 11.0;
        double h5 = this.SWL * 5.0 / 11.0;
        double h6 = this.SWL * 6.0 / 11.0;
        double h7 = this.SWL * 7.0 / 11.0;
        double h8 = this.SWL * 8.0 / 11.0;
        double h9 = this.SWL * 9.0 / 11.0;
        double h10 = this.SWL * 10.0 / 11.0;
        double h11 = this.SWL * 11.0 / 11.0;
        double at0 = this.ComputeAt(h0 - this.SWL);
        double at1 = this.ComputeAt(h1 - this.SWL);
        double at2 = this.ComputeAt(h2 - this.SWL);
        double at3 = this.ComputeAt(h3 - this.SWL);
        double at4 = this.ComputeAt(h4 - this.SWL);
        double at5 = this.ComputeAt(h5 - this.SWL);
        double at6 = this.ComputeAt(h6 - this.SWL);
        double at7 = this.ComputeAt(h7 - this.SWL);
        double at8 = this.ComputeAt(h8 - this.SWL);
        double at9 = this.ComputeAt(h9 - this.SWL);
        double at10 = this.ComputeAt(h10 - this.SWL);
        double at11 = this.ComputeAt(h11 - this.SWL);

        string ret = $"C   0    |    1    |    2    |    3    |    4    |    5    |    6    |    7    |    8    |" + Environment.NewLine +
                     $"C   56789012345678901234567890123456789012345678901234567890123456789012345678901234567890" + Environment.NewLine +
                     $"C   {this.CurrentModel} Current Model" + Environment.NewLine +
                     $"CRNT   1.                                {h0,-9:N3} {at0,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h1,-9:N3} {at1,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h2,-9:N3} {at2,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h3,-9:N3} {at3,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h4,-9:N3} {at4,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h5,-9:N3} {at5,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h6,-9:N3} {at6,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h7,-9:N3} {at7,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h8,-9:N3} {at8,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h9,-9:N3} {at9,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h10,-9:N3} {at10,-9:N3}" + Environment.NewLine +
                     $"CRNT   1.                                {h11,-9:N3} {at11,-9:N3}";

        return ret;

    }
}