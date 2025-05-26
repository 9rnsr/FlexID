namespace FlexID.Calc;

using System.Text.RegularExpressions;

public class Program
{
    public class CommandLine
    {
        public string Output;
        public string Input;
        public string CalcTimeMesh;
        public string OutTimeMesh;
        public string CommitmentPeriod;

        public static bool operator ==(CommandLine c1, CommandLine c2)
        {
            if (object.ReferenceEquals(c1, null) && object.ReferenceEquals(c2, null))
                return true;
            if (object.ReferenceEquals(c1, null) || object.ReferenceEquals(c1, null))
                return false;
            return
                c1.Output == c2.Output &&
                c1.Input == c2.Input &&
                c1.CalcTimeMesh == c2.CalcTimeMesh &&
                c1.OutTimeMesh == c2.OutTimeMesh &&
                c1.CommitmentPeriod == c2.CommitmentPeriod;
        }

        public static bool operator !=(CommandLine c1, CommandLine c2)
        {
            return !(c1 == c2);
        }

        public override bool Equals(object obj)
        {
            var c = obj as CommandLine;
            if (!object.ReferenceEquals(c, null))
                return this == c;
            return false;
        }

        public override int GetHashCode()
        {
            // memo: メッセージを消すためだけの適当な実装、性能は考慮してない
            return
                Output.GetHashCode() +
                Input.GetHashCode() +
                CalcTimeMesh.GetHashCode() +
                OutTimeMesh.GetHashCode() +
                CommitmentPeriod.GetHashCode();
        }
    }

    public static int Main(string[] args)
    {
        if (args.Length == 0)
            return usage();

        var main = new MainRoutine_OIR();
        try
        {
            // パラメータファイルが2つ以上の時エラー
            if (args.Length > 1)
                throw Program.Error("Specify one parameter file.");

            var FileLines = File.ReadAllLines(args[0]);
            var param = GetParam(FileLines);

            var data = new InputDataReader_OIR(param.Input).Read();

            main.OutputPath       /**/= param.Output;
            main.CalcTimeMeshPath /**/= param.CalcTimeMesh;
            main.OutTimeMeshPath  /**/= param.OutTimeMesh;
            main.CommitmentPeriod /**/= param.CommitmentPeriod;

            main.Main(data);

            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return -1;
        }
    }

    public static CommandLine GetParam(string[] FileLines)
    {
        var param = new CommandLine();
        for (int i = 0; i < FileLines.Length; i++)
        {
            var line = FileLines[i].Trim();

            if (line.StartsWith("#") || line == "")
                continue;
            else if (line.StartsWith("Output=", StringComparison.CurrentCultureIgnoreCase))
            {
                if (param.Output != null)
                    throw Program.Error("Specify one Output parameter.");
                param.Output = Regex.Replace(line, "Output=", "", RegexOptions.IgnoreCase);
            }
            else if (line.StartsWith("Input=", StringComparison.CurrentCultureIgnoreCase))
            {
                if (param.Input != null)
                    throw Program.Error("Specify one Input parameter.");
                param.Input = Regex.Replace(line, "Input=", "", RegexOptions.IgnoreCase);
            }
            else if (line.StartsWith("CalcTimeMesh=", StringComparison.CurrentCultureIgnoreCase))
            {
                if (param.CalcTimeMesh != null)
                    throw Program.Error("Specify one CalcTimeMesh parameter.");
                param.CalcTimeMesh = Regex.Replace(line, "CalcTimeMesh=", "", RegexOptions.IgnoreCase);
            }
            else if (line.StartsWith("OutTimeMesh=", StringComparison.CurrentCultureIgnoreCase))
            {
                if (param.OutTimeMesh != null)
                    throw Program.Error("Specify one OutTimeMesh parameter.");
                param.OutTimeMesh = Regex.Replace(line, "OutTimeMesh=", "", RegexOptions.IgnoreCase);
            }
            else if (line.StartsWith("CommitmentPeriod=", StringComparison.CurrentCultureIgnoreCase))
            {
                if (param.CommitmentPeriod != null)
                    throw Program.Error("Specify one CommitmentPeriod parameter.");
                param.CommitmentPeriod = Regex.Replace(line, "CommitmentPeriod=", "", RegexOptions.IgnoreCase);
            }
            else
                throw Program.Error("There is invalid parameter on line " + (i + 1) + ".");
        }

        // パラメータ確認
        if (param.Output == null || param.Output == "")
            throw Program.Error("Please enter the Output parameter.");
        if (param.Input == null || param.Input == "")
            throw Program.Error("Please enter the Input parameter.");
        if (param.CalcTimeMesh == null || param.CalcTimeMesh == "")
            throw Program.Error("Please enter the CalcTimeMesh parameter.");
        if (param.OutTimeMesh == null || param.OutTimeMesh == "")
            throw Program.Error("Please enter the OutTimeMesh parameter.");
        if (param.CommitmentPeriod == null || param.CommitmentPeriod == "")
            throw Program.Error("Please enter the CommitmentPeriod parameter.");

        return param;
    }

    static int usage()
    {
        Console.WriteLine("\nFlexID parameter file option:");
        Console.WriteLine("Output=Calculation result output file path.");
        Console.WriteLine("Input=Input file path.");
        Console.WriteLine("CalcTimeMesh=Calculation time mesh file path.");
        Console.WriteLine("OutTimeMesh=Output time mesh file path.");
        Console.WriteLine("CommitmentPeriod=Arbitrary commitment period (integer) + 'days'or'months'or'years'.");
        return 0;
    }

    public static Exception Error(string msg)
    {
        return new ApplicationException(string.Format(msg));
    }
}
