using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FlexID.Calc
{
    /// <summary>
    /// アウトプットファイル種別。
    /// </summary>
    public enum OutputType
    {
        Unknown,
        RetentionActivity,
        CumulativeActivity,
        Dose,
        DoseRate
    }

    [DebuggerDisplay("{Nuclide}, {Type}, Compartments: {Compartments.Length}")]
    public class OutputData
    {
        public OutputData(OutputType type, string title,
            string timesUnit, string valuesUnit,
            List<double> timeSteps, IEnumerable<OutputNuclideData> nuclides)
        {
            Type = type;
            Title = title;
            TimeStepUnit = timesUnit;
            DataValueUnit = valuesUnit;
            TimeSteps = timeSteps.AsReadOnly();
            Nuclides = nuclides.ToArray();
        }

        public OutputType Type { get; }

        public string Nuclide => Nuclides[0].Nuclide;

        public string Title { get; }

        public string TimeStepUnit { get; }

        public string DataValueUnit { get; }

        public IReadOnlyList<double> TimeSteps { get; }

        public OutputNuclideData[] Nuclides { get; }
    }

    public class OutputNuclideData
    {
        public OutputNuclideData(string nuclide, IEnumerable<OutputCompartmentData> compartments)
        {
            Nuclide = nuclide;

            Compartments = compartments.ToArray();
        }

        public string Nuclide { get; }

        public OutputCompartmentData[] Compartments { get; }
    }

    [DebuggerDisplay("{Name}, Values: {Values}")]
    public class OutputCompartmentData
    {
        public OutputCompartmentData(string name, IEnumerable<double> values)
        {
            Name = name;
            Values = values.ToList().AsReadOnly();
        }

        public string Name { get; }

        public IReadOnlyList<double> Values { get; }
    }

    /// <summary>
    /// アウトプットファイルの読み取り処理。
    /// </summary>
    public class OutputDataReader : IDisposable
    {
        /// <summary>
        /// アウトプットファイルの読み出し用TextReader。
        /// </summary>
        private readonly StreamReader reader;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="outputPath">アウトプットファイルのパス文字列。</param>
        public OutputDataReader(string outputPath)
        {
            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var reader = new StreamReader(stream);

            this.reader = reader;
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="reader">アウトプットの読み込み元。</param>
        public OutputDataReader(StreamReader reader)
        {
            this.reader = reader;
        }

        public void Dispose() => reader.Dispose();

        /// <summary>
        /// アウトプットファイルを読み込む。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">内容が適切ではない場合。</exception>
        public OutputData Read()
        {
            string title;
            string timesUnit;
            string valuesUnit;
            var timeSteps = new List<double>();

            string line;
            string[] values;

            void ReadEmptyLine()
            {
                line = reader.ReadLine();
                if (line != "")
                    throw new InvalidDataException("unrecognized file format");
            }

            line = reader.ReadLine();
            if (!line.StartsWith("FlexID output: "))
                throw new InvalidDataException("unrecognized file format");

            line = line.Substring("FlexID output: ".Length);
            var (type, unit) =
                line == "RetentionActivity" ? (OutputType.RetentionActivity, "Bq/Bq") :
                line == "CumulativeActivity" ? (OutputType.CumulativeActivity, "Bq") :
                line == "Dose" ? (OutputType.Dose, "Sv/Bq") :
                line == "DoseRate" ? (OutputType.DoseRate, "Sv/h") :
                throw new InvalidDataException("unrecognized file format");

            title = reader.ReadLine();
            if (title is null)
                throw new InvalidCastException("unrecognized file format");

            ReadEmptyLine();

            line = reader.ReadLine();
            if (line is null || !line.StartsWith("Radionuclide: "))
                throw new InvalidCastException("unrecognized file format");
            var nuclides = line.Substring("Radionuclide: ".Length).Split(new[] { ", " }, StringSplitOptions.None);

            line = reader.ReadLine();
            if (line is null || !line.StartsWith("Units: "))
                throw new InvalidCastException("unrecognized file format");
            var units = line.Substring("Units: ".Length).Split(new[] { ", " }, StringSplitOptions.None);

            timesUnit = "day";
            valuesUnit =
                type == OutputType.RetentionActivity ? "Bq/Bq" :
                type == OutputType.CumulativeActivity ? "Bq" :
                type == OutputType.Dose ? "Sv/Bq" :
                type == OutputType.DoseRate ? "Sv/h" : throw new NotSupportedException();
            if (units.Length != 2 || units[0] != timesUnit || units[1] != valuesUnit)
                throw new InvalidCastException("unrecognized file format");

            ReadEmptyLine();

            var separators = new[] { ' ' };
            string[] ReadValues(string ln)
            {
                if (ln is null)
                    throw new InvalidDataException("unrecognized file format");
                return ln.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            }

            var outputCount =
                type == OutputType.RetentionActivity || type == OutputType.CumulativeActivity ? nuclides.Length : 1;

            var nuclideDatas = new List<OutputNuclideData>(outputCount);
            for (int iOut = 0; iOut < outputCount; iOut++)
            {
                var nuclide = reader.ReadLine();
                if (nuclide != nuclides[iOut])
                    throw new InvalidDataException("mismatch radiouclide");

                values = ReadValues(reader.ReadLine());
                if (values.Length < 2)  // Time 1 or more compartments
                    throw new InvalidDataException("too low data column count");
                int expectColumns = values.Length;
                var compartments = values.Skip(1).ToArray();
                var compartmentValues = compartments.Select(i => new List<double>()).ToArray();

                while (true)
                {
                    line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        nuclideDatas.Add(new OutputNuclideData(nuclide,
                            compartments.Select((name, i) => new OutputCompartmentData(name, compartmentValues[i]))));
                        break;
                    }

                    values = ReadValues(line);
                    if (values.Length != expectColumns)
                        throw new InvalidDataException("mismatch data column count");

                    if (!double.TryParse(values[0], out var step))
                        throw new InvalidDataException("incorrect time step column");
                    if (nuclideDatas.Count == 0)
                        timeSteps.Add(step);
                    else
                    {
                        var istep = compartmentValues[0].Count;
                        if (timeSteps[istep] != step)
                            throw new InvalidDataException("incorrect time step column");
                    }

                    for (int i = 1; i < expectColumns; i++)
                    {
                        if (!double.TryParse(values[i], out var value))
                            value = double.NaN;
                        compartmentValues[i - 1].Add(value);
                    }
                }
            }
            if (line != null)
                throw new InvalidDataException("unrecognized file format");

            return new OutputData(type, title, timesUnit, valuesUnit, timeSteps, nuclideDatas);
        }
    }
}
