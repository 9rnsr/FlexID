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
        public OutputData(OutputType type, string intakeRoute,
            string timesUnit, string valuesUnit,
            List<double> timeSteps, IEnumerable<OutputNuclideData> nuclides)
        {
            Type = type;
            IntakeRoute = intakeRoute;
            TimeStepUnit = timesUnit;
            DataValueUnit = valuesUnit;
            TimeSteps = timeSteps.AsReadOnly();
            Nuclides = nuclides.ToArray();
        }

        public OutputType Type { get; }

        public string Nuclide => Nuclides[0].Nuclide;

        public string IntakeRoute { get; }

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
            string nuclide;
            string intakeRoute;
            string[] compartments;
            string timesUnit;
            string valuesUnit;
            var timeSteps = new List<double>();

            var separators = new[] { ' ' };
            string[] Split(string line)
            {
                if (line is null)
                    throw new InvalidDataException("");
                return line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            }

            string[] values;

            values = reader.ReadLine()?.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (values is null || values.Length != 3)
                throw new InvalidDataException("unrecognized file format");

            var type =
                values[0] == "Retention" ? OutputType.RetentionActivity :
                values[0] == "CumulativeActivity" ? OutputType.CumulativeActivity :
                values[0] == "Effective/Equivalent_Dose" ? OutputType.Dose :
                values[0] == "DoseRate" ? OutputType.DoseRate :
                throw new InvalidDataException("unrecognized file format");

            nuclide = values[1];
            intakeRoute = values[2];

            int expectColumns;

            values = Split(reader.ReadLine());
            if (values.Length < 2)  // Time 1 or more compartments
                throw new InvalidDataException("too low data column count");
            expectColumns = values.Length;
            compartments = values.Skip(1).ToArray();

            timesUnit = "day";
            valuesUnit =
                type == OutputType.RetentionActivity ? "Bq/Bq" :
                type == OutputType.CumulativeActivity ? "Bq" :
                type == OutputType.Dose ? "Sv/Bq" :
                type == OutputType.DoseRate ? "Sv/h" : throw new NotSupportedException();

            values = Split(reader.ReadLine());
            if (values.Length != expectColumns)
                throw new InvalidDataException("mismatch data column count");
            if (values[0] != "[day]")
                throw new InvalidDataException("incorrect data column unit");
            var valuesUnits = values.Skip(1).Distinct();
            if (valuesUnits.Count() != 1 || valuesUnits.First() != $"[{valuesUnit}]")
                throw new InvalidDataException("incorrect data column unit");

            var nuclides = new List<OutputNuclideData>();

            var compartmentValues = compartments.Select(i => new List<double>()).ToArray();
            while (true)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    nuclides.Add(new OutputNuclideData(nuclide,
                        compartments.Select((name, i) => new OutputCompartmentData(name, compartmentValues[i]))));

                    if (line is null)
                        break;

                    values = reader.ReadLine()?.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    if (values is null || values.Length != 3)
                        throw new InvalidDataException("unrecognized file format");

                    nuclide = values[1];

                    values = Split(reader.ReadLine());
                    if (values.Length < 2)  // Time + 1 or more compartments
                        throw new InvalidDataException("too low data column count");
                    expectColumns = values.Length;
                    compartments = values.Skip(1).ToArray();

                    values = Split(reader.ReadLine());
                    if (values.Length != expectColumns)
                        throw new InvalidDataException("mismatch data column count");
                    if (values[0] != "[day]")
                        throw new InvalidDataException("incorrect data column unit");
                    valuesUnits = values.Skip(1).Distinct();
                    if (valuesUnits.Count() != 1 || valuesUnits.First() != $"[{valuesUnit}]")
                        throw new InvalidDataException("incorrect data column unit");

                    compartmentValues = compartments.Select(i => new List<double>()).ToArray();

                    line = reader.ReadLine();
                }

                values = Split(line);
                if (values.Length != expectColumns)
                    throw new InvalidDataException("mismatch data column count");

                if (!double.TryParse(values[0], out var step))
                    throw new InvalidDataException("incorrect time step column");
                if (nuclides.Count == 0)
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

            return new OutputData(type, intakeRoute, timesUnit, valuesUnit, timeSteps, nuclides);
        }
    }
}
