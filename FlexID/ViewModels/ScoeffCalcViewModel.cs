using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Calc;
using Microsoft.Win32;

namespace FlexID.ViewModels;

public partial class NuclideItem : ObservableObject
{
    [ObservableProperty]
    public partial string Nuclide { get; set; }

    [ObservableProperty]
    public partial bool IsChecked { get; set; }
}

public partial class ScoeffCalcViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public ScoeffCalcViewModel()
    {
        OutputFilePath = @"out\";

        Task.Run(() =>
        {
            var nuclides = SAFDataReader.ReadRadNuclides().Select(nuc => new NuclideItem { Nuclide = nuc });

            Application.Current.Dispatcher.Invoke(() =>
            {
                Nuclides.AddRange(nuclides);
            });
        });
    }

    [ObservableProperty]
    public partial bool CalcMale { get; set; } = true;

    [ObservableProperty]
    public partial bool CalcFemale { get; set; } = true;

    [ObservableProperty]
    public partial bool CalcPchip { get; set; } = true;

    [ObservableProperty]
    public partial bool IdacDoseCompatible { get; set; } = false;

    public ObservableCollection<NuclideItem> Nuclides { get; } = [];

    [ObservableProperty]
    public partial string OutputFilePath { get; set; }

    [RelayCommand]
    private void SelectOutputFilePath()
    {
        var dialog = new SaveFileDialog();
        dialog.InitialDirectory = Environment.CurrentDirectory;
        if (dialog.ShowDialog() == true)
            OutputFilePath = dialog.FileName;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    private partial bool IsBusy { get; set; }

    private bool CanRun => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanRun))]
    private async Task Run()
    {
        try
        {
            WeakReferenceMessenger.Default.Send(new BusyState(true));

            var outPath = OutputFilePath;
            Directory.CreateDirectory(outPath);

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
                calcAM ? Run(Sex.Male, interpolationMethod, nuclides, outPath, isIdacDoseCompatible) : Task.CompletedTask,
                calcAF ? Run(Sex.Female, interpolationMethod, nuclides, outPath, isIdacDoseCompatible) : Task.CompletedTask);

            MessageBox.Show("Finish", "S-Coefficient", MessageBoxButton.OK);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            WeakReferenceMessenger.Default.Send(new BusyState(false));
        }
    }

    private static Task Run(Sex sex, string interpolationMethod, string[] nuclides, string outPath, bool isIdacDoseCompatible)
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
                var scoeffFilePath = Path.Combine(outPath, target + ".csv");
                calcS.WriteOutIdacDoseCompatibleResult(scoeffFilePath, sex);
            }
            else
            {
                var scoeffFilePath = Path.Combine(outPath, target + ".txt");
                calcS.WriteOutTotalResult(scoeffFilePath);
            }
        })));
    }
}
