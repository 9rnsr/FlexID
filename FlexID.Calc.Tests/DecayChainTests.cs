using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FlexID.Calc.Tests
{
    using static InputErrorTestHelpers;

    [TestClass]
    public class DecayChainTests
    {
        // Sb-129 -+-> Te-129m -+-+-> Te-129 -+-> I-129 --> (Xe-129)
        //          \            X           /
        //           +----------+ +---------+

        private static string[] GetInflows(Organ organ) => organ.Inflows.Select(i => i.Organ.ToString()).ToArray();

        /// <summary>
        /// 直接的・間接的な壊変経路の設定を混合した場合でも、系列内の壊変経路が正しく接続されることを確認する。
        /// </summary>
        /// <param name="reader"></param>
        [TestMethod]
        [DynamicData(nameof(MixingDecayPathSettings_Cases))]
        public void MixingDecayPathSettings(InputDataReader_OIR reader)
        {
            var data = reader.Read();
            var organs = data.Organs;
            Assert.IsTrue(organs.Count == 5);

            Assert.IsTrue(organs[0].ToString() == "Sb-129/input");
            Assert.IsTrue(organs[1].ToString() == "Sb-129/Blood");
            Assert.IsTrue(organs[2].ToString() == "Te-129m/Blood");
            Assert.IsTrue(organs[3].ToString() == "Te-129/Blood");
            Assert.IsTrue(organs[4].ToString() == "I-129/Blood");

            // DecayChain内の核種をインプットで定義したコンパートメントで網羅するよう設定したので、
            // DecayCompartmentは定義されない。

            CollectionAssert.AreEquivalent(organs[1].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/input" });
            CollectionAssert.AreEquivalent(organs[2].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Blood" });
            CollectionAssert.AreEquivalent(organs[3].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Blood", "Te-129m/Blood" });
            CollectionAssert.AreEquivalent(organs[4].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Te-129m/Blood", "Te-129/Blood" });
        }

        public static IEnumerable<object[]> MixingDecayPathSettings_Cases()
        {
            // 系列内で1つ以上の核種を暗黙に通過する、間接的な壊変経路の定義を使用する。
            yield return new object[]
            {
                CreateReader(new[]
                {
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                    "[Sb-129:compartment]",  "inp  input  ---",
                                             "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input         Blood  100%",
                    "[Te-129m:transfer]", "Sb-129/Blood  Blood  ---",   // 親－娘の壊変経路を定義する。
                    "[Te-129:transfer]",  "Sb-129/Blood  Blood  ---",   // 間接的な壊変経路を定義する。
                    "[I-129:transfer]",   "Sb-129/Blood  Blood  ---",   // 間接的な壊変経路を定義する。
                }),
            };

            // 親－娘の壊変経路だけを指定する。
            yield return new object[]
            {
                CreateReader(new[]
                {
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                    "[Sb-129:compartment]",  "inp  input  ---",
                                             "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input          Blood  100%",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",   // 親－娘の壊変経路。
                    "[Te-129:transfer]",  "Sb-129/Blood   Blood  ---",   // 親－娘の壊変経路。
                                          "Te-129m/Blood  Blood  ---",   // 親－娘の壊変経路。
                    "[I-129:transfer]",   "Te-129m/Blood  Blood  ---",   // 親－娘の壊変経路。
                                          "Te-129/Blood   Blood  ---",   // 親－娘の壊変経路。
                }),
            };

            // 先に定義した間接的な経路の一部を、後から直接的な経路で明示する。
            yield return new object[]
            {
                CreateReader(new[]
                {
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                    "[Sb-129:compartment]",  "inp  input  ---",
                                             "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input          Blood  100%",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",  // 親－娘の壊変経路。
                    "[Te-129:transfer]",  "Sb-129/Blood   Blood  ---",  // 先に系列を作る。
                                          "Te-129m/Blood  Blood  ---",  // 後で系列内の親－娘の壊変経路を明示している。
                    "[I-129:transfer]",   "Sb-129/Blood   Blood  ---",  // 先に系列を作る。
                                          "Te-129m/Blood  Blood  ---",  // 後で系列内の親－娘の壊変経路を明示している。
                                          "Te-129/Blood   Blood  ---",  // 後で系列内の親－娘の壊変経路を明示している。
                }),
            };

            // 先に定義した直接的な経路を包含する間接的な経路を後から定義する。
            yield return new object[]
            {
                CreateReader(new[]
                {
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                    "[Sb-129:compartment]",  "inp  input  ---",
                                             "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input          Blood  100%",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",  // 親－娘の壊変経路。
                    "[Te-129:transfer]",
                                          "Te-129m/Blood  Blood  ---",  // 先に系列内の親－娘の壊変経路を明示している。
                                          "Sb-129/Blood   Blood  ---",  // 後で系列を作る。
                    "[I-129:transfer]",
                                          "Te-129m/Blood  Blood  ---",  // 先に系列内の親－娘の壊変経路を明示している。
                                          "Te-129/Blood   Blood  ---",  // 先に系列内の親－娘の壊変経路を明示している。
                                          "Sb-129/Blood   Blood  ---",  // 後で系列を作る。
                }),
            };

            // 先に定義した直接的な経路＋系列全体を包絡しない間接的な経路の組み合わせを設定する。
            yield return new object[]
            {
                CreateReader(new[]
                {
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                    "[Sb-129:compartment]",  "inp  input  ---",
                                             "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input          Blood  100%",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",  // 親－娘の壊変経路。
                    "[Te-129:transfer]",
                                          "Te-129m/Blood  Blood  ---",  // 先に系列内の親－娘の壊変経路を明示している。
                                          "Sb-129/Blood   Blood  ---",  // 後で系列を作る。
                    "[I-129:transfer]",
                                          "Te-129m/Blood  Blood  ---",  // Sb-129から始まる系列全体を包絡しない間接的な経路。
                }),
            };
        }

        /// <summary>
        /// 移行速度付き壊変経路を設定する際の開始核種によらず、系列内の壊変経路が正しく接続されることを確認する。
        /// </summary>
        /// <param name="reader"></param>
        [TestMethod]
        [DynamicData(nameof(AnywhereStartDecayPathWithCoeff_Cases))]
        public void AnyStartDecayPathWithCoeff(InputDataReader_OIR reader)
        {
            var data = reader.Read();
            var organs = data.Organs;
            Assert.IsTrue(organs.Count == 6);

            Assert.IsTrue(organs[0].ToString() == "Sb-129/input");
            Assert.IsTrue(organs[1].ToString() == "Sb-129/Blood");
            Assert.IsTrue(organs[2].ToString() == "Te-129m/Blood");
            Assert.IsTrue(organs[3].ToString() == "Te-129/Blood");
            Assert.IsTrue(organs[4].ToString() == "I-129/Blood");

            // DecayCompartmentは同じDecayChainに属するよう生成される。
            Assert.IsTrue(organs[5].ToString() == "I-129/<decay from Sb-129/Blood>");
            Assert.IsTrue(organs[5].IsDecayCompartment);

            CollectionAssert.AreEquivalent(organs[1].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/input" });
            CollectionAssert.AreEquivalent(organs[2].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Blood" });
            CollectionAssert.AreEquivalent(organs[3].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Sb-129/Blood", "Te-129m/Blood" });
            CollectionAssert.AreEquivalent(organs[4].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "I-129/<decay from Sb-129/Blood>" });
            CollectionAssert.AreEquivalent(organs[5].Inflows.Select(i => i.Organ.ToString()).ToArray(), new[] { "Te-129m/Blood", "Te-129/Blood" });
        }

        public static IEnumerable<object[]> AnywhereStartDecayPathWithCoeff_Cases()
        {
            yield return new object[]
            {
                CreateReader(new[]
                {
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                    "[Sb-129:compartment]",  "inp  input  ---",
                                             "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input         Blood  100%",
                    "[Te-129m:transfer]", "Sb-129/Blood  Blood  ---",
                    "[Te-129:transfer]",  "Sb-129/Blood  Blood  ---",
                    "[I-129:transfer]",   "Sb-129/Blood  Blood  100",   // 移行速度付きの壊変経路を、系列先頭の核種Sb-129から設定。
                }),
            };

            yield return new object[]
            {
                CreateReader(new[]
                {
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                    "[Sb-129:compartment]",   "inp  input  ---",
                                              "acc  Blood  ---",
                    "[Te-129m:compartment]",  "acc  Blood  ---",
                    "[Te-129:compartment]",   "acc  Blood  ---",
                    "[I-129:compartment]",    "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input          Blood  100%",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",
                    "[Te-129:transfer]",  "Sb-129/Blood   Blood  ---",
                    "[I-129:transfer]",   "Te-129m/Blood  Blood  100",   // 移行速度付きの壊変経路を、系列途中の核種Te-129mから設定。
                }),
            };

            yield return new object[]
            {
                CreateReader(new[]
                {
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                    "[Sb-129:compartment]",   "inp  input  ---",
                                              "acc  Blood  ---",
                    "[Te-129m:compartment]",  "acc  Blood  ---",
                    "[Te-129:compartment]",   "acc  Blood  ---",
                    "[I-129:compartment]",    "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input          Blood  100%",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",
                    "[Te-129:transfer]",  "Sb-129/Blood   Blood  ---",
                    "[I-129:transfer]",   "Te-129/Blood   Blood  100",   // 移行速度付きの壊変経路を、系列途中の核種Te-129から設定。
                }),
            };
        }

        /// <summary>
        /// DecayChainで表現される壊変経路の系列が1つだけの場合において、
        /// 暗黙に生成されるDecayCompartmentの作成処理をテストする。
        /// </summary>
        /// <param name="reader"></param>
        [TestMethod]
        public void OneDecayChainTest()
        {
            var reader = CreateReader(new[]
            {
                "[title]", "dummy",
                "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                "[Sb-129:compartment]",  "inp  input  ---",
                                            "acc  Blood  ---",
                "[Te-129m:compartment]", "acc  Blood  ---",
                "[Te-129:compartment]",  "acc  Blood  ---",
                "[I-129:compartment]",   "acc  Blood  ---",

                "[Sb-129:transfer]",  "input         Blood   100%",
                "[Te-129m:transfer]", "Sb-129/Blood  Blood    10",  // 系列の起点に同じSb-129/Bloodを設定。
                "[Te-129:transfer]",  "Sb-129/Blood  Blood   100",  // 同上。
                "[I-129:transfer]",   "Sb-129/Blood  Blood  1000",  // 同上。
            });

            var data = reader.Read();
            var organs = data.Organs;
            Assert.IsTrue(organs.Count == 8);

            Assert.IsTrue(organs[0].ToString() == "Sb-129/input");
            Assert.IsTrue(organs[1].ToString() == "Sb-129/Blood");
            Assert.IsTrue(organs[2].ToString() == "Te-129m/Blood");
            Assert.IsTrue(organs[3].ToString() == "Te-129/Blood");
            Assert.IsTrue(organs[4].ToString() == "I-129/Blood");

            // DecayChainは、Sb-129/Bloodから始まる崩壊系列の1つが作成される。

            // 移行速度付きの壊変経路で、移行後の核種を受けるambiguous compartmentとして先行生成されたもの。
            Assert.IsTrue(organs[5].ToString() == "Te-129m/<decay from Sb-129/Blood>");
            Assert.IsTrue(organs[6].ToString() == "Te-129/<decay from Sb-129/Blood>");
            Assert.IsTrue(organs[7].ToString() == "I-129/<decay from Sb-129/Blood>");
            Assert.IsTrue(organs[5].IsDecayCompartment);
            Assert.IsTrue(organs[6].IsDecayCompartment);
            Assert.IsTrue(organs[7].IsDecayCompartment);

            // 崩壊系列を補完するambiguous compartmentとしてDefineDecayTransfersにおいて生成されたものは無い。

            CollectionAssert.AreEquivalent(GetInflows(organs[1]), new[] { "Sb-129/input" });
            CollectionAssert.AreEquivalent(GetInflows(organs[2]), new[] { "Te-129m/<decay from Sb-129/Blood>" });
            CollectionAssert.AreEquivalent(GetInflows(organs[3]), new[] { "Te-129/<decay from Sb-129/Blood>" });
            CollectionAssert.AreEquivalent(GetInflows(organs[4]), new[] { "I-129/<decay from Sb-129/Blood>" });

            CollectionAssert.AreEquivalent(GetInflows(organs[5]), new[] { "Sb-129/Blood", });
            CollectionAssert.AreEquivalent(GetInflows(organs[6]), new[] { "Sb-129/Blood", "Te-129m/<decay from Sb-129/Blood>" });
            CollectionAssert.AreEquivalent(GetInflows(organs[7]), new[] { "Te-129m/<decay from Sb-129/Blood>", "Te-129/<decay from Sb-129/Blood>" });
        }

        /// <summary>
        /// DecayChainで表現される壊変経路の系列が2つある場合において、
        /// 暗黙に生成されるDecayCompartmentの作成処理をテストする。
        /// </summary>
        /// <param name="reader"></param>
        [TestMethod]
        public void TwoDecayChainsTest()
        {
            var reader = CreateReader(new[]
            {
                "[title]", "dummy",
                "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",

                "[Sb-129:compartment]",  "inp  input  ---",
                                         "acc  Blood  ---",
                "[Te-129m:compartment]", "acc  Blood  ---",
                "[Te-129:compartment]",  "acc  Blood  ---",
                "[I-129:compartment]",   "acc  Blood  ---",

                "[Sb-129:transfer]",  "input          Sb-129/Blood   100%",
                "[Te-129m:transfer]", "Sb-129/Blood   Te-129m/Blood   10",  // 1つ目の系列の起点。
                "[Te-129:transfer]",  "Te-129m/Blood  Te-129/Blood   100",  // 2つ目の系列の起点。
                "[I-129:transfer]",   "Sb-129/Blood   I-129/Blood   1000",  // 1つ目の系列。
            });

            var data = reader.Read();
            var organs = data.Organs;
            Assert.IsTrue(organs.Count == 10);

            Assert.IsTrue(organs[0].ToString() == "Sb-129/input");
            Assert.IsTrue(organs[1].ToString() == "Sb-129/Blood");
            Assert.IsTrue(organs[2].ToString() == "Te-129m/Blood");
            Assert.IsTrue(organs[3].ToString() == "Te-129/Blood");
            Assert.IsTrue(organs[4].ToString() == "I-129/Blood");

            // DecayChainは、以下の2つが作成される。
            // - Sb-129/Blood から始まる崩壊系列
            // - Te-129m/Blood から始まる崩壊系列

            // 移行速度付きの壊変経路で、移行後の核種を受けるambiguous compartmentとして先行して生成されたもの。
            Assert.IsTrue(organs[5].ToString() == "Te-129m/<decay from Sb-129/Blood>");
            Assert.IsTrue(organs[6].ToString() == "Te-129/<decay from Te-129m/Blood>");
            Assert.IsTrue(organs[7].ToString() == "I-129/<decay from Sb-129/Blood>");
            Assert.IsTrue(organs[5].IsDecayCompartment);
            Assert.IsTrue(organs[6].IsDecayCompartment);
            Assert.IsTrue(organs[7].IsDecayCompartment);

            // 残りは、崩壊系列を補完するambiguous compartmentとしてDefineDecayTransfersにおいて生成されたもの。
            Assert.IsTrue(organs[8].ToString() == "Te-129/<decay from Sb-129/Blood>");
            Assert.IsTrue(organs[9].ToString() == "I-129/<decay from Te-129m/Blood>");
            Assert.IsTrue(organs[8].IsDecayCompartment);
            Assert.IsTrue(organs[9].IsDecayCompartment);

            CollectionAssert.AreEquivalent(GetInflows(organs[1]), new[] { "Sb-129/input" });
            CollectionAssert.AreEquivalent(GetInflows(organs[2]), new[] { "Te-129m/<decay from Sb-129/Blood>" });
            CollectionAssert.AreEquivalent(GetInflows(organs[3]), new[] { "Te-129/<decay from Te-129m/Blood>" });
            CollectionAssert.AreEquivalent(GetInflows(organs[4]), new[] { "I-129/<decay from Sb-129/Blood>" });

            CollectionAssert.AreEquivalent(GetInflows(organs[5]), new[] { "Sb-129/Blood", });
            CollectionAssert.AreEquivalent(GetInflows(organs[6]), new[] { "Te-129m/Blood" });
            CollectionAssert.AreEquivalent(GetInflows(organs[7]), new[] { "Te-129m/<decay from Sb-129/Blood>", "Te-129/<decay from Sb-129/Blood>" });

            CollectionAssert.AreEquivalent(GetInflows(organs[8]), new[] { "Sb-129/Blood", "Te-129m/<decay from Sb-129/Blood>" });
            CollectionAssert.AreEquivalent(GetInflows(organs[9]), new[] { "Te-129m/Blood", "Te-129/<decay from Te-129m/Blood>" });
        }

        [TestMethod]
        public void Test()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "Sr-90 Ingestion:Other",
                "",
                "[nuclide]",
                "  Sr-90  Y-90",
                "",
                "[Sr-90:compartment]",
                "  inp   input   ---",
                "  acc   Blood   Blood",
                "  acc   ST0     Other",
                "",
                "[Sr-90:transfer]",
                "  input         Blood  100%",
                "  Blood         ST0      7.5",
                "",
                "[Y-90:compartment]",
                "  acc   Blood1  Blood",
                "  acc   ST0     Other",
                "",
                "[Y-90:transfer]",
                "  Sr-90/Blood   Blood1   ---",     // 直接の壊変経路(A)
                "  Sr-90/ST0     Blood1   0.23105", // 間接の壊変経路(B: Aとは関係ない)
            });

            var data = reader.Read();
        }

        [TestMethod]
        public void Test2()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "Sr-90 Ingestion:Other",
                "",
                "[nuclide]",
                "  Sr-90  Y-90",
                "",
                "[Sr-90:compartment]",
                "  inp   input           ---",
                "  acc   Exch-C-bone-V   C-bone-V",
                "  acc   Noch-C-bone-V   C-bone-V",
                "",
                "[Sr-90:transfer]",
                "  input  Exch-C-bone-V  50%",
                "  input  Noch-C-bone-V  50%",
                "",
                "[Y-90:compartment]",
                "  acc   C-bone-V   C-bone-V",
                "",
                "[Y-90:transfer]",
                "  Sr-90/Exch-C-bone-V   C-bone-V   ---",   // 壊変経路の合流
                "  Sr-90/Noch-C-bone-V   C-bone-V   ---",   // 壊変経路の合流
            });

            var data = reader.Read();

            // TODO: 2つのDecayChainが途中で1つの壊変移行先コンパートメントを共有することで合流した場合に、
            // そこで生成される子孫核種を落とす先も共有されている必要がある。
        }

        [TestMethod]
        public void Test3()
        {
            // Os-181 --> Re-181 --> W-181 --> (Ta-181)

            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "Os-181  Re-181  W-181",
                "",
                "[Os-181:compartment]",
                "  inp   input           ---",
                "  acc   Exch-C-bone-V   C-bone-V",
                "  acc   Noch-C-bone-V   C-bone-V",
                "",
                "[Os-181:transfer]",
                "  input  Exch-C-bone-V  50%",
                "  input  Noch-C-bone-V  50%",
                "",
                "[Re-181:compartment]",
                "  acc   C-bone-V   C-bone-V",
                "",
                "[Re-181:transfer]",
                "  Os-181/Exch-C-bone-V   C-bone-V   ---",   // 壊変経路の合流
                "  Os-181/Noch-C-bone-V   C-bone-V   ---",   // 壊変経路の合流
                "",
                "[W-181:compartment]",
                "  acc   Blood   Blood",
                "",
                "[W-181:transfer]",
                "  Re-181/C-bone-V        Blood    100",
            });

            var data = reader.Read();
            var organs = data.Organs;

            // TODO:
            // Os-181/Exch-C-bone-Vから落ちてきたW-181と
            // Os-181/Noch-C-bone-Vから落ちてきたW-181の それぞれを受けるW-181のDecayCompartmentが
            // 2つ生成されてしまう。ただしそのうちの1つ W-181/<decay from Os-181/Exch-C-bone-V> が
            // W-181/Blood への移行速度付き壊変経路に使用されて、残る1つは倍増したW-181を受けるだけになっている。
        }
    }
}
