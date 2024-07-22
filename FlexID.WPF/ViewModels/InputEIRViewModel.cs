using Prism.Mvvm;
using System.Collections.Generic;

namespace FlexID.ViewModels
{
    public class InputEIRViewModel : BindableBase
    {

        public IReadOnlyList<string> CommitmentPeriodUnits { get; } = new List<string>
        {
            "days",
            "months",
            "years",
        }.AsReadOnly();

        public IReadOnlyList<string> IntakeAges { get; } = new List<string>
        {
            "3months old",
            "1years old",
            "5years old",
            "10years old",
            "15years old",
            "Adult",
        }.AsReadOnly();

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public InputEIRViewModel()
        {

        }
    }
}
