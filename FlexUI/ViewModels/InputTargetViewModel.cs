using CommunityToolkit.Mvvm.ComponentModel;
using FlexID.Models;
using R3;

namespace FlexID.ViewModels;

public partial class InputTargetViewModel : ViewModelBase, ICheckableItem
{
    public InputTargetViewModel(InputTarget target)
    {
        InputTarget = target;

        Progenies = [.. target.Progenies.Select(p => new ProgenySelectionViewModel(this, p))];

        OnCheckedProgeniesChanged();
    }

    public InputTarget InputTarget { get; }

    [ObservableProperty]
    public partial bool IsChecked { get; set; }

    string ICheckableItem.ItemText => Title;

    public string Title => InputTarget.Title;

    public string Nuclide => InputTarget.Nuclide;

    public bool CanCheckProgenies => InputTarget.Progenies.Count != 0;

    public IReadOnlyList<ProgenySelectionViewModel> Progenies { get; }

    [ObservableProperty]
    public partial string? CheckedProgenies { get; private set; }

    void OnCheckedProgeniesChanged()
    {
        CheckedProgenies = string.Join(", ", Progenies.Where(p => p.IsChecked).Select(p => p.Progeny));
    }

    public partial class ProgenySelectionViewModelBase : ViewModelBase
    {
        private readonly InputTargetViewModel _owner;

        public string Progeny { get; }

        [ObservableProperty]
        public partial bool IsChecked { get; set; } = true;

        public ProgenySelectionViewModelBase(InputTargetViewModel owner, string progeny)
        {
            _owner = owner;
            Progeny = progeny;
        }

        partial void OnIsCheckedChanged(bool value)
        {
            _owner.OnCheckedProgeniesChanged();
        }
    }
}

public partial class ProgenySelectionViewModel : InputTargetViewModel.ProgenySelectionViewModelBase
{
    public ProgenySelectionViewModel(InputTargetViewModel owner, string progeny)
        : base(owner, progeny)
    { }
}
