using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FlexID.Models;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;

namespace FlexID.ViewModels;

public partial class GraphViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public GraphViewModel()
    {
        PlotModel.Legend.IsVisible = false;

        PlotModel.Grid.MajorLineColor = Colors.Black.WithOpacity(.15);
        PlotModel.Grid.MinorLineColor = Colors.Black.WithOpacity(.05);
        PlotModel.Grid.MinorLineWidth = 1;

        AxisX.Label.Text = "Days after Intake";

        SetAxisX(IsLogAxisX);
        SetAxisY(IsLogAxisY);
    }

    /// <summary>
    /// 表示中のブロックデータ(核種毎の残留放射能や男女別の線量)。
    /// </summary>
    private OutputBlockData? SelectedBlock { get; set; }

    public ObservableCollection<SerieData> Series { get; } = [];

    public Plot PlotModel { get; } = new();

    public ICommand? RefreshCommand { get; set; }

    public void Refresh() => RefreshCommand?.Execute(null);

    private IXAxis AxisX => PlotModel.Axes.Bottom;

    private ITickGenerator LinAxisX { get; } = new NumericAutomatic();

    private ITickGenerator LogAxisX { get; } = new NumericAutomatic()
    {
        MinorTickGenerator = new LogMinorTickGenerator(),
        IntegerTicksOnly = true,
        LabelFormatter = v => $"{Math.Pow(10, v)}"
    };

    private IYAxis AxisY => PlotModel.Axes.Left;

    private ITickGenerator LinAxisY { get; } = new NumericAutomatic();

    private ITickGenerator LogAxisY { get; } = new NumericAutomatic()
    {
        MinorTickGenerator = new LogMinorTickGenerator(),
        IntegerTicksOnly = true,
        LabelFormatter = v => $"{Math.Pow(10, v):0e+00}"
    };

    [ObservableProperty]
    public partial bool IsLogAxisX { get; set; } = true;

    [ObservableProperty]
    public partial bool IsLogAxisY { get; set; } = true;

    partial void OnIsLogAxisXChanged(bool value)
    {
        SetAxisX(value);
        PlotModel.Axes.AutoScaleX();
        Refresh();
    }

    partial void OnIsLogAxisYChanged(bool value)
    {
        SetAxisY(value);
        PlotModel.Axes.AutoScaleY();
        Refresh();
    }

    private void SetAxisX(bool isLog)
    {
        AxisX.TickGenerator = isLog ? LogAxisX : LinAxisX;

        foreach (var serie in Series)
            serie.SetAxisX(isLog);
    }

    private void SetAxisY(bool isLog)
    {
        AxisY.TickGenerator = isLog ? LogAxisY : LinAxisY;

        foreach (var serie in Series)
            serie.SetAxisY(isLog);
    }

    public void SetBlock(OutputData? output, OutputBlockData? block)
    {
        if (SelectedBlock == block)
            return;

        SelectedBlock = null;

        Series.Clear();
        PlotModel.Clear();

        if (output is null || block is null)
            return;

        SelectedBlock = block;

        var type = output.Type;
        var timeSteps = output.TimeSteps;
        var compartments = SelectedBlock.Compartments;

        void AddSeries(string name, Color? color = null)
        {
            var compartment = compartments.FirstOrDefault(c => c.Name == name);
            if (compartment is null)
                return;

            if (PlotModel.GetPlottables().OfType<IHasLegendText>().Any(serie => serie.LegendText == name))
                return;

            Series.Add(new SerieData(this, timeSteps, compartment, color));
        }

        AddSeries("WholeBody", Color.FromARGB(0xFF4466A3));

        if (type == OutputType.RetentionActivity ||
            type == OutputType.CumulativeActivity)
        {
            AddSeries("Urine",            /**/Color.FromARGB(0xFFF39C35));
            AddSeries("Faeces",           /**/Color.FromARGB(0xFFF14C14));
            AddSeries("AlimentaryTract*", /**/Color.FromARGB(0xFF4E97A8));
            AddSeries("Lungs*",           /**/Color.FromARGB(0xFF2B406B));
            AddSeries("Skeleton*",        /**/Color.FromARGB(0xFFB3080E));
            AddSeries("Liver*",           /**/Color.FromARGB(0xFFF2C05D));
            AddSeries("Thyroid*",         /**/Color.FromARGB(0xFF1D7B63));
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

        AxisY.Label.Text = graphLabel;

        PlotModel.Axes.AutoScale();
    }
}

public class SerieData : ViewModelBase, ICheckableItem
{
    public SerieData(GraphViewModel vm, IReadOnlyList<double> timeSteps, OutputCompartmentData compartment, Color? color)
    {
        _viewModel = vm;

        var name = compartment.Name;
        var values = compartment.Values;

        var skipCount = timeSteps[0] == 0 ? 1 : 0;
        _xsLinear = [.. timeSteps.Skip(skipCount)];
        _ysLinear = [.. values.Skip(skipCount)];

        _xsLog = [.. _xsLinear.Select(Math.Log10)];
        _ysLog = [.. _ysLinear.Select(Math.Log10)];

        _xs = vm.IsLogAxisX ? [.. _xsLog] : [.. _xsLinear];
        _ys = vm.IsLogAxisY ? [.. _ysLog] : [.. _ysLinear];

        var plot = vm.PlotModel;
        _scatter = plot.Add.Scatter(_xs, _ys);
        _scatter.LegendText = name;
        _scatter.IsVisible = color is not null;
        _scatter.MarkerFillColor = color ?? _scatter.Color;
    }

    private readonly GraphViewModel _viewModel;

    private readonly double[] _xs, _xsLinear, _xsLog;
    private readonly double[] _ys, _ysLinear, _ysLog;

    private readonly Scatter _scatter;

    public bool IsChecked
    {
        get => _scatter.IsVisible;
        set
        {
            if (_scatter.IsVisible == value)
                return;
            _scatter.IsVisible = value;
            _viewModel.Refresh();
            OnPropertyChanged();
        }
    }

    public string Name => _scatter.LegendText;

    public string ItemText => Name;

    public void SetAxisX(bool isLog)
    {
        (isLog ? _xsLog : _xsLinear).CopyTo(_xs);
    }

    public void SetAxisY(bool isLog)
    {
        (isLog ? _ysLog : _ysLinear).CopyTo(_ys);
    }
}
