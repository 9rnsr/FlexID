using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Services;

namespace FlexID.ViewModels;

public partial class NuclideViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Nuclide { get; set; }

    [ObservableProperty]
    public partial bool IsChecked { get; set; }
}

public partial class InputScoeffViewModel : ViewModelBase
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public InputScoeffViewModel()
    {
        OutputDirectory = @"out\";

        WeakReferenceMessenger.Default.Register<BusyState>(this, (r, m) => IsBusy = m.Value);

#if false
        Task.Run(() =>
        {
            var nuclides = SAFDataReader.ReadRadNuclides().Select(nuc => new NuclideViewModel { Nuclide = nuc });

            Application.Current.Dispatcher.Invoke(() =>
            {
                Nuclides.AddRange(nuclides);
            });
        });
#endif
    }

    public ObservableCollection<NuclideViewModel> Nuclides { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    public partial bool CalcMale { get; set; } = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    public partial bool CalcFemale { get; set; } = true;

    [ObservableProperty]
    public partial bool CalcPchip { get; set; } = true;

    [ObservableProperty]
    public partial bool IdacDoseCompatible { get; set; } = false;

    [ObservableProperty]
    public partial string OutputDirectory { get; set; }

    [RelayCommand]
    private async Task SelectOutputDirectory(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            //var dialog = new SaveFileDialog();
            //dialog.InitialDirectory = Environment.CurrentDirectory;
            //if (dialog.ShowDialog() == true)
            //    selected = dialog.FileName;
        }
        if (selected is not null)
            OutputDirectory = selected;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    private partial bool IsBusy { get; set; }

    private bool CanRun => !IsBusy && (CalcMale || CalcFemale);

    [RelayCommand(CanExecute = nameof(CanRun))]
    private async Task Run()
    {
        try
        {
            WeakReferenceMessenger.Default.Send(new BusyState(true));

            var outputDir = OutputDirectory;

            if (!Path.IsPathFullyQualified(outputDir))
                outputDir = Path.Combine(AppResource.ProcessDir, outputDir);

            Directory.CreateDirectory(outputDir);

            var calcAM = CalcMale;
            var calcAF = CalcFemale;
            if (!calcAM && !calcAF)
                throw new Exception("Select target male and/or female.");

            var nuclides = Nuclides.Where(ni => ni.IsChecked).Select(ni => ni.Nuclide).ToArray();
            if (!nuclides.Any())
                throw new Exception("No nuclides selected.");

            var interpolationMethod = CalcPchip ? "PCHIP" : "線形補間";

            var isIdacDoseCompatible = IdacDoseCompatible;

            await Task.WhenAll(
                calcAM ? Run(Sex.Male, interpolationMethod, nuclides, outputDir, isIdacDoseCompatible) : Task.CompletedTask,
                calcAF ? Run(Sex.Female, interpolationMethod, nuclides, outputDir, isIdacDoseCompatible) : Task.CompletedTask);

            MessageService.Confirm("Finished S-Coefficient caculation");
        }
        catch (Exception ex)
        {
            MessageService.Error(ex.Message);
        }
        finally
        {
            WeakReferenceMessenger.Default.Send(new BusyState(false));
        }
    }

    private static Task Run(Sex sex, string interpolationMethod, string[] nuclides, string outputDir, bool isIdacDoseCompatible)
    {
        var safdata = SAFDataReader.ReadSAF(sex);
        if (safdata is null)
            throw new Exception("There are multiple files of the same type.");

        return Task.WhenAll(nuclides.Select(nuc => Task.Run(() =>
        {
            var calcS = new CalcScoeff(safdata);

            calcS.InterpolationMethod = interpolationMethod;

            calcS.CalcS(nuc);

            var target = $@"{nuc}_{(sex == Sex.Male ? "AM" : "AF")}";

            if (isIdacDoseCompatible)
            {
                var scoeffFilePath = Path.Combine(outputDir, target + ".csv");
                calcS.WriteOutIdacDoseCompatibleResult(scoeffFilePath, sex);
            }
            else
            {
                var scoeffFilePath = Path.Combine(outputDir, target + ".txt");
                calcS.WriteOutTotalResult(scoeffFilePath);
            }
        })));
    }
}
