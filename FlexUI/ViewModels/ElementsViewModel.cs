using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlexID.ViewModels;

public partial class ElementsViewModel : ViewModelBase
{
    public ElementsViewModel()
    {
        Elements = Enumerable.Range(1, ElementNames.Length)
            .Select(n => new ElementViewModel(n, ElementNames[n - 1])).ToArray();
    }

    public ElementViewModel[] Elements { get; }

    public IEnumerable<ElementViewModel> CheckedElements => Elements.Where(elementVM => elementVM.IsChecked);

    public ElementViewModel GetElement(int number)
    {
        if (1 <= number && number <= ElementNames.Length)
            return Elements[number - 1];

        throw new ArgumentOutOfRangeException(nameof(number));
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var element in CheckedElements)
            element.IsChecked = false;
    }

    static readonly string[] Lanthanoid =
    [
        "La", "Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu",
    ];
    static readonly string[] Actinoid =
    [
        "Ac", "Th", "Pa", "U",  "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr",
    ];

    static readonly string[] ElementNames =
    [
        "H",                                                                                                          "He",
        "Li", "Be",                                                                     "B",  "C",  "N",  "O",  "F",  "Ne",
        "Na", "Mg",                                                                     "Al", "Si", "P",  "S",  "Cl", "Ar",
        "K",  "Ca", "Sc",         "Ti", "V",  "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Ga", "Ge", "As", "Se", "Br", "Kr",
        "Rb", "Sr", "Y",          "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "In", "Sn", "Sb", "Te", "I",  "Xe",
        "Cs", "Ba", ..Lanthanoid, "Hf", "Ta", "W",  "Re", "Os", "Ir", "Pt", "Au", "Hg", "Tl", "Pb", "Bi", "Po", "At", "Rn",
        "Fr", "Ra", ..Actinoid,   "Rf", "Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn", "Nh", "Fl", "Mc", "Lv", "Ts", "Og",
    ];
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
