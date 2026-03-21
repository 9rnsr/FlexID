using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlexID.ViewModels;

public partial class ElementsViewModel : ViewModelBase
{
    public ElementsViewModel()
    {
        Elements = Enumerable.Range(1, ElementTable.Names.Count)
            .Select(n => new ElementViewModel(n, ElementTable.Names[n - 1])).ToArray();
    }

    public ElementViewModel[] Elements { get; }

    public IEnumerable<ElementViewModel> CheckedElements => Elements.Where(elementVM => elementVM.IsChecked);

    public ElementViewModel GetElement(int number)
    {
        if (1 <= number && number <= ElementTable.Names.Count)
            return Elements[number - 1];

        throw new ArgumentOutOfRangeException(nameof(number));
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var element in CheckedElements)
            element.IsChecked = false;
    }

}

public partial class ElementViewModel(int number, string element) : ViewModelBase
{
    [ObservableProperty]
    public partial bool IsChecked { get; set; }

    public int Number { get; } = number;

    public string Element { get; } = element;

    //partial void OnIsCheckedChanged(bool value)
    //{
    //    System.Diagnostics.Debug.WriteLine($"{Number}-{Element} IsChecked = {value}");
    //}
}
