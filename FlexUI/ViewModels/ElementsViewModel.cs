using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using R3;

namespace FlexID.ViewModels;

public partial class ElementsViewModel : ViewModelBase
{
    public ElementsViewModel()
    {
        Elements = Enumerable.Range(1, ElementTable.Names.Count)
            .Select(n => new ElementViewModel(n, ElementTable.Names[n - 1])).ToArray();

        IEnumerable<ElementViewModel> ListVMs(string[] elements) =>
            Elements.Where(el => elements.Contains(el.Element));

        OirPart2 = new ElementGroup(ListVMs(Elements_OirPart2)).AddTo(disposables);
        OirPart3 = new ElementGroup(ListVMs(Elements_OirPart3)).AddTo(disposables);
        OirPart4 = new ElementGroup(ListVMs(Elements_OirPart4)).AddTo(disposables);
        OirPart5 = new ElementGroup(ListVMs(Elements_OirPart5)).AddTo(disposables);
    }

    public ElementViewModel[] Elements { get; }

    public IEnumerable<ElementViewModel> CheckedElements => Elements.Where(elementVM => elementVM.IsChecked);

    public ElementViewModel GetElement(int number)
    {
        if (1 <= number && number <= ElementTable.Names.Count)
            return Elements[number - 1];

        throw new ArgumentOutOfRangeException(nameof(number));
    }

    /// <summary>
    /// 全ての元素のチェックを外す。
    /// </summary>
    [RelayCommand]
    private void ClearAll()
    {
        foreach (var element in CheckedElements)
            element.IsChecked = false;
    }

    [ObservableProperty]
    public partial bool IsEnableOirSelector { get; set; }

    public ElementGroup OirPart2 { get; }
    public ElementGroup OirPart3 { get; }
    public ElementGroup OirPart4 { get; }
    public ElementGroup OirPart5 { get; }

    private static readonly string[] Elements_OirPart2 =
    [
        "H", "C", "P", "S", "Ca", "Fe", "Co", "Zn", "Sr", "Y", "Zr", "Nb", "Mo", "Tc",
    ];

    private static readonly string[] Elements_OirPart3 =
    [
        "Ru", "Sb", "Te", "I", "Cs", "Ba", "Ir", "Pb", "Bi", "Po", "Rn", "Ra", "Th", "U",
    ];

    private static readonly string[] Elements_OirPart4 =
    [
        "La", "Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu",
        "Ac", /* */ "Pa", /* */ "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm",
    ];

    private static readonly string[] Elements_OirPart5 =
    [
        "Be", "F", "Na", "Mg", "Al", "Si", "Cl", "K", "Sc", "Ti", "V", "Cr", "Mn", "Ni",
        "Cu", "Ga", "Ge", "As", "Se", "Br", "Rb", "Rh", "Pd", "Ag", "Cd", "In", "Sn",
        "Hf", "Ta", "W", "Re", "Os", "Pt", "Au", "Hg", "Tl", "At", "Fr",
    ];
}

/// <summary>
/// 1つの元素を表現する。
/// </summary>
public partial class ElementViewModel(int number, string element) : ViewModelBase
{
    /// <summary>
    /// 原子番号。
    /// </summary>
    public int Number { get; } = number;

    /// <summary>
    /// 元素記号。
    /// </summary>
    public string Element { get; } = element;

    /// <summary>
    /// 元素がチェックされている場合は<see langword="true"/>。
    /// </summary>
    [ObservableProperty]
    public partial bool IsChecked { get; set; }
}

/// <summary>
/// 複数の元素をグループ化し、それらのチェック状態をまとめて取り扱う。
/// </summary>
public partial class ElementGroup : ViewModelBase
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="elementVMs"></param>
    public ElementGroup(IEnumerable<ElementViewModel> elementVMs)
    {
        ElementVMs = [.. elementVMs];

        ElementVMs.Select(el =>
            // IsCheckedの変更に対して、ElementViewModelそれ自体を通知する
            el.ObservePropertyChanged(vm => vm.IsChecked).Skip(1).Select(_ => el)
        ).Merge().Subscribe(el =>
        {
            CheckedCout += el.IsChecked ? +1 : -1;
            IsChecked = CheckedCout != 0;
        }).AddTo(disposables);
    }

    private IEnumerable<ElementViewModel> ElementVMs { get; }

    /// <summary>
    /// チェックされている元素の数。
    /// </summary>
    public int CheckedCout { get; private set; }

    /// <summary>
    /// 1個以上の元素がチェックされている場合は<see langword="true"/>。
    /// </summary>
    [ObservableProperty]
    public partial bool IsChecked { get; private set; }

    /// <summary>
    /// 全ての元素のチェック状態を切り替える。
    /// </summary>
    [RelayCommand]
    private void Switch()
    {
        var newValue = !IsChecked;
        foreach (var elementVM in ElementVMs)
            elementVM.IsChecked = newValue;
    }
}
