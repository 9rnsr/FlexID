using Prism.Mvvm;
using Reactive.Bindings;
using System.Collections.Generic;

namespace FlexID.ViewModels
{
    public class InputOIRViewModel : BindableBase
    {

        public IReadOnlyList<string> CommitmentPeriodUnits { get; } = new List<string>
        {
            "days",
            "months",
            "years",
        }.AsReadOnly();

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public InputOIRViewModel()
        {

        }
    }
}
