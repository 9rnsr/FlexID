using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FlexID.Calc;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace FlexID.Viewer.ViewModels;

public partial class GraphViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public GraphViewModel()
    {
        PlotModel.Axes.Add(LogAxisX);
        PlotModel.Axes.Add(LogAxisY);
    }

    /// <summary>
    /// 表示中のブロックデータ(核種毎の残留放射能や男女別の線量)。
    /// </summary>
    private OutputBlockData SelectedBlock { get; set; }

    public ObservableCollection<RegionData> Regions { get; } = [];

    public PlotModel PlotModel { get; } = new();

    private LogarithmicAxis LogAxisX { get; } = new LogarithmicAxis()
    {
        Position = AxisPosition.Bottom,
        MajorGridlineStyle = LineStyle.Automatic,
        MajorGridlineColor = OxyColor.FromRgb(0, 0, 0),
        MinorGridlineStyle = LineStyle.Dot,
        MinorGridlineColor = OxyColor.FromRgb(128, 128, 128),
        TitleFontSize = 14,
        Title = "Days after Intake"
    };

    private LogarithmicAxis LogAxisY { get; } = new LogarithmicAxis()
    {
        Position = AxisPosition.Left,
        MajorGridlineStyle = LineStyle.Automatic,
        MajorGridlineColor = OxyColor.FromRgb(0, 0, 0),
        MinorGridlineStyle = LineStyle.Dot,
        MinorGridlineColor = OxyColor.FromRgb(128, 128, 128),
        TitleFontSize = 14,
        AxisTitleDistance = 10,
    };

    private LinearAxis LinAxisX { get; } = new LinearAxis()
    {
        Position = AxisPosition.Bottom,
        MajorGridlineStyle = LineStyle.Automatic,
        MajorGridlineColor = OxyColor.FromRgb(0, 0, 0),
        MinorGridlineStyle = LineStyle.Dot,
        MinorGridlineColor = OxyColor.FromRgb(128, 128, 128),
        TitleFontSize = 14,
        Title = "Days after Intake"
    };

    private LinearAxis LinAxisY { get; } = new LinearAxis()
    {
        Position = AxisPosition.Left,
        MajorGridlineStyle = LineStyle.Automatic,
        MajorGridlineColor = OxyColor.FromRgb(0, 0, 0),
        MinorGridlineStyle = LineStyle.Dot,
        MinorGridlineColor = OxyColor.FromRgb(128, 128, 128),
        TitleFontSize = 14,
        AxisTitleDistance = 10,
    };

    [ObservableProperty]
    public partial bool IsLogAxisX { get; set; } = true;

    [ObservableProperty]
    public partial bool IsLogAxisY { get; set; } = true;

    partial void OnIsLogAxisXChanged(bool value)
    {
        PlotModel.Axes[0] = value ? LogAxisX : LinAxisX;
        PlotModel.InvalidatePlot(false);
    }

    partial void OnIsLogAxisYChanged(bool value)
    {
        PlotModel.Axes[1] = value ? LogAxisY : LinAxisY;
        PlotModel.InvalidatePlot(false);
    }

    public void SetBlock(OutputData output, OutputBlockData block)
    {
        if (SelectedBlock == block)
            return;

        SelectedBlock = null;

        Regions.Clear();
        PlotModel.Series.Clear();
        PlotModel.InvalidatePlot(updateData: true);

        if (block is null)
            return;

        SelectedBlock = block;

        var type = output.Type;
        var timeSteps = output.TimeSteps;
        var compartments = SelectedBlock.Compartments;

        void AddSeries(string name, OxyColor? color = null)
        {
            var compartment = compartments.FirstOrDefault(c => c.Name == name);
            if (compartment is null)
                return;

            if (PlotModel.Series.Any(ser => ser.Title == name))
                return;

            var series = new ScatterSeries()
            {
                Title = name,
                IsVisible = color != null,
                MarkerFill = color ?? OxyColors.Automatic,
            };

            var values = compartment.Values;
            for (int j = 0; j < timeSteps.Count; j++)
            {
                if (timeSteps[j] == 0)
                    continue;
                series.Points.Add(new ScatterPoint(timeSteps[j], values[j]));
            }

            Regions.Add(new RegionData(series, compartment.Name));
            PlotModel.Series.Add(series);
        }

        AddSeries("WholeBody", OxyColor.FromUInt32(0xFF4466A3));

        if (type == OutputType.RetentionActivity ||
            type == OutputType.CumulativeActivity)
        {
            AddSeries("Urine",            /**/OxyColor.FromUInt32(0xFFF39C35));
            AddSeries("Faeces",           /**/OxyColor.FromUInt32(0xFFF14C14));
            AddSeries("AlimentaryTract*", /**/OxyColor.FromUInt32(0xFF4E97A8));
            AddSeries("Lungs*",           /**/OxyColor.FromUInt32(0xFF2B406B));
            AddSeries("Skeleton*",        /**/OxyColor.FromUInt32(0xFFB3080E));
            AddSeries("Liver*",           /**/OxyColor.FromUInt32(0xFFF2C05D));
            AddSeries("Thyroid*",         /**/OxyColor.FromUInt32(0xFF1D7B63));
        }

        foreach (var compartment in compartments)
            AddSeries(compartment.Name);

        var graphLabel = type switch
        {
            OutputType.RetentionActivity  /**/=> "Retention",
            OutputType.CumulativeActivity /**/=> "CumulativeActivity",
            OutputType.Dose               /**/=> "Effective/Equivalent Dose",
            OutputType.DoseRate           /**/=> "DoseRate",
            _ => throw new NotSupportedException(),
        };
        graphLabel += $"[{output.DataValueUnit}]";

        LogAxisY.Title = graphLabel;
        LinAxisY.Title = graphLabel;

        // グラフがFitする範囲などを更新するためにupdateData: trueが必要。
        PlotModel.InvalidatePlot(updateData: true);
    }
}

public class RegionData
{
    public RegionData(ScatterSeries serie, string name)
    {
        this.serie = serie;
        Name = name;
    }

    private readonly ScatterSeries serie;

    public string Name { get; }

    public bool IsVisible
    {
        get => serie.IsVisible;
        set
        {
            if (serie.IsVisible == value)
                return;
            serie.IsVisible = value;
            serie.PlotModel.InvalidatePlot(true);
        }
    }
}
