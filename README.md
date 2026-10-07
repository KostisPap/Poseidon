# Poseidon

A C# (.NET 6) library of calculations for hydrodynamic load analysis of offshore monopile foundations: wave components and constrained waves, current profiles, drag and inertia coefficients, and the run/load-case model that ties them together.

> **Status:** portfolio / reference code. The public tree contains the calculation library only. The solver-integration layer, the console application that drives it, the integration tests and all project data are intentionally **not included** (see [Not included](#not-included)), so this repository builds as a library but is not a runnable application.

## What it does

- **Monopile Foundation Superelement**: produces Wind Turbine Generator (WTG) monopile foundation superelements for Integrated Load Analysis (ILA).
- **Wave and current modelling**: wave components, constrained-wave handling, and current profiles from ECM, NCM and NOC current models.
- **Hydrodynamic coefficients**: drag (Cd) and inertia (Cm) coefficients over elevation, including marine-growth roughness and anode bracelets (`DragCoefficient`, `InertiaCoefficient`).
- **Run model**: `Run`, `Location`, `MPSection`, `LoadCase` and related types describing an analysis (time step, duration, geometry, marine growth, bracelets, load cases).
- **Excel readers**: monopile geometry and location water depths (`ReadExcel`).
- **Result readers**: parsing of wave-component and kinematics output files (`ReadComponentFile`, `ConstrainedWave`, `WaveComponent`).
- **Validation helpers**: `ResultsValidation` holds helper routines for checking drag-coefficient results.

## Solution layout

| Project | Purpose |
|---|---|
| `Poseidon` | Class library: waves, coefficients, current profile, Excel and result readers |
| `Poseidon.Shared` | Shared domain model: `Run`, `LoadCase`, current models, validation helpers |

## Building

```text
dotnet build Poseidon.sln
```

`ReadExcel` looks for workbooks under `POSEIDON_ROOT` (default: the current directory):

```text
<POSEIDON_ROOT>/
  data/
    geometry/*Geometry*.xlsx        monopile geometry
    geometry/*Water Depth*.xlsx     location water depths
```

It expects specific sheet names (for example `MP_Geometry`, `LocationWaterDepths`), so adjust them to match your own workbooks.

## Not included

These files are part of the full project but are kept out of the public repository, either because they depend on licensed third-party software or because they hold project-specific settings.

| Path | What it is |
|---|---|
| `Poseidon/Program.cs` | Console entry point: run settings and the end-to-end analysis pipeline |
| `Poseidon/DragCoefProfileCreation.cs` | Drag-coefficient profile generation that re-runs the solver at each layer elevation |
| `Poseidon.Fem/Poseidon.Fem.csproj` | Project file for the solver-integration layer |
| `Poseidon.Fem/SolverInputFiles.cs` | Generates solver input files and batch commands from the load-case list |
| `Poseidon.Fem/ApplicationManager.cs` | Locates installed solver modules and runs them as processes |
| `Poseidon.Fem/SolverRunners.cs` | Concrete solver-module wrappers |
| `Poseidon.Fem/ApplicationStatuses.cs` | Status values for solver discovery |
| `Poseidon.Fem/ExternalTools.cs` | Reads solver install locations from environment variables |
| `Poseidon.Fem/FemFiles.cs` | Minimal FEM model templates used to derive wave kinematics |
| `Poseidon.Fem/LoadCaseReader.cs` | Reads the load-case table (Excel) into `LoadCase` objects |
| `Poseidon.Fem/Resources/SwimFile.xml`, `Resource.resx`, `Resource.Designer.cs` | Setup template embedded for the wind-simulation manager |
| `Poseidon.Tests/*` | MSTest integration tests that need the licensed solver and solver input files |

Also not distributed:

- **Licensed solver software and its assemblies.** The integration layer is built against a vendor assembly and drives a commercial FEM / wind-simulation suite that must be installed under your own licence.
- **Input data.** Load-case tables, geometry and water-depth workbooks, and solver input/output files.

If you are interested in the excluded files, contact the repository owner.

## Tech

C# 10 / .NET 6, [MathNet.Numerics](https://numerics.mathdotnet.com/), [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader).
