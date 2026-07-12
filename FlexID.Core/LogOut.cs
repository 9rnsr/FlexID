using System.Text;

namespace FlexID;

internal class LogOut : IDisposable
{
    private readonly InputData data;
    private readonly TextWriter writer;

    public static LogOut Create(InputData data, string filePath)
    {
        var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        var writer = new StreamWriter(stream, Encoding.UTF8);
        return new LogOut(data, writer);
    }

    public LogOut(InputData data, TextWriter writer)
    {
        this.data = data;
        this.writer = writer;
    }

    public void Dispose() => writer.Dispose();

    public void WriteOutNuclides()
    {
        var formatted = data.Nuclides
            .Select(n => (Nuclide: n.Name, Lambda: n.Lambda.ToString("E"), HalfLife: n.HalfLife ?? "",
                          Branches: n.Branches.Select(d => $"{d.Daughter.Name}/{d.Fraction}").ToArray())).ToArray();

        const string HeaderN = "Nuclide";
        const string HeaderL = "Lambda";
        const string HeaderH = "HalfLife";

        var widthN = formatted.Max(n => n.Nuclide.Length);
        var widthL = formatted.Max(n => n.Lambda.Length);
        var widthHL = formatted.Max(n => n.HalfLife.Length);
        var widthBr = formatted.SelectMany(n => n.Branches).Append("").Max(r => r.Length);

        widthN = Math.Max(widthN, HeaderN.Length);
        widthL = Math.Max(widthL, HeaderL.Length);
        if (widthHL > 0)
            widthHL = Math.Max(widthHL, HeaderH.Length);
        var branchColumn = widthBr != 0;

        writer.WriteLine();

        int remainPadding = 0;
        void WriteStr(string value, int width = 0)
        {
            while (remainPadding-- != 0)
                writer.Write(' ');

            if (width > 0) // PadLeft
                for (int w = width - value.Length; w != 0; w--) writer.Write(' ');

            writer.Write(value);

            if (width < 0) // PadRight
                remainPadding = -width - value.Length;
            else
                remainPadding = 0;
        }
        void WriteLine()
        {
            writer.WriteLine();
            remainPadding = 0;
        }

        WriteStr(HeaderN, -widthN);
        WriteStr("  ");
        WriteStr(HeaderL, -widthL);
        if (widthHL != 0) { WriteStr("  "); WriteStr(HeaderH, widthHL); }
        if (widthBr != 0) { WriteStr("  Branches"); }
        WriteLine();

        foreach (var nuclide in formatted)
        {
            WriteStr(nuclide.Nuclide, -widthN);
            WriteStr("  ");
            WriteStr(nuclide.Lambda, +widthL);

            if (widthHL != 0)
            {
                WriteStr("  ");
                WriteStr(nuclide.HalfLife, widthHL);
            }
            if (nuclide.Branches.Any())
            {
                WriteStr(" ");
                foreach (var branch in nuclide.Branches)
                {
                    WriteStr(" ");
                    WriteStr(branch, -widthBr);
                }
            }

            WriteLine();
        }

        writer.Flush();
    }

    public void WriteOutCompartments()
    {
        if (!data.PrintCompartments)
            return;

        writer.WriteLine();
        writer.WriteLine("Compartments:");

        var nuclideLength = data.Nuclides.Max(o => o.Name.Length);
        var nameLength = data.Organs.Max(o => o.Name.Length);

        var spacing = new char[Math.Max(nuclideLength, nameLength)];
        spacing.AsSpan().Fill(' ');

        NuclideData? prevNuclide = null;
        foreach (var nuclide in data.Nuclides)
        {
            var organs = data.Organs.Where(o => o.Nuclide == nuclide);

            if (prevNuclide != null)
                writer.WriteLine();
            prevNuclide = nuclide;

            foreach (var organ in organs)
            {
                writer.Write("  ");
                writer.Write(organ.Func.ToString());
                if (organ.IsZeroInflow)
                    writer.Write(" Z ");
                else
                    writer.Write("   ");

                writer.Write(nuclide.Name);
                writer.Write(spacing, 0, nuclideLength - nuclide.Name.Length);
                writer.Write('/');
                writer.Write(organ.Name);

                if (organ.SourceRegion is not null)
                {
                    writer.Write(spacing, 0, nameLength - organ.Name.Length);
                    writer.Write("   ");
                    writer.Write(organ.SourceRegion);
                }

                writer.WriteLine();
            }
        }

        writer.Flush();
    }

