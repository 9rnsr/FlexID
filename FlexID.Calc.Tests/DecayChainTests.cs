using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FlexID.Calc.Tests
{
    using static InputErrorTestHelpers;

    [TestClass]
    public class DecayChainTests
    {
        [TestMethod]
        public void Try()
        {
            //    Nuclide  Halflife    f1      Nuclide   f2      Nuclide
            //  1 Sb-129      4.40h  2.262E-01 Te-129m 7.738E-01 Te-129
            //  2 Te-129m     33.6d  6.300E-01 Te-129  3.700E-01 I-129
            //  3 Te-129      69.6m  1.000E+00 I-129
            //  4 I-129    1.57E+7y  1.000E+00 (Xe-129)

            // Sb-129 ┬──┬> Te-129 ┬> I-129 ─> (Xe-129)
            //        │    /          /
            //        └> Te-129m ──┘

            var reader = CreateReader(new[]
            {
                "[title]", "dummy",
                "",
                "[nuclide]", "  Sb-129  Te-129m  Te-129  I-129",
                "",

                "[Sb-129:compartment]",
                "  inp    input     ---",
                "  acc    Plasma    ---",
                "",
                "[Te-129m:compartment]",
                "  acc    Blood1    ---",
                "",
                "[Te-129:compartment]",
                "  acc    Blood1    ---",
                "",
                "[I-129:compartment]",
                "  acc    Blood1    ---",
                "",

                "[Sb-129:transfer]",
                "  input           Plasma  100%",
                "",
                "[Te-129m:transfer]",
                "  Sb-129/Plasma   Blood1  ---",
                "",
                "[Te-129:transfer]",
                "  Sb-129/Plasma   Blood1  ---",
                "",
                "[I-129:transfer]",
                "  Sb-129/Plasma   Blood1  ---",
            });

            var data = reader.Read();
            var organs = data.Organs;
            Assert.IsTrue(organs.Count == 5);

            Assert.IsTrue(organs[0].ToString() == "Sb-129/input");
            Assert.IsTrue(organs[1].ToString() == "Sb-129/Plasma");
            Assert.IsTrue(organs[2].ToString() == "Te-129m/Blood1");
            Assert.IsTrue(organs[3].ToString() == "Te-129/Blood1");
            Assert.IsTrue(organs[4].ToString() == "I-129/Blood1");

            CollectionAssert.AreEquivalent(organs[1].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/input" });
            CollectionAssert.AreEquivalent(organs[2].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Plasma" });
            CollectionAssert.AreEquivalent(organs[3].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Plasma", "Te-129m/Blood1" });
            CollectionAssert.AreEquivalent(organs[4].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Te-129m/Blood1", "Te-129/Blood1" });
        }

        [TestMethod]
        public void Try1()
        {
            //    Nuclide  Halflife    f1      Nuclide   f2      Nuclide
            //  1 Sb-129      4.40h  2.262E-01 Te-129m 7.738E-01 Te-129
            //  2 Te-129m     33.6d  6.300E-01 Te-129  3.700E-01 I-129
            //  3 Te-129      69.6m  1.000E+00 I-129
            //  4 I-129    1.57E+7y  1.000E+00 (Xe-129)

            // Sb-129 ┬──┬> Te-129 ┬> I-129 ─> (Xe-129)
            //        │    /          /
            //        └> Te-129m ──┘

            var reader = CreateReader(new[]
            {
                "[title]", "dummy",
                "[nuclide]", "  Sb-129  Te-129m  Te-129  I-129",

                "[Sb-129:compartment]",   "  inp    input     ---",
                                          "  acc    Plasma    ---",
                "[Te-129m:compartment]",  "  acc    Blood1    ---",
                "[Te-129:compartment]",   "  acc    Blood1    ---",
                "[I-129:compartment]",    "  acc    Blood1    ---",
                "",

                "[Sb-129:transfer]",  "  input           Plasma  100%",
                "[Te-129m:transfer]", "  Sb-129/Plasma   Blood1  ---",
                "[Te-129:transfer]",  "  Sb-129/Plasma   Blood1  ---",
                "[I-129:transfer]",   "  Sb-129/Plasma   Blood1  100",
            });

            var data = reader.Read();
            var organs = data.Organs;
            Assert.IsTrue(organs.Count == 6);

            Assert.IsTrue(organs[0].ToString() == "Sb-129/input");
            Assert.IsTrue(organs[1].ToString() == "Sb-129/Plasma");
            Assert.IsTrue(organs[2].ToString() == "Te-129m/Blood1");
            Assert.IsTrue(organs[3].ToString() == "Te-129/Blood1");
            Assert.IsTrue(organs[4].ToString() == "I-129/Blood1");
            Assert.IsTrue(organs[5].IsDecayCompartment);
            Assert.IsTrue(organs[5].ToString() == "I-129/Decay-Sb-129/Plasma");

            CollectionAssert.AreEquivalent(organs[1].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/input" });
            CollectionAssert.AreEquivalent(organs[2].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Plasma" });
            CollectionAssert.AreEquivalent(organs[3].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Plasma", "Te-129m/Blood1" });
            CollectionAssert.AreEquivalent(organs[4].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "I-129/Decay-Sb-129/Plasma" });
            CollectionAssert.AreEquivalent(organs[5].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Te-129m/Blood1", "Te-129/Blood1" });
        }

        [TestMethod]
        public void Try2()
        {
            //    Nuclide  Halflife    f1      Nuclide   f2      Nuclide
            //  1 Sb-129      4.40h  2.262E-01 Te-129m 7.738E-01 Te-129
            //  2 Te-129m     33.6d  6.300E-01 Te-129  3.700E-01 I-129
            //  3 Te-129      69.6m  1.000E+00 I-129
            //  4 I-129    1.57E+7y  1.000E+00 (Xe-129)

            // Sb-129 ┬──┬> Te-129 ┬> I-129 ─> (Xe-129)
            //        │    /          /
            //        └> Te-129m ──┘

            var reader = CreateReader(new[]
            {
                "[title]", "dummy",
                "[nuclide]", "  Sb-129  Te-129m  Te-129  I-129",

                "[Sb-129:compartment]",   "  inp    input     ---",
                                          "  acc    Plasma    ---",
                "[Te-129m:compartment]",  "  acc    Blood1    ---",
                "[Te-129:compartment]",   "  acc    Blood1    ---",
                "[I-129:compartment]",    "  acc    Blood1    ---",
                "",

                "[Sb-129:transfer]",  "  input           Plasma  100%",
                "[Te-129m:transfer]", "  Sb-129/Plasma   Blood1  ---",
                "[Te-129:transfer]",  "  Sb-129/Plasma   Blood1  ---",
                "[I-129:transfer]",   "  Te-129m/Blood1  Blood1  100",
            });

            var data = reader.Read();
            var organs = data.Organs;
            Assert.IsTrue(organs.Count == 6);   // fail!

            Assert.IsTrue(organs[0].ToString() == "Sb-129/input");
            Assert.IsTrue(organs[1].ToString() == "Sb-129/Plasma");
            Assert.IsTrue(organs[2].ToString() == "Te-129m/Blood1");
            Assert.IsTrue(organs[3].ToString() == "Te-129/Blood1");
            Assert.IsTrue(organs[4].ToString() == "I-129/Blood1");
            Assert.IsTrue(organs[5].ToString() == "I-129/Decay-Sb-129/Plasma");

            CollectionAssert.AreEquivalent(organs[1].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/input" });
            CollectionAssert.AreEquivalent(organs[2].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Plasma" });
            CollectionAssert.AreEquivalent(organs[3].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Plasma", "Te-129m/Blood1" });
            CollectionAssert.AreEquivalent(organs[4].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "I-129/Decay-Sb-129/Plasma" });
            CollectionAssert.AreEquivalent(organs[5].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Te-129m/Blood1", "Te-129/Blood1" });
        }
    }
}
