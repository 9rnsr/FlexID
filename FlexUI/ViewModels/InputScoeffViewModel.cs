using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Models;
using FlexID.Services;
using R3;

namespace FlexID.ViewModels;

public partial class NuclideViewModel(string nuc) : ObservableObject, ICheckableItem
{
    [ObservableProperty]
    public partial bool IsChecked { get; set; }

    public string Nuclide { get; } = nuc;

    string ICheckableItem.ItemText => Nuclide;
}

public partial class InputScoeffViewModel : ViewModelBase
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public InputScoeffViewModel()
    {
        Nuclides = new(nuc => new NuclideViewModel(nuc));

        OutputDirectory = @"out\";

        WeakReferenceMessenger.Default.Register<BusyState>(this, (r, m) => IsBusy = m.Value);

        Nuclides.ObservePropertyChanged(m => m.IsCheckedAny)
                   .Subscribe(_ => RunCommand.NotifyCanExecuteChanged())
                   .AddTo(disposables);

        // 放射線データに定義されている核種の一覧を取得する。
        Task.Run(async () =>
        {
            await Nuclides.AddRangeAsync(SAFDataReader.ReadRadNuclides());

            //Debug.WriteLine("Got Nuclides");
            WeakReferenceMessenger.Default.Send(new Views.ScoeffState { IsRead = true });
        });
    }

    public CheckableItemsView<string, NuclideViewModel> Nuclides { get; }

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
            var appId = App.Current.AppWindow!.Id;
            var picker = new Microsoft.Windows.Storage.Pickers.FolderPicker(appId)
            {
                SuggestedStartLocation = Microsoft.Windows.Storage.Pickers.PickerLocationId.Unspecified,
                //SuggestedFolder = Environment.CurrentDirectory, // Windows App SDK 2.0
            };

            var result = await picker.PickSingleFolderAsync();

            selected = result?.Path;
        }
        if (selected is not null)
            OutputDirectory = selected;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    private partial bool IsBusy { get; set; }

    private bool CanRun => !IsBusy && Nuclides.IsCheckedAny && (CalcMale || CalcFemale);

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

            var nuclides = Nuclides.FilteredItems.Where(ni => ni.IsChecked).Select(ni => ni.Nuclide).ToArray();
            if (!nuclides.Any())
                throw new Exception("No nuclides selected.");

            var interpolationMethod = CalcPchip ? "PCHIP" : "線形補間";

            var isIdacDoseCompatible = IdacDoseCompatible;

            await Task.WhenAll(
                calcAM ? Run(Sex.Male, interpolationMethod, nuclides, outputDir, isIdacDoseCompatible) : Task.CompletedTask,
                calcAF ? Run(Sex.Female, interpolationMethod, nuclides, outputDir, isIdacDoseCompatible) : Task.CompletedTask);

            var message = "All tasks completed successfully.";
            MessageService.Confirm("Caculation Finished", message);
        }
        catch (Exception ex)
        {
            var message = "Unexpected error has occurred during processing: " + ex.Message;
            MessageService.Error("Calculation Stopped", message);
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