    public void WriteOutTransfers()
    {
        if (!data.PrintTransfers)
            return;

        writer.WriteLine();
        writer.WriteLine("Transfers:");

        var transfers = new List<(int nuclideIndex, string from, string to, string? coeff)>();
        var fromLength = 0;
        var toLength = 0;
        var coeffAlignment = 0;
        foreach (var nuclide in data.Nuclides)
        {
            void Add(Organ organFrom, Organ organTo, string? coeff)
            {
                var from = $"{organFrom.Nuclide.Name}/{organFrom.Name}";
                var to = $"{organTo.Nuclide.Name}/{organTo.Name}";

                transfers.Add((nuclide.Index, from, to, coeff));

                fromLength = Math.Max(fromLength, from.Length + 2);
                toLength = Math.Max(toLength, to.Length + 2);
                if (coeff != null)
                    coeffAlignment = Math.Max(coeffAlignment, to.Length + 2 + coeff.IndexOf('='));
            }

            var organs = data.Organs.Where(o => o.Nuclide == nuclide);
            foreach (var organTo in organs)
            {
                foreach (var inflow in organTo.Inflows.Where(i => i.Organ.Nuclide != organTo.Nuclide))
                {
                    var organFrom = inflow.Organ;
                    Add(organFrom, organTo, null);
                }
            }
            foreach (var organTo in organs)
            {
                foreach (var inflow in organTo.Inflows.Where(i => i.Organ.Nuclide == organTo.Nuclide))
                {
                    var organFrom = inflow.Organ;

                    var rate = inflow.Rate;
                    var coeff = organFrom.IsInstantOutflow
                        ? $"= {rate * 100:G}%" : $"= {rate:G}";

                    Add(organFrom, organTo, coeff);
                }
            }
        }

        var spacing = new char[Math.Max(fromLength, Math.Max(toLength, coeffAlignment))];
        spacing.AsSpan().Fill(' ');

        var prevNuclideIndex = -1;
        foreach (var (nuclideIndex, from, to, coeff) in transfers)
        {
            if (nuclideIndex != prevNuclideIndex)
            {
                if (prevNuclideIndex != -1)
                    writer.WriteLine();
                prevNuclideIndex = nuclideIndex;
            }

            writer.Write("  ");
            writer.Write(from);
            writer.Write(spacing, 0, fromLength - from.Length);

            writer.Write("-> ");
            writer.Write(to);

            if (coeff is null)
            {
                writer.Write(spacing, 0, toLength - to.Length);
                writer.WriteLine("---");
            }
            else
            {
                var coeffLeftLength = coeff.IndexOf('=');
                var spacingLength = coeffAlignment - (to.Length + coeffLeftLength);
                writer.Write(spacing, 0, spacingLength);
                writer.WriteLine(coeff);
            }
        }

        writer.Flush();
    }

    public void WriteOutScoefficients()
    {
        var printScoeff = data.PrintScoefficients;

        var sourceRegions = data.SourceRegions.Select(s => s.Name).ToArray();
        var otherSourceRegion = "Other";

        foreach (var nuclide in data.Nuclides)
        {
            writer.WriteLine();
            writer.WriteLine($"Nuclide: {nuclide.Name}");

            if (nuclide.IsStable)
            {
                writer.WriteLine($"No S-coefficients for Stable nuclide.");
                continue;
            }

            var indexN = data.Nuclides.IndexOf(nuclide);
            var scoeffTableM = data.SCoeffTablesM[indexN];
            var scoeffTableF = data.SCoeffTablesF[indexN];

            if (printScoeff)
            {
                writer.WriteLine();
                writer.WriteLine($"S-Coefficients (Adult Male):");
                foreach (var line in CalcScoeff.GenerateScoeffFileContent(scoeffTableM, sourceRegions, data.TargetRegions))
                    writer.WriteLine(line);

                writer.WriteLine();
                writer.WriteLine($"S-Coefficients (Adult Female):");
                foreach (var line in CalcScoeff.GenerateScoeffFileContent(scoeffTableF, sourceRegions, data.TargetRegions))
                    writer.WriteLine(line);
            }

            writer.WriteLine();
            writer.WriteLine($"Source regions those are part of '{otherSourceRegion}':");
            writer.WriteLine(string.Join(",", nuclide.OtherSourceRegions));
            writer.WriteLine();
            writer.WriteLine($"S-Coefficient values from '{otherSourceRegion}' to each target regions:");
            writer.WriteLine($"{"  T/S",-10} {otherSourceRegion + "(Male)",-14} {otherSourceRegion + "(Female)",-14}");

            var scoeffOtherM = scoeffTableM[otherSourceRegion];
            var scoeffOtherF = scoeffTableF[otherSourceRegion];

            foreach (var targetRegion in data.TargetRegions)
            {
                var indexT = Array.IndexOf(data.TargetRegions, targetRegion);
                var scoeffM = scoeffOtherM[indexT];
                var scoeffF = scoeffOtherF[indexT];

                writer.WriteLine($"{targetRegion,-10} {scoeffM:0.00000000E+00} {scoeffF:0.00000000E+00}");
            }
        }

        writer.Flush();
    }
}
