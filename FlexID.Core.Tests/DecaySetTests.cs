namespace FlexID;

using static InputErrorTestHelpers;

[TestClass]
public class DecaySetTests
{
    /// <summary>
    /// N1 --> N2 --> N3 --> N4 --> (stable)
    /// </summary>
    public class StraightDecayChain
    {
        protected static Location L1 = new Location { LineNum = 1 };
        protected static Location L2 = new Location { LineNum = 2 };
        protected static Location L3 = new Location { LineNum = 3 };
        protected static Location L4 = new Location { LineNum = 4 };
        protected static Location L5 = new Location { LineNum = 5 };
        protected static Location L6 = new Location { LineNum = 6 };
        protected static Location L7 = new Location { LineNum = 7 };

        protected readonly NuclideData N1; protected readonly Organ C11, C12, C13, C14;
        protected readonly NuclideData N2; protected readonly Organ C21, C22;
        protected readonly NuclideData N3; protected readonly Organ C31, C32;
        protected readonly NuclideData N4; protected readonly Organ C41;

        protected readonly DecaySet decaySet;
        protected readonly InputErrors errors = new();

        public StraightDecayChain()
        {
            N1 = new NuclideData { Index = 0, Name = "N1", };
            N2 = new NuclideData { Index = 1, Name = "N2", IsProgeny = true, };
            N3 = new NuclideData { Index = 2, Name = "N3", IsProgeny = true, };
            N4 = new NuclideData { Index = 3, Name = "N4", IsProgeny = true, };
            N1.Branches = [(Daughter: N2, Fraction: 1.0)];
            N2.Branches = [(Daughter: N3, Fraction: 1.0)];
            N3.Branches = [(Daughter: N4, Fraction: 1.0)];
            N4.Branches = [];

            C11 = new Organ { Nuclide = N1, Name = "C11", Func = OrganFunc.acc, };
            C12 = new Organ { Nuclide = N1, Name = "C12", Func = OrganFunc.acc, };
            C13 = new Organ { Nuclide = N1, Name = "C13", Func = OrganFunc.acc, };
            C14 = new Organ { Nuclide = N1, Name = "C14", Func = OrganFunc.acc, };
            C21 = new Organ { Nuclide = N2, Name = "C21", Func = OrganFunc.acc, };
            C22 = new Organ { Nuclide = N2, Name = "C22", Func = OrganFunc.acc, };
            C31 = new Organ { Nuclide = N3, Name = "C31", Func = OrganFunc.acc, };
            C32 = new Organ { Nuclide = N3, Name = "C32", Func = OrganFunc.acc, };
            C41 = new Organ { Nuclide = N4, Name = "C41", Func = OrganFunc.acc, };
            C11.ToString().ShouldBe("N1/C11");
            C12.ToString().ShouldBe("N1/C12");
            C13.ToString().ShouldBe("N1/C13");
            C14.ToString().ShouldBe("N1/C14");
            C21.ToString().ShouldBe("N2/C21");
            C22.ToString().ShouldBe("N2/C22");
            C31.ToString().ShouldBe("N3/C31");
            C32.ToString().ShouldBe("N3/C32");
            C41.ToString().ShouldBe("N4/C41");

            decaySet = new DecaySet([N1, N2, N3, N4], errors);
        }
    }

    /// <summary>
    /// 単一、または並列な壊変経路の定義動作。
    /// </summary>
    [TestClass]
    public class BasicTests1 : StraightDecayChain
    {
        //      N1      N2
        // [L1] C11 --> C21
        // ================
        //      C11 --> C21
        [TestMethod]
        public void 単一の壊変経路_N1からN2_係数なし()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
            }
        }

        //      N1              N2
        // [L1] C11 -(coeff.)-> C21
        // ===========================
        //      C11 -(coeff.)-> d21
        //                      ↓
        //                    (coeff.)
        //                      ↓
        //                      C21
        [TestMethod]
        public void 単一の壊変経路_N1からN2_係数あり()
        {
            decaySet.AddDecayPath(L1, C11, C21, 1.23m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d21 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (1.23m, C21)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, d21));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[d21].ShouldBe([chain1]);
                decaySet.JointChains.ContainsKey(C21).ShouldBeFalse();
            }
        }

        //      N1    N2    N3
        // [L1] C11 ------> C31
        // ====================
        //      C11 ------> C31
        [TestMethod]
        public void 単一の壊変経路_N1からN3_係数なし()
        {
            decaySet.AddDecayPath(L1, C11, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe(default);
                chain1[N3].ShouldBe((L1, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }
        }

        //      N1    N2             N3
        // [L1] C11 ------(coeff.)-> C31
        // ================================
        //      C11 ------(coeff.)-> d31
        //                           ↓
        //                         (coeff.)
        //                           ↓
        //                           C31
        [TestMethod]
        public void 単一の壊変経路_N1からN3_係数あり()
        {
            decaySet.AddDecayPath(L1, C11, C31, 1.23m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (1.23m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe(default);
                chain1[N3].ShouldBe((L1, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain1]);
                decaySet.JointChains.ContainsKey(C31).ShouldBeFalse();
            }
        }

        //      N1      N2
        // [L1] C11 --> C21
        // [L2] C12 --> C22
        // ================
        //      C11 --> C21
        //      C12 --> C22
        [TestMethod]
        public void 並列の壊変経路_N1からN2_係数なし()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C12, C22, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe((L2, C22));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C22].ShouldBe([chain2]);
            }
        }

        //      N1              N2
        // [L1] C11 ----------> C21
        // [L2] C12 -(coeff.)-> C21
        // ===========================
        //      C11 ----------> C21
        //                      ↑
        //                    (coeff.)
        //                      ↑
        //      C12 ----------> d21
        [TestMethod]
        public void 並列の壊変経路_N1からN2_係数あり()
        {
            // 2つの経路設定は、終端側がC21で合流しているように見えるが
            // 2つ目の経路は移行速度付きのため実際はN2位置で壊変コンパートメントが自動作成され、
            // 従ってこれらは独立した2つの壊変経路を作成する。
            decaySet.AddDecayPath(L1, C11, C21, null);   // 壊変経路1
            decaySet.AddDecayPath(L2, C12, C21, 123m);   // 壊変経路2: 経路1とは関係ない
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var d21 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C21)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe((L2, d21));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d21].ShouldBe([chain2]);
            }
        }

        //      N1               N2
        // [L1] C11 -(coeff.A)-> C21
        // [L2] C12 -(coeff.B)-> C21
        // ===========================
        //      C11 -----------> d21a
        //                       ↓
        //                     (coeff.A)
        //                       ↓
        //                       C21
        //                       ↑
        //                     (coeff.B)
        //                       ↑
        //      C12 -----------> d21b
        [TestMethod]
        public void 並列の壊変経路_N1からN2_係数あり_2nd()
        {
            // 2つの経路設定は、終端側がC21で合流しているように見えるが
            // それぞれの経路は移行速度付きのため実際はN2位置で壊変コンパートメントが自動作成され、
            // 従ってこれらは独立した2つの壊変経路を作成する。
            decaySet.AddDecayPath(L1, C11, C21, 123m);   // 壊変経路A
            decaySet.AddDecayPath(L2, C12, C21, 456m);   // 壊変経路B: 経路Aとは関係ない
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var d21a = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C21)).Key;
                var d21b = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (456m, C21)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, d21a));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe((L2, d21b));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[d21a].ShouldBe([chain1]);
                decaySet.JointChains[d21b].ShouldBe([chain2]);
                decaySet.JointChains.ContainsKey(C21).ShouldBeFalse();
            }
        }

        //      N1    N2    N3
        // [L1] C11 ------> C31
        // [L2] C12 ------> C32
        // ====================
        //      C11 ------> C31
        //      C12 ------> C32
        [TestMethod]
        public void 並列の壊変経路_N1からN3_係数なし()
        {
            decaySet.AddDecayPath(L1, C11, C31, null);
            decaySet.AddDecayPath(L2, C12, C32, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe(default);
                chain1[N3].ShouldBe((L1, C31));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe(default);
                chain2[N3].ShouldBe((L2, C32));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
                decaySet.JointChains[C32].ShouldBe([chain2]);
            }
        }

        //      N1    N2             N3
        // [L1] C11 ---------------> C31
        // [L2] C12 ------(coeff.)-> C31
        // ================================
        //      C11 ---------------> C31
        //                           ↑
        //                         (coeff.)
        //                           ↑
        //      C12 ---------------> d31
        [TestMethod]
        public void 並列の壊変経路_N1からN3_係数あり()
        {
            // 2つの経路設定は、終端側がC31で合流しているように見えるが
            // 2つ目の経路は移行速度付きのため実際はN3位置で壊変コンパートメントが自動作成され、
            // 従ってこれらは独立した2つの壊変経路を作成する。
            decaySet.AddDecayPath(L1, C11, C31, null);   // 壊変経路1
            decaySet.AddDecayPath(L2, C12, C31, 123m);   // 壊変経路2: 経路1とは関係ない
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe(default);
                chain1[N3].ShouldBe((L1, C31));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe(default);
                chain2[N3].ShouldBe((L2, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain2]);
            }
        }
    }

    /// <summary>
    /// 壊変経路の接続、経路の穴埋め、合流と合流点以降での系列共有動作。
    /// </summary>
    [TestClass]
    public class BasicTests2 : StraightDecayChain
    {
        /// <summary>
        ///
        ///      N1      N2      N3
        /// [L1] C11 --> C21
        /// [L2]         C21 --> C31
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1N2とN2N3の経路接続_順序1()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C21, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///
        ///      N1      N2      N3
        /// [L1]         C21 --> C31
        /// [L2] C11 --> C21
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1N2とN2N3の経路接続_順序2()
        {
            decaySet.AddDecayPath(L1, C21, C31, null);
            decaySet.AddDecayPath(L2, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L2, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        /// 
        ///      N1      N2      N3
        /// [L1] C11 ----------> C31
        /// [L2] C11 --> C21
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1からN3への経路の穴埋め_順序1()
        {
            decaySet.AddDecayPath(L1, C11, C31, null);
            decaySet.AddDecayPath(L2, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L2, C21));
                chain1[N3].ShouldBe((L1, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        /// 
        ///      N1      N2      N3
        /// [L1] C11 --> C21
        /// [L2] C11 ----------> C31
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1からN3への経路の穴埋め_順序2()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C11, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///
        ///      N1      N2      N3
        /// [L1] C11 --> C21
        /// [L2]         C21 --> C31
        /// [L3] C11 ----------> C31
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1からN3への経路の穴埋め_順序3()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C21, C31, null);
            decaySet.AddDecayPath(L3, C11, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///
        ///      N1      N2      N3
        /// [L1]         C21 --> C31
        /// [L2] C11 --> C21
        /// [L3] C11 ----------> C31
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1からN3への経路の穴埋め_順序4()
        {
            decaySet.AddDecayPath(L1, C21, C31, null);
            decaySet.AddDecayPath(L2, C11, C21, null);
            decaySet.AddDecayPath(L3, C11, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L2, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///
        ///      N1      N2      N3
        /// [L1] C11 --> C21
        /// [L2] C11 ----------> C31
        /// [L3]         C21 --> C31
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1からN3への経路の穴埋め_順序5()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C11, C31, null);
            decaySet.AddDecayPath(L3, C21, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///
        ///      N1      N2      N3
        /// [L1]         C21 --> C31
        /// [L2] C11 ----------> C31
        /// [L3] C11 --> C21
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1からN3への経路の穴埋め_順序6()
        {
            decaySet.AddDecayPath(L1, C21, C31, null);
            decaySet.AddDecayPath(L2, C11, C31, null);
            decaySet.AddDecayPath(L3, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L3, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, C31));

                chain2[N1].ShouldBe((L2, C11));
                chain2[N2].ShouldBe((L3, C21));
                chain2[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }

        /// <summary>
        ///
        ///      N1      N2      N3
        /// [L1] C11 ----------> C31
        /// [L2] C11 --> C21
        /// [L3]         C21 --> C31
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1からN3への経路の穴埋め_順序7()
        {
            decaySet.AddDecayPath(L1, C11, C31, null);
            decaySet.AddDecayPath(L2, C11, C21, null);
            decaySet.AddDecayPath(L3, C21, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L2, C21));
                chain1[N3].ShouldBe((L1, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///      N1      N2      N3
        /// [L1] C11 ----------> C31
        /// [L2]         C21 --> C31
        /// [L3] C11 --> C21
        /// ========================
        ///      C11 --> C21 --> C31
        /// </summary>
        [TestMethod]
        public void N1からN3への経路の穴埋め_順序8()
        {
            decaySet.AddDecayPath(L1, C11, C31, null);
            decaySet.AddDecayPath(L2, C21, C31, null);
            decaySet.AddDecayPath(L3, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L3, C21));
                chain1[N3].ShouldBe((L1, C31));

                chain2[N1].ShouldBe((L3, C11));
                chain2[N2].ShouldBe((L2, C21));
                chain2[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }

        //      N1         N2
        // [L1] C11 -----> C21
        // [L2] C12 -----> C21
        // ===================
        //      C11 ---+-> C21
        //            /
        //      C12 -+
        [TestMethod]
        public void N2での合流()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C12, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe((L2, C21));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }

        // FlexIDではこのような経路を直接作ることはできない。
        // 代わりに、d21をd21a,d21bの2つに分けることで同等の経路にする。
        // //      N1              N2
        // // [L1] C11 -(coeff.)-> C21
        // // [L2] C12 -(coeff.)-> C21
        // // ========================
        // //      C11 ---+------> d21
        // //            /         ↓
        // //      C12 -+        (coeff.)
        // //                      ↓
        // //                      C21
        // [TestMethod]
        // public void N2での合流_係数あり()

        /// <summary>
        /// 核種N2で合流が発生した場合、その子孫核種であるN3のコンパートメントは
        /// 自動的に合流した2つの系列で共有される。
        /// 
        ///      N1         N2      N3
        /// [L1] C11 -----> C21
        /// [L2] C12 -----> C21
        /// [L3]            C21 --> C31
        /// ===========================
        ///      C11 ---+-> C21 --> C31
        ///            /
        ///      C12 -+
        /// </summary>
        [TestMethod]
        public void N2での合流によるN3の共有_係数なし_パターン1()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C12, C21, null);
            decaySet.AddDecayPath(L3, C21, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L3, C31));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe((L2, C21));
                chain2[N3].ShouldBe((L3, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }

        /// <summary>
        ///      N1         N2      N3
        /// [L1] C11 -----> C21
        /// [L2]            C21 --> C31
        /// [L3] C12 -----> C21
        /// ===========================
        ///      C11 ---+-> C21 --> C31
        ///            /
        ///      C12 -+
        /// </summary>
        [TestMethod]
        public void N2での合流によるN3の共有_係数なし_パターン2()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C21, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }

            // chain1に対してN2位置で合流するchain2を追加する。
            decaySet.AddDecayPath(L3, C12, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, C31));

                chain2[N1].ShouldBe((L3, C12));
                chain2[N2].ShouldBe((L3, C21));
                chain2[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }

        /// <summary>
        ///      N1         N2      N3
        /// [L1]            C21 --> C31
        /// [L2] C11 -----> C21
        /// [L3] C12 -----> C21
        /// ===========================
        ///      C11 ---+-> C21 --> C31
        ///            /
        ///      C12 -+
        /// </summary>
        [TestMethod]
        public void N2での合流によるN3の共有_係数なし_パターン3()
        {
            decaySet.AddDecayPath(L1, C21, C31, null);
            decaySet.AddDecayPath(L2, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L2, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }

            // chain1に対してN2位置で合流するchain2を追加する。
            decaySet.AddDecayPath(L3, C12, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L2, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, C31));

                chain2[N1].ShouldBe((L3, C12));
                chain2[N2].ShouldBe((L3, C21));
                chain2[N3].ShouldBe((L1, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }

        /// <summary>
        /// 合流が発生した核種N2の子孫核種N3の位置が壊変コンパートメントである場合も、
        /// 自動的に合流した2つの系列で共有される。
        /// 
        ///      N1           N2              N3
        /// [L1] C11 -------> C21
        /// [L2] C12 -------> C21
        /// [L3]              C21 -(coeff.)-> C31
        /// ========================================
        ///      C11 -----+-> C21 ----------> d31
        ///              /                    ↓
        ///             /                   (coeff.)
        ///            /                      ↓
        ///      C12 -+                       C31
        /// </summary>
        [TestMethod]
        public void N2での合流によるN3の共有_係数あり_パターン1()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C12, C21, null);
            decaySet.AddDecayPath(L3, C21, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L3, d31));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe((L2, C21));
                chain2[N3].ShouldBe((L3, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[d31].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains.ContainsKey(C31).ShouldBeFalse();
            }
        }

        /// <summary>
        ///      N1           N2              N3
        /// [L1] C11 -------> C21
        /// [L2]              C21 -(coeff.)-> C31
        /// [L3] C12 -------> C21
        /// ========================================
        ///      C11 -----+-> C21 ----------> d31
        ///              /                    ↓
        ///             /                   (coeff.)
        ///            /                      ↓
        ///      C12 -+                       C31
        /// </summary>
        [TestMethod]
        public void N2での合流によるN3の共有_係数あり_パターン2()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C21, C31, 123m);
            decaySet.AddDecayPath(L3, C12, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, d31));

                chain2[N1].ShouldBe((L3, C12));
                chain2[N2].ShouldBe((L3, C21));
                chain2[N3].ShouldBe((L2, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[d31].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains.ContainsKey(C31).ShouldBeFalse();
            }
        }

        /// <summary>
        ///      N1           N2              N3
        /// [L1]              C21 -(coeff.)-> C31
        /// [L2] C11 -------> C21
        /// [L3] C12 -------> C21
        /// ========================================
        ///      C11 -----+-> C21 ----------> d31
        ///              /                    ↓
        ///             /                   (coeff.)
        ///            /                      ↓
        ///      C12 -+                       C31
        /// </summary>
        [TestMethod]
        public void N2での合流によるN3の共有_係数あり_パターン3()
        {
            decaySet.AddDecayPath(L1, C21, C31, 123m);
            decaySet.AddDecayPath(L2, C11, C21, null);
            decaySet.AddDecayPath(L3, C12, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L2, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, d31));

                chain2[N1].ShouldBe((L3, C12));
                chain2[N2].ShouldBe((L3, C21));
                chain2[N3].ShouldBe((L1, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[d31].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains.ContainsKey(C31).ShouldBeFalse();
            }
        }

        /// <summary>
        /// 
        ///      N1   N2         N3
        /// [L1] C11 ----------> C31
        /// [L2]      C21 -----> C31
        /// ========================
        ///      C11 -( )----+-> C31
        ///                 /
        ///           C21 -+
        /// </summary>
        [TestMethod]
        public void N3での合流_順序1()
        {
            decaySet.AddDecayPath(L1, C11, C31, null);
            decaySet.AddDecayPath(L2, C21, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe(default);
                chain1[N3].ShouldBe((L1, C31));

                chain2[N1].ShouldBe(default);
                chain2[N2].ShouldBe((L2, C21));
                chain2[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain2]);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }

        /// <summary>
        /// 
        ///      N1   N2        N3
        /// [L1]      C21 ----> C31
        /// [L2] C11 ---------> C31
        /// =======================
        ///           C21 --+-> C31
        ///                /
        ///      C11 -( )-+
        /// </summary>
        [TestMethod]
        public void N3での合流_順序2()
        {
            decaySet.AddDecayPath(L1, C21, C31, null);
            decaySet.AddDecayPath(L2, C11, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe(default);
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, C31));

                chain2[N1].ShouldBe((L2, C11));
                chain2[N2].ShouldBe(default);
                chain2[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }
    }

    /// <summary>
    /// 同じ移行先への速度付き壊変経路が、経路の定義順序に関わらずマージされる動作。
    /// </summary>
    [TestClass]
    public class BasicTests3 : StraightDecayChain
    {
        /// <summary>
        ///      N1      N2              N3
        /// [L1] C11 --> C21
        /// [L2]         C21 -(coeff.)-> C31
        /// [L3] C11 ---------(coeff.)-> C31
        /// ========================
        ///      C11 --> C21 ----------> d31
        //                               ↓
        //                             (coeff.)
        //                               ↓
        ///                              C31
        /// </summary>
        [TestMethod]
        public void N3への係数付き経路のマージ_パターン1()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C21, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain1]);
            }

            decaySet.AddDecayPath(L3, C11, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///      N1      N2              N3
        /// [L1] C11 --> C21
        /// [L2] C11 ---------(coeff.)-> C31
        /// [L3]         C21 -(coeff.)-> C31
        /// ========================
        ///      C11 --> C21 ----------> d31
        //                               ↓
        //                             (coeff.)
        //                               ↓
        ///                              C31
        /// </summary>
        [TestMethod]
        public void N3への係数付き経路のマージ_パターン2()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C11, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain1]);
            }

            decaySet.AddDecayPath(L3, C21, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///      N1      N2              N3
        /// [L1]         C21 -(coeff.)-> C31
        /// [L2] C11 --> C21
        /// [L3] C11 ---------(coeff.)-> C31
        /// ========================
        ///      C11 --> C21 ----------> d31
        //                               ↓
        //                             (coeff.)
        //                               ↓
        ///                              C31
        /// </summary>
        [TestMethod]
        public void N3への係数付き経路のマージ_パターン3()
        {
            decaySet.AddDecayPath(L1, C21, C31, 123m);
            decaySet.AddDecayPath(L2, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L2, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain1]);
            }

            decaySet.AddDecayPath(L3, C11, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L2, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///      N1      N2              N3
        /// [L1]         C21 -(coeff.)-> C31
        /// [L2] C11 ---------(coeff.)-> C31
        /// [L3] C11 --> C21
        /// ========================
        ///      C11 --> C21 ----------> d31
        //                               ↓
        //                             (coeff.)
        //                               ↓
        ///                              C31
        /// </summary>
        [TestMethod]
        public void N3への係数付き経路のマージ_パターン4()
        {
            decaySet.AddDecayPath(L1, C21, C31, 123m);
            decaySet.AddDecayPath(L2, C11, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var d31a = chain1[N3].Organ.ShouldNotBeNull();
                var d31b = chain2[N3].Organ.ShouldNotBeNull();
                decaySet.AfterDecayPath.Keys.ShouldBe([d31a, d31b], ignoreOrder: true);

                chain1[N1].ShouldBe(default);
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, d31a));

                chain2[N1].ShouldBe((L2, C11));
                chain2[N2].ShouldBe(default);
                chain2[N3].ShouldBe((L2, d31b));

                decaySet.JointChains[C11].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d31a].ShouldBe([chain1]);
                decaySet.JointChains[d31b].ShouldBe([chain2]);
            }

            decaySet.AddDecayPath(L3, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                // d31a, d31bは1つにマージされる。
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;
                decaySet.AfterDecayPath.Keys.ShouldBe([d31]);

                chain1[N1].ShouldBe((L3, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L1, d31));

                chain2[N1].ShouldBe((L2, C11));
                chain2[N2].ShouldBe((L3, C21));
                chain2[N3].ShouldBe((L2, d31));

                decaySet.JointChains[C11].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[d31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }

        /// <summary>
        ///      N1      N2              N3
        /// [L1] C11 ---------(coeff.)-> C31
        /// [L2] C11 --> C21
        /// [L3]         C21 -(coeff.)-> C31
        /// ========================
        ///      C11 --> C21 ----------> d31
        //                               ↓
        //                             (coeff.)
        //                               ↓
        ///                              C31
        /// </summary>
        [TestMethod]
        public void N3への係数付き経路のマージ_パターン5()
        {
            decaySet.AddDecayPath(L1, C11, C31, 123m);
            decaySet.AddDecayPath(L2, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L2, C21));
                chain1[N3].ShouldBe((L1, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain1]);
            }

            decaySet.AddDecayPath(L3, C21, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L2, C21));
                chain1[N3].ShouldBe((L1, d31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[d31].ShouldBe([chain1]);
            }
        }

        /// <summary>
        ///      N1      N2              N3
        /// [L1] C11 ---------(coeff.)-> C31
        /// [L2]         C21 -(coeff.)-> C31
        /// [L3] C11 --> C21
        /// ========================
        ///      C11 --> C21 ----------> d31
        //                               ↓
        //                             (coeff.)
        //                               ↓
        ///                              C31
        /// </summary>
        [TestMethod]
        public void N3への係数付き経路のマージ_パターン6()
        {
            decaySet.AddDecayPath(L1, C11, C31, 123m);
            decaySet.AddDecayPath(L2, C21, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                var d31a = chain1[N3].Organ.ShouldNotBeNull();
                var d31b = chain2[N3].Organ.ShouldNotBeNull();
                decaySet.AfterDecayPath.Keys.ShouldBe([d31a, d31b], ignoreOrder: true);

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe(default);
                chain1[N3].ShouldBe((L1, d31a));

                chain2[N1].ShouldBe(default);
                chain2[N2].ShouldBe((L2, C21));
                chain2[N3].ShouldBe((L2, d31b));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain2]);
                decaySet.JointChains[d31a].ShouldBe([chain1]);
                decaySet.JointChains[d31b].ShouldBe([chain2]);
            }

            decaySet.AddDecayPath(L3, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                // d31a, d31bは1つにマージされる。
                var d31 = decaySet.AfterDecayPath.First(p => p.Value.Outflow == (123m, C31)).Key;
                decaySet.AfterDecayPath.Keys.ShouldBe([d31]);

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L3, C21));
                chain1[N3].ShouldBe((L1, d31));

                chain2[N1].ShouldBe((L3, C11));
                chain2[N2].ShouldBe((L2, C21));
                chain2[N3].ShouldBe((L2, d31));

                decaySet.JointChains[C11].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[d31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }
    }

    [TestClass]
    public class BasicTests4 : StraightDecayChain
    {
        /// <summary>
        /// 核種N2で合流が発生した場合、その子孫核種であるN3,N4のコンパートメントは
        /// 自動的に合流した2つの系列で共有される。
        /// 
        ///      N1           N2      N3      N4
        /// [L1] C11 -------> C21
        /// [L2]              C21 --> C31
        /// [L3]                      C31 --> C41
        /// [L4] C12 -------> C21
        /// =====================================
        ///      C11 -----+-> C21 --> C31 --> C41
        ///              /
        ///             /
        ///            /
        ///      C12 -+
        /// </summary>
        [TestMethod]
        public void N2での合流によるN3とN4の共有()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C21, C31, null);
            decaySet.AddDecayPath(L3, C31, C41, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, C31));
                chain1[N4].ShouldBe((L3, C41));
            }

            decaySet.AddDecayPath(L4, C12, C21, null);
            {
                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain2[N1].ShouldBe((L4, C12));
                chain2[N2].ShouldBe((L4, C21));
                chain2[N3].ShouldBe((L2, C31));
                chain2[N4].ShouldBe((L3, C41));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C41].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }

        /// <summary>
        /// 
        ///
        ///      N1          N2           N3      N4
        /// [L1] C11 ------> C21
        /// [L2] C12 ------> C21
        /// [L3]             C21 -------> C31
        /// [L4]                          C31 --> C41
        /// [L5] C13 ------> C22
        /// [L6] C14 ------> C22
        /// [L7]             C22 -------> C31
        /// =========================================
        ///      C11 ---+--> C21 -----+-> C31 --> C41
        ///            /             /
        ///      C12 -+             /
        ///                        /
        ///      C13 ---+--> C22 -+
        ///            /
        ///      C14 -+
        /// </summary>
        [TestMethod]
        public void N2での合流2つとN3での合流1つとN4の共有()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C12, C21, null);
            decaySet.AddDecayPath(L3, C21, C31, null);
            decaySet.AddDecayPath(L4, C31, C41, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L3, C31));
                chain1[N4].ShouldBe((L4, C41));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe((L2, C21));
                chain2[N3].ShouldBe((L3, C31));
                chain2[N4].ShouldBe((L4, C41));
            }

            decaySet.AddDecayPath(L5, C13, C22, null);
            decaySet.AddDecayPath(L6, C14, C22, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(4);
                var chain3 = decaySet.DecayChains[2];
                var chain4 = decaySet.DecayChains[3];

                chain3[N1].ShouldBe((L5, C13));
                chain3[N2].ShouldBe((L5, C22));
                chain3[N3].ShouldBe(default);
                chain3[N4].ShouldBe(default);

                chain4[N1].ShouldBe((L6, C14));
                chain4[N2].ShouldBe((L6, C22));
                chain4[N3].ShouldBe(default);
                chain4[N4].ShouldBe(default);
            }
            decaySet.AddDecayPath(L7, C22, C31, null);
            {
                decaySet.DecayChains.Count.ShouldBe(4);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var chain3 = decaySet.DecayChains[2];
                var chain4 = decaySet.DecayChains[3];

                chain3[N3].ShouldBe((L7, C31));
                chain3[N4].ShouldBe((L4, C41));
                chain4[N3].ShouldBe((L7, C31));
                chain4[N4].ShouldBe((L4, C41));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C13].ShouldBe([chain3]);
                decaySet.JointChains[C14].ShouldBe([chain4]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C22].ShouldBe([chain3, chain4], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2, chain3, chain4], ignoreOrder: true);
                decaySet.JointChains[C41].ShouldBe([chain1, chain2, chain3, chain4], ignoreOrder: true);
            }
        }

        /// <summary>
        /// 
        ///      N1          N2
        /// [L1] C11 ------> C21
        /// [L2] C12 ------> C21
        /// [L3] C13 ------> C22
        /// [L4] C14 ------> C22
        /// [L5]             C21 -------> C31
        /// [L6]             C22 -------> C31
        /// =================================
        ///      C11 ---+--> C21 -----+-> C31
        ///            /             /
        ///      C12 -+             /
        ///                        /
        ///      C13 ---+--> C22 -+
        ///            /
        ///      C14 -+
        /// </summary>
        [TestMethod]
        public void N2での合流2つとN3での合流_順序1()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C12, C21, null);
            decaySet.AddDecayPath(L3, C13, C22, null);
            decaySet.AddDecayPath(L4, C14, C22, null);
            {
                decaySet.DecayChains.Count.ShouldBe(4);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var chain3 = decaySet.DecayChains[2];
                var chain4 = decaySet.DecayChains[3];

                chain1[N1].ShouldBe((L1, C11));
                chain2[N1].ShouldBe((L2, C12));
                chain3[N1].ShouldBe((L3, C13));
                chain4[N1].ShouldBe((L4, C14));
                chain1[N2].ShouldBe((L1, C21));
                chain2[N2].ShouldBe((L2, C21));
                chain3[N2].ShouldBe((L3, C22));
                chain4[N2].ShouldBe((L4, C22));

                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C22].ShouldBe([chain3, chain4], ignoreOrder: true);
            }

            decaySet.AddDecayPath(L5, C21, C31, null);
            decaySet.AddDecayPath(L6, C22, C31, null);
            {
                decaySet.DecayChains.Count.ShouldBe(4);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var chain3 = decaySet.DecayChains[2];
                var chain4 = decaySet.DecayChains[3];

                chain1[N1].ShouldBe((L1, C11));
                chain2[N1].ShouldBe((L2, C12));
                chain3[N1].ShouldBe((L3, C13));
                chain4[N1].ShouldBe((L4, C14));
                chain1[N2].ShouldBe((L1, C21));
                chain2[N2].ShouldBe((L2, C21));
                chain3[N2].ShouldBe((L3, C22));
                chain4[N2].ShouldBe((L4, C22));
                chain1[N3].ShouldBe((L5, C31));
                chain2[N3].ShouldBe((L5, C31));
                chain3[N3].ShouldBe((L6, C31));
                chain4[N3].ShouldBe((L6, C31));
                decaySet.JointChains[C31].ShouldBe([chain1, chain2, chain3, chain4], ignoreOrder: true);
            }
        }

        /// <summary>
        /// 
        ///      N1          N2           N3
        /// [L1] C11 ------> C21
        /// [L2] C12 ------> C21
        /// [L3] C13 ------> C22
        /// [L4] C14 ------> C22
        /// [L5] C11 -------------------> C31
        /// [L6] C14 -------------------> C31
        /// =================================
        ///      C11 ---+--> C21 -----+-> C31
        ///            /             /
        ///      C12 -+             /
        ///                        /
        ///      C13 ---+--> C22 -+
        ///            /
        ///      C14 -+
        /// </summary>
        [TestMethod]
        public void N2での合流2つとN3での合流_順序2()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C12, C21, null);
            decaySet.AddDecayPath(L3, C13, C22, null);
            decaySet.AddDecayPath(L4, C14, C22, null);
            {
                decaySet.DecayChains.Count.ShouldBe(4);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var chain3 = decaySet.DecayChains[2];
                var chain4 = decaySet.DecayChains[3];

                chain1[N1].ShouldBe((L1, C11));
                chain2[N1].ShouldBe((L2, C12));
                chain3[N1].ShouldBe((L3, C13));
                chain4[N1].ShouldBe((L4, C14));
                chain1[N2].ShouldBe((L1, C21));
                chain2[N2].ShouldBe((L2, C21));
                chain3[N2].ShouldBe((L3, C22));
                chain4[N2].ShouldBe((L4, C22));

                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C22].ShouldBe([chain3, chain4], ignoreOrder: true);
            }

            // L5はchain1に対して直接の関係を発見できるが、
            // chain2に対してはN3の位置がまだ共有されていないため、間接的な関係を見つけてマージする必要がある。
            // 同様の処理を、L6ではchain4に対して行う必要がある。
            decaySet.AddDecayPath(L5, C11, C31, null);
            decaySet.AddDecayPath(L6, C14, C31, null);
            {
                decaySet.DecayChains.Count.ShouldBe(4);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var chain3 = decaySet.DecayChains[2];
                var chain4 = decaySet.DecayChains[3];

                chain1[N1].ShouldBe((L1, C11));
                chain2[N1].ShouldBe((L2, C12));
                chain3[N1].ShouldBe((L3, C13));
                chain4[N1].ShouldBe((L4, C14));

                chain1[N2].ShouldBe((L1, C21));
                chain2[N2].ShouldBe((L2, C21));
                chain3[N2].ShouldBe((L3, C22));
                chain4[N2].ShouldBe((L4, C22));

                chain1[N3].ShouldBe((L5, C31));
                chain2[N3].ShouldBe((L5, C31));
                chain3[N3].ShouldBe((L6, C31));
                chain4[N3].ShouldBe((L6, C31));

                decaySet.JointChains[C31].ShouldBe([chain1, chain2, chain3, chain4], ignoreOrder: true);
            }
        }

        /// <summary>
        ///      N1         N2      N3      N4
        /// [L1] C11 ---------------------> C41
        /// [L2] C11 -----> C21
        /// [L3] C12 ---------------------> C41
        /// [L4] C12 -----> C21
        /// [L5] C11 -------------> C31
        /// ===================================
        ///      C11 ---+-> C21 --> C31 --> C41
        //             /
        //       C12 -+
        /// </summary>
        [TestMethod]
        public void N3への経路設定を契機にN2位置での合流経路を発見する()
        {
            decaySet.AddDecayPath(L1, C11, C41, null);
            decaySet.AddDecayPath(L2, C11, C21, null);
            decaySet.AddDecayPath(L3, C12, C41, null);
            decaySet.AddDecayPath(L4, C12, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L2, C21));
                chain1[N3].ShouldBe(default);
                chain1[N4].ShouldBe((L1, C41));

                chain2[N1].ShouldBe((L3, C12));
                chain2[N2].ShouldBe((L4, C21));
                chain2[N3].ShouldBe(default);
                chain2[N4].ShouldBe((L3, C41));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C41].ShouldBe([chain1, chain2], ignoreOrder: true);
            }

            // chain1(C11→C41)経路に対するN3位置へのC31設定を契機として
            // chain1とchain2(C12→C41)がN2位置のC21で合流していることを発見し、
            // (N4位置は既に共有済みのため)N2位置とN3位置それぞれでの共有設定を行う。
            decaySet.AddDecayPath(L5, C11, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L2, C21));
                chain1[N3].ShouldBe((L5, C31));
                chain1[N4].ShouldBe((L1, C41));

                chain2[N1].ShouldBe((L3, C12));
                chain2[N2].ShouldBe((L4, C21));
                chain2[N3].ShouldBe((L5, C31));
                chain2[N4].ShouldBe((L3, C41));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C41].ShouldBe([chain1, chain2], ignoreOrder: true);
            }
        }
    }

    [TestClass]
    public class ErrorTests : StraightDecayChain
    {
        /// <summary>
        /// 壊変経路の移行先が分岐する場合に対するエラー報告。
        ///
        ///      N1                     N2
        /// [L1] C11 -----(coeff1.?)--> C21
        /// [L2] C11 -----(coeff2.?)--> C22
        /// ===============================
        ///      C11 -+---(coeff1.?)--> C21
        ///            \ 
        ///             +-(coeff2.?)--> C22 (error)
        /// </summary>
        [TestMethod]
        [DataRow(null, null)]
        [DataRow(1.23, null)]
        [DataRow(null, 2.34)]
        [DataRow(1.23, 2.34)]
        public void N1からN2での分岐エラー(double? coeff1, double? coeff2)
        {
            decaySet.AddDecayPath(L1, C11, C21, (decimal?)coeff1);
            {
                errors.GetErrorLines().ShouldBe([]);
            }

            decaySet.AddDecayPath(L2, C11, C22, (decimal?)coeff2);
            {
                errors.GetErrorLines().ShouldBe(
                [
                    $"{L2}: Conflict of decay path definitions found:",
                    $"{L1}:     previous, decay to '{C21}' {(coeff1 is null ? "without coefficient" : $"with coefficient {coeff1.Value}")},",
                    $"{L2}:     and here, decay to '{C22}' {(coeff2 is null ? "without coefficient" : $"with coefficient {coeff2.Value}")}.",
                ]);
            }
        }

        /// <summary>
        /// 壊変経路の移行先が分岐する場合に対するエラー。
        /// 
        ///      N1         N2         N3
        /// [L1] C11 -----> C21
        /// [L2] C12 -----> C21
        /// [L3]            C21 -----> C31
        /// [L4] C12 ----------------> C32
        /// ==============================
        ///      C11 ---+-> C21 -+---> C31
        ///            /          \
        ///      C12 -+            +-> C32 (error)
        /// </summary>
        [TestMethod]
        public void N2での合流後にN3での分岐エラー()
        {
            // 核種N2で合流が発生した場合、その子孫核種であるN3のコンパートメントは
            // 自動的に合流した2つの系列で共有される。
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C12, C21, null);
            decaySet.AddDecayPath(L3, C21, C31, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L3, C31));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe((L2, C21));
                chain2[N3].ShouldBe((L3, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
            }

            // 新しい経路の末端側の位置(核種N3)で壊変経路の分岐が生じる。
            decaySet.AddDecayPath(L4, C12, C32, null);
            {
                errors.GetErrorLines().ShouldBe(
                [
                    $"{L4}: Conflict of decay path definitions found:",
                    $"{L3}:     previous, decay to '{C31}' without coefficient,",
                    $"{L4}:     and here, decay to '{C32}' without coefficient.",
                ]);
            }
        }

        /// <summary>
        ///      N1         N2         N3
        /// [L1] C11 ----------------> C31
        /// [L2] C12 ----------------> C32
        /// [L3] C11 -----> C21
        /// [L4] C12 -----> C21
        /// ==============================
        ///      C11 ---+-> C21 -+---> C31
        ///            /          \
        ///      C12 -+            +-> C32 (error)
        /// </summary>
        [TestMethod]
        public void N3への並列とN2での合流によるエラー()
        {
            // C11 -> C21 -> C31 と
            // C12 --------> C32 の並列な壊変経路を作る。
            decaySet.AddDecayPath(L1, C11, C31, null);
            decaySet.AddDecayPath(L2, C12, C32, null);
            decaySet.AddDecayPath(L3, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe([]);

                decaySet.DecayChains.Count.ShouldBe(2);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L3, C21));
                chain1[N3].ShouldBe((L1, C31));

                chain2[N1].ShouldBe((L2, C12));
                chain2[N2].ShouldBe(default);
                chain2[N3].ShouldBe((L2, C32));
            }

            // 新しい経路の末端側(核種N2)より子孫の位置(核種N3)で壊変経路の分岐が生じる。
            decaySet.AddDecayPath(L4, C12, C21, null);
            {
                errors.GetErrorLines().ShouldBe(
                [
                    $"{L4}: Conflict of decay path definitions found:",
                    $"{L1}:     previous,  decay to '{C31}' without coefficient,",
                    $"{L2}:     and there, decay to '{C32}' without coefficient.",
                ]);
            }
        }

        /// <summary>
        ///      N1      N2               N3
        /// [L1] C11 --> C21
        /// [L2]         C21 -(coeff.A)-> C31
        /// [L3] C11 ---------(coeff.B)-> C31
        /// =================================
        ///      C11 --> C21 -----------> d31
        //                                ↓
        //                      (coeff.A) vs (coeff.B) (error)
        //                                ↓
        ///                               C31
        /// </summary>
        [TestMethod]
        public void N3への係数付き経路のマージ_パターン1における係数不一致エラー()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C21, C31, 123m);
            {
                errors.GetErrorLines().ShouldBe([]);
            }

            decaySet.AddDecayPath(L3, C11, C31, 456m);
            {
                errors.GetErrorLines().ShouldBe(
                [
                    $"{L3}: Conflict of decay path definitions found:",
                    $"{L2}:     previous, decay to '{C31}' with coefficient 123,",
                    $"{L3}:     and here, decay to '{C31}' with coefficient 456.",
                ]);
            }
        }

        /// <summary>
        ///      N1      N2               N3
        /// [L1] C11 ---------(coeff.A)-> C31
        /// [L2]         C21 -(coeff.B)-> C31
        /// [L3] C11 --> C21
        /// =================================
        ///      C11 --> C21 -----------> d31
        //                                ↓
        //                      (coeff.A) vs (coeff.B) (error)
        //                                ↓
        ///                               C31
        /// </summary>
        [TestMethod]
        public void N3への係数付き経路のマージ_パターン6における係数不一致エラー()
        {
            decaySet.AddDecayPath(L1, C11, C31, 123m);
            decaySet.AddDecayPath(L2, C21, C31, 456m);
            {
                errors.GetErrorLines().ShouldBe([]);
            }

            decaySet.AddDecayPath(L3, C11, C21, null);
            {
                errors.GetErrorLines().ShouldBe(
                [
                    $"{L3}: Conflict of decay path definitions found:",
                    $"{L1}:     previous,  decay to '{C31}' with coefficient 123,",
                    $"{L2}:     and there, decay to '{C31}' with coefficient 456.",
                ]);
            }
        }
    }

    /// <summary>
    ///        +--> N2 --+
    ///       /           \
    /// N1 --+             +-> N4
    ///       \           /
    ///        +--> N3 --+
    /// </summary>
    [TestClass]
    public class DecayChainType_Diamond
    {
        static Location L1 = new Location { LineNum = 1 };
        static Location L2 = new Location { LineNum = 2 };
        static Location L3 = new Location { LineNum = 3 };
        static Location L4 = new Location { LineNum = 4 };
        static Location L5 = new Location { LineNum = 5 };

        readonly NuclideData N1; readonly Organ C11, C12, C13;
        readonly NuclideData N2; readonly Organ C21;
        readonly NuclideData N3; readonly Organ C31;
        readonly NuclideData N4; readonly Organ C41;

        readonly DecaySet decaySet;
        readonly InputErrors errors = new();

        public DecayChainType_Diamond()
        {
            N1 = new NuclideData { Index = 0, Name = "N1", };
            N2 = new NuclideData { Index = 1, Name = "N2", IsProgeny = true, };
            N3 = new NuclideData { Index = 2, Name = "N3", IsProgeny = true, };
            N4 = new NuclideData { Index = 3, Name = "N4", IsProgeny = true, };
            N1.Branches = [(Daughter: N2, Fraction: 0.5), (Daughter: N3, Fraction: 0.5)];
            N2.Branches = [(Daughter: N4, Fraction: 1.0)];
            N3.Branches = [(Daughter: N4, Fraction: 1.0)];
            N4.Branches = [];

            C11 = new Organ { Nuclide = N1, Name = "C11", Func = OrganFunc.acc, };
            C12 = new Organ { Nuclide = N1, Name = "C12", Func = OrganFunc.acc, };
            C13 = new Organ { Nuclide = N1, Name = "C13", Func = OrganFunc.acc, };
            C21 = new Organ { Nuclide = N2, Name = "C21", Func = OrganFunc.acc, };
            C31 = new Organ { Nuclide = N3, Name = "C31", Func = OrganFunc.acc, };
            C41 = new Organ { Nuclide = N4, Name = "C41", Func = OrganFunc.acc, };
            C11.ToString().ShouldBe("N1/C11");
            C12.ToString().ShouldBe("N1/C12");
            C13.ToString().ShouldBe("N1/C13");
            C21.ToString().ShouldBe("N2/C21");
            C31.ToString().ShouldBe("N3/C31");
            C41.ToString().ShouldBe("N4/C41");

            decaySet = new DecaySet([N1, N2, N3, N4], errors);
        }

        /// <summary>
        ///
        ///      N1            N2     N3          N4
        /// [L1] C11 --------> C21
        /// [L2] C11 ---------------> C31
        /// [L1] C12 --------> C21
        /// [L2] C13 ---------------> C31
        /// [L3] C11 ---------------------------> C41
        /// =========================================
        ///      C12 -----+--> C21 ---------+
        ///              /                   \
        ///      C11 ---+                     +-> C41
        ///              \                   /
        ///               +---------> C31 --+
        ///              /
        ///      C13 ---+
        /// </summary>
        [TestMethod]
        public void 間接的な壊変経路の設定が分岐合流を網羅する()
        {
            decaySet.AddDecayPath(L1, C11, C21, null);
            decaySet.AddDecayPath(L2, C11, C31, null);
            {
                decaySet.DecayChains.Count.ShouldBe(1);
                var chain1 = decaySet.DecayChains[0];

                chain1[N1].ShouldBe((L1, C11));
                chain1[N2].ShouldBe((L1, C21));
                chain1[N3].ShouldBe((L2, C31));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
                decaySet.JointChains[C31].ShouldBe([chain1]);
            }

            decaySet.AddDecayPath(L3, C12, C21, null);
            decaySet.AddDecayPath(L4, C13, C31, null);
            {
                decaySet.DecayChains.Count.ShouldBe(3);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var chain3 = decaySet.DecayChains[2];

                chain2[N1].ShouldBe((L3, C12));
                chain2[N2].ShouldBe((L3, C21));
                chain2[N3].ShouldBe(default);

                chain3[N1].ShouldBe((L4, C13));
                chain3[N2].ShouldBe(default);
                chain3[N3].ShouldBe((L4, C31));

                decaySet.JointChains[C12].ShouldBe([chain2]);
                decaySet.JointChains[C13].ShouldBe([chain3]);
                decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
                decaySet.JointChains[C31].ShouldBe([chain1, chain3], ignoreOrder: true);
            }

            // chain1の中でC41への経路を設定した時点で、これに加えて
            // C21に合流している崩壊系列chain2と
            // C31に合流している崩壊系列chain3も
            // C41を共有する崩壊系列として判定される。
            decaySet.AddDecayPath(L5, C11, C41, null);
            {
                decaySet.DecayChains.Count.ShouldBe(3);
                var chain1 = decaySet.DecayChains[0];
                var chain2 = decaySet.DecayChains[1];
                var chain3 = decaySet.DecayChains[2];

                chain1[N4].ShouldBe((L5, C41));
                chain2[N4].ShouldBe((L5, C41));
                chain3[N4].ShouldBe((L5, C41));

                decaySet.JointChains[C41].ShouldBe([chain1, chain2, chain3], ignoreOrder: true);
            }
        }
    }

    /// <summary>
    /// 直接的・間接的な壊変経路の設定を混合した場合でも、系列内の壊変経路が正しく接続されることを確認する。
    /// 
    /// N1 -+-> N2 -+-+-> N3 -+-> N4 --> (stable)
    ///      \       X       /
    ///       +-----+ +-----+
    /// </summary>
    [TestClass]
    public class DecayChainType_Ladder
    {
        readonly NuclideData N1; readonly Organ C1;
        readonly NuclideData N2; readonly Organ C2;
        readonly NuclideData N3; readonly Organ C3;
        readonly NuclideData N4; readonly Organ C4;

        readonly DecaySet decaySet;
        readonly InputErrors errors = new();

        public DecayChainType_Ladder()
        {
            N1 = new NuclideData { Index = 0, Name = "N1", };
            N2 = new NuclideData { Index = 1, Name = "N2", IsProgeny = true, };
            N3 = new NuclideData { Index = 2, Name = "N3", IsProgeny = true, };
            N4 = new NuclideData { Index = 3, Name = "N4", IsProgeny = true, };
            N1.Branches = [(Daughter: N2, Fraction: 0.5), (Daughter: N3, Fraction: 0.5)];
            N2.Branches = [(Daughter: N3, Fraction: 0.5), (Daughter: N4, Fraction: 0.5)];
            N3.Branches = [(Daughter: N4, Fraction: 0.5)];
            N4.Branches = [];

            C1 = new Organ { Nuclide = N1, Name = "C1", Func = OrganFunc.acc, };
            C2 = new Organ { Nuclide = N2, Name = "C2", Func = OrganFunc.acc, };
            C3 = new Organ { Nuclide = N3, Name = "C3", Func = OrganFunc.acc, };
            C4 = new Organ { Nuclide = N4, Name = "C4", Func = OrganFunc.acc, };
            C1.ToString().ShouldBe("N1/C1");
            C2.ToString().ShouldBe("N2/C2");
            C3.ToString().ShouldBe("N3/C3");
            C4.ToString().ShouldBe("N4/C4");

            decaySet = new DecaySet([N1, N2, N3, N4], errors);
        }

        /// <summary>
        /// 直接的・間接的な壊変経路の設定を混合した場合でも、系列内の壊変経路が正しく接続されることを確認する。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(MixingDecayPathSettings_Cases))]
        public void MixingDecayPathSettings(params (string From, string To)[] paths)
        {
            IReadOnlyList<Organ> compartments = [C1, C2, C3, C4];

            var loc = new Location { LineNum = 1 };
            foreach (var (from, to) in paths)
            {
                var organFrom = compartments.First(c => c.Name == from);
                var organTo = compartments.First(c => c.Name == to);
                decaySet.AddDecayPath(loc, organFrom, organTo, null);
                loc.LineNum++;
            }
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            chain1[N1].Organ.ShouldBe(C1);
            chain1[N2].Organ.ShouldBe(C2);
            chain1[N3].Organ.ShouldBe(C3);
            chain1[N4].Organ.ShouldBe(C4);

            decaySet.JointChains[C1].ShouldBe([chain1]);
            decaySet.JointChains[C2].ShouldBe([chain1]);
            decaySet.JointChains[C3].ShouldBe([chain1]);
            decaySet.JointChains[C4].ShouldBe([chain1]);
        }

        public static IEnumerable<object[]> MixingDecayPathSettings_Cases()
        {
            // 系列内で1つ以上の核種を暗黙に通過する、間接的な壊変経路の定義を使用する。
            yield return new object[]
            {
                ("C1", "C2"),   // 親－娘の壊変経路を定義する。
                ("C1", "C3"),   // 間接的な壊変経路を定義する。
                ("C1", "C4"),   // 間接的な壊変経路を定義する。
            };

            // 親－娘の壊変経路だけを指定する。
            yield return new object[]
            {
                ("C1", "C2"),   // 親－娘の壊変経路。
                ("C1", "C3"),   // 親－娘の壊変経路。
                ("C2", "C3"),   // 親－娘の壊変経路。
                ("C2", "C4"),   // 親－娘の壊変経路。
                ("C3", "C4"),   // 親－娘の壊変経路。
            };

            // 先に定義した間接的な経路の一部を、後から直接的な経路で明示する。
            yield return new object[]
            {
                ("C1", "C2"),   // 親－娘の壊変経路。
                ("C1", "C3"),   // 先に系列を作る。
                ("C2", "C3"),   // 後で系列内の親－娘の壊変経路を明示している。
                ("C1", "C4"),   // 先に系列を作る。
                ("C2", "C4"),   // 後で系列内の親－娘の壊変経路を明示している。
                ("C3", "C4"),   // 後で系列内の親－娘の壊変経路を明示している。
            };

            // 先に定義した直接的な経路を包含する間接的な経路を後から定義する。
            yield return new object[]
            {
                ("C1", "C2"),   // 親－娘の壊変経路。
                ("C2", "C3"),   // 先に系列内の親－娘の壊変経路を明示している。
                ("C1", "C3"),   // 後で系列を作る。
                ("C2", "C4"),   // 先に系列内の親－娘の壊変経路を明示している。
                ("C3", "C4"),   // 先に系列内の親－娘の壊変経路を明示している。
                ("C1", "C4"),   // 後で系列を作る。
            };

            // 先に定義した直接的な経路＋系列全体を包絡しない間接的な経路の組み合わせを設定する。
            yield return new object[]
            {
                ("C1", "C2"),   // 親－娘の壊変経路。
                ("C2", "C3"),   // 先に系列内の親－娘の壊変経路を明示している。
                ("C1", "C3"),   // 後で系列を作る。
                ("C2", "C4"),   // N1/C1から始まる系列全体を包絡しない間接的な経路。
            };
        }
    }

    [TestClass]
    public class InputAndDecayChainTests
    {
        // Sb-129 -+-> Te-129m -+-+-> Te-129 -+-> I-129 --> (Xe-129)
        //          \            X           /
        //           +----------+ +---------+

        private static string[] GetInflows(Organ organ) => [.. organ.Inflows.Select(i => i.Organ.ToString())];

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

            organs.Select(o => o.ToString()).ShouldBe(
            [
                "Sb-129/input",
                "Sb-129/Blood",
                "Te-129m/Blood",
                "Te-129/Blood",
                "I-129/Blood",
            ]);

            // DecayChain内の核種をインプットで定義したコンパートメントで網羅するよう設定したので、
            // DecayCompartmentは定義されない。

            GetInflows(organs[1]).ShouldBe(["Sb-129/input"]);
            GetInflows(organs[2]).ShouldBe(["Sb-129/Blood"]);
            GetInflows(organs[3]).ShouldBe(["Sb-129/Blood", "Te-129m/Blood"]);
            GetInflows(organs[4]).ShouldBe(["Te-129m/Blood", "Te-129/Blood"]);
        }

        public static IEnumerable<object[]> MixingDecayPathSettings_Cases()
        {
            // 系列内で1つ以上の核種を暗黙に通過する、間接的な壊変経路の定義を使用する。
            yield return new object[]
            {
                CreateReader(
                [
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                    "[intake]", "Blood  100%",

                    "[Sb-129:compartment]",  "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",
                    "[Te-129m:transfer]", "Sb-129/Blood  Blood  ---",   // 親－娘の壊変経路を定義する。
                    "[Te-129:transfer]",  "Sb-129/Blood  Blood  ---",   // 間接的な壊変経路を定義する。
                    "[I-129:transfer]",   "Sb-129/Blood  Blood  ---",   // 間接的な壊変経路を定義する。
                ]),
            };

            // 親－娘の壊変経路だけを指定する。
            yield return new object[]
            {
                CreateReader(
                [
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                    "[intake]", "Blood  100%",

                    "[Sb-129:compartment]",  "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",   // 親－娘の壊変経路。
                    "[Te-129:transfer]",  "Sb-129/Blood   Blood  ---",   // 親－娘の壊変経路。
                                          "Te-129m/Blood  Blood  ---",   // 親－娘の壊変経路。
                    "[I-129:transfer]",   "Te-129m/Blood  Blood  ---",   // 親－娘の壊変経路。
                                          "Te-129/Blood   Blood  ---",   // 親－娘の壊変経路。
                ]),
            };

            // 先に定義した間接的な経路の一部を、後から直接的な経路で明示する。
            yield return new object[]
            {
                CreateReader(
                [
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                    "[intake]", "Blood  100%",

                    "[Sb-129:compartment]",  "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",  // 親－娘の壊変経路。
                    "[Te-129:transfer]",  "Sb-129/Blood   Blood  ---",  // 先に系列を作る。
                                          "Te-129m/Blood  Blood  ---",  // 後で系列内の親－娘の壊変経路を明示している。
                    "[I-129:transfer]",   "Sb-129/Blood   Blood  ---",  // 先に系列を作る。
                                          "Te-129m/Blood  Blood  ---",  // 後で系列内の親－娘の壊変経路を明示している。
                                          "Te-129/Blood   Blood  ---",  // 後で系列内の親－娘の壊変経路を明示している。
                ]),
            };

            // 先に定義した直接的な経路を包含する間接的な経路を後から定義する。
            yield return new object[]
            {
                CreateReader(
                [
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                    "[intake]", "Blood  100%",

                    "[Sb-129:compartment]",  "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",  // 親－娘の壊変経路。
                    "[Te-129:transfer]",
                                          "Te-129m/Blood  Blood  ---",  // 先に系列内の親－娘の壊変経路を明示している。
                                          "Sb-129/Blood   Blood  ---",  // 後で系列を作る。
                    "[I-129:transfer]",
                                          "Te-129m/Blood  Blood  ---",  // 先に系列内の親－娘の壊変経路を明示している。
                                          "Te-129/Blood   Blood  ---",  // 先に系列内の親－娘の壊変経路を明示している。
                                          "Sb-129/Blood   Blood  ---",  // 後で系列を作る。
                ]),
            };

            // 先に定義した直接的な経路＋系列全体を包絡しない間接的な経路の組み合わせを設定する。
            yield return new object[]
            {
                CreateReader(
                [
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                    "[intake]", "Blood  100%",

                    "[Sb-129:compartment]",  "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",  // 親－娘の壊変経路。
                    "[Te-129:transfer]",
                                          "Te-129m/Blood  Blood  ---",  // 先に系列内の親－娘の壊変経路を明示している。
                                          "Sb-129/Blood   Blood  ---",  // 後で系列を作る。
                    "[I-129:transfer]",
                                          "Te-129m/Blood  Blood  ---",  // Sb-129から始まる系列全体を包絡しない間接的な経路。
                ]),
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

            organs.Select(o => o.ToString()).ShouldBe(
            [
                "Sb-129/input",
                "Sb-129/Blood",
                "Te-129m/Blood",
                "Te-129/Blood",
                "I-129/Blood",
                "I-129/<decay_from_Sb-129/Blood>",
            ]);

            // DecayCompartmentは同じDecayChainに属するよう生成される。
            organs[5].IsDecayCompartment.ShouldBeTrue();

            GetInflows(organs[1]).ShouldBe(["Sb-129/input"]);
            GetInflows(organs[2]).ShouldBe(["Sb-129/Blood"]);
            GetInflows(organs[3]).ShouldBe(["Sb-129/Blood", "Te-129m/Blood"]);
            GetInflows(organs[4]).ShouldBe(["I-129/<decay_from_Sb-129/Blood>"]);
            GetInflows(organs[5]).ShouldBe(["Te-129m/Blood", "Te-129/Blood"]);
        }

        public static IEnumerable<object[]> AnywhereStartDecayPathWithCoeff_Cases()
        {
            yield return new object[]
            {
                CreateReader(
                [
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                    "[intake]", "Blood  100%",

                    "[Sb-129:compartment]",  "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",
                    "[Te-129m:transfer]", "Sb-129/Blood  Blood  ---",
                    "[Te-129:transfer]",  "Sb-129/Blood  Blood  ---",
                    "[I-129:transfer]",   "Sb-129/Blood  Blood  100",   // 移行速度付きの壊変経路を、系列先頭の核種Sb-129から設定。
                ]),
            };

            yield return new object[]
            {
                CreateReader(
                [
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                    "[intake]", "Blood  100%",

                    "[Sb-129:compartment]",   "acc  Blood  ---",
                    "[Te-129m:compartment]",  "acc  Blood  ---",
                    "[Te-129:compartment]",   "acc  Blood  ---",
                    "[I-129:compartment]",    "acc  Blood  ---",

                    "[Sb-129:transfer]",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",
                    "[Te-129:transfer]",  "Sb-129/Blood   Blood  ---",
                    "[I-129:transfer]",   "Te-129m/Blood  Blood  100",   // 移行速度付きの壊変経路を、系列途中の核種Te-129mから設定。
                ]),
            };

            yield return new object[]
            {
                CreateReader(
                [
                    "[title]", "dummy",
                    "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                    "[intake]", "Blood  100%",

                    "[Sb-129:compartment]",   "acc  Blood  ---",
                    "[Te-129m:compartment]",  "acc  Blood  ---",
                    "[Te-129:compartment]",   "acc  Blood  ---",
                    "[I-129:compartment]",    "acc  Blood  ---",

                    "[Sb-129:transfer]",
                    "[Te-129m:transfer]", "Sb-129/Blood   Blood  ---",
                    "[Te-129:transfer]",  "Sb-129/Blood   Blood  ---",
                    "[I-129:transfer]",   "Te-129/Blood   Blood  100",   // 移行速度付きの壊変経路を、系列途中の核種Te-129から設定。
                ]),
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
            var reader = CreateReader(
            [
                "[title]", "dummy",
                "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                "[intake]", "Blood  100%",

                "[Sb-129:compartment]",  "acc  Blood  ---",
                "[Te-129m:compartment]", "acc  Blood  ---",
                "[Te-129:compartment]",  "acc  Blood  ---",
                "[I-129:compartment]",   "acc  Blood  ---",

                "[Sb-129:transfer]",
                "[Te-129m:transfer]", "Sb-129/Blood  Blood    10",  // 系列の起点に同じSb-129/Bloodを設定。
                "[Te-129:transfer]",  "Sb-129/Blood  Blood   100",  // 同上。
                "[I-129:transfer]",   "Sb-129/Blood  Blood  1000",  // 同上。
            ]);

            var data = reader.Read();
            var organs = data.Organs;

            organs.Select(o => o.ToString()).ShouldBe(
            [
                "Sb-129/input",
                "Sb-129/Blood",
                "Te-129m/Blood",
                "Te-129/Blood",
                "I-129/Blood",
                "Te-129m/<decay_from_Sb-129/Blood>",
                "Te-129/<decay_from_Sb-129/Blood>",
                "I-129/<decay_from_Sb-129/Blood>",
            ]);

            // DecayChainは、Sb-129/Bloodから始まる崩壊系列の1つが作成される。

            // 移行速度付きの壊変経路で、移行後の核種を受けるambiguous compartmentとして先行生成されたもの。
            organs[5].IsDecayCompartment.ShouldBeTrue();
            organs[6].IsDecayCompartment.ShouldBeTrue();
            organs[7].IsDecayCompartment.ShouldBeTrue();

            // 崩壊系列を補完するambiguous compartmentとしてDefineDecayTransfersにおいて生成されたものは無い。

            GetInflows(organs[1]).ShouldBe(["Sb-129/input"]);
            GetInflows(organs[2]).ShouldBe(["Te-129m/<decay_from_Sb-129/Blood>"]);
            GetInflows(organs[3]).ShouldBe(["Te-129/<decay_from_Sb-129/Blood>"]);
            GetInflows(organs[4]).ShouldBe(["I-129/<decay_from_Sb-129/Blood>"]);

            GetInflows(organs[5]).ShouldBe(["Sb-129/Blood",]);
            GetInflows(organs[6]).ShouldBe(["Sb-129/Blood", "Te-129m/<decay_from_Sb-129/Blood>"]);
            GetInflows(organs[7]).ShouldBe(["Te-129m/<decay_from_Sb-129/Blood>", "Te-129/<decay_from_Sb-129/Blood>"]);
        }

        /// <summary>
        /// DecayChainで表現される壊変経路の系列が2つある場合において、
        /// 暗黙に生成されるDecayCompartmentの作成処理をテストする。
        /// </summary>
        /// <param name="reader"></param>
        [TestMethod]
        public void TwoDecayChainsTest()
        {
            var reader = CreateReader(
            [
                "[title]", "dummy",
                "[nuclide]", "Sb-129  Te-129m  Te-129  I-129",
                "[intake]", "Blood  100%",

                "[Sb-129:compartment]",  "acc  Blood  ---",
                "[Te-129m:compartment]", "acc  Blood  ---",
                "[Te-129:compartment]",  "acc  Blood  ---",
                "[I-129:compartment]",   "acc  Blood  ---",

                "[Sb-129:transfer]",
                "[Te-129m:transfer]", "Sb-129/Blood   Te-129m/Blood   10",  // 1つ目の系列の起点。
                "[Te-129:transfer]",  "Te-129m/Blood  Te-129/Blood   100",  // 2つ目の系列の起点。
                "[I-129:transfer]",   "Sb-129/Blood   I-129/Blood   1000",  // 1つ目の系列。
            ]);

            var data = reader.Read();
            var organs = data.Organs;

            organs.Select(o => o.ToString()).ShouldBe(
            [
                "Sb-129/input",
                "Sb-129/Blood",
                "Te-129m/Blood",
                "Te-129/Blood",
                "I-129/Blood",
                "Te-129m/<decay_from_Sb-129/Blood>",
                "Te-129/<decay_from_Te-129m/Blood>",
                "I-129/<decay_from_Sb-129/Blood>",
                "Te-129/<decay_from_Sb-129/Blood>",
                "I-129/<decay_from_Te-129m/Blood>",
            ]);

            // DecayChainは、以下の2つが作成される。
            // - Sb-129/Blood から始まる崩壊系列
            // - Te-129m/Blood から始まる崩壊系列

            // 移行速度付きの壊変経路で、移行後の核種を受けるambiguous compartmentとして先行して生成されたもの。
            organs[5].IsDecayCompartment.ShouldBeTrue();
            organs[6].IsDecayCompartment.ShouldBeTrue();
            organs[7].IsDecayCompartment.ShouldBeTrue();

            // 残りは、崩壊系列を補完するambiguous compartmentとしてDefineDecayTransfersにおいて生成されたもの。
            organs[8].IsDecayCompartment.ShouldBeTrue();
            organs[9].IsDecayCompartment.ShouldBeTrue();

            GetInflows(organs[1]).ShouldBe(["Sb-129/input"]);
            GetInflows(organs[2]).ShouldBe(["Te-129m/<decay_from_Sb-129/Blood>"]);
            GetInflows(organs[3]).ShouldBe(["Te-129/<decay_from_Te-129m/Blood>"]);
            GetInflows(organs[4]).ShouldBe(["I-129/<decay_from_Sb-129/Blood>"]);

            GetInflows(organs[5]).ShouldBe(["Sb-129/Blood",]);
            GetInflows(organs[6]).ShouldBe(["Te-129m/Blood"]);
            GetInflows(organs[7]).ShouldBe(["Te-129m/<decay_from_Sb-129/Blood>", "Te-129/<decay_from_Sb-129/Blood>"]);

            GetInflows(organs[8]).ShouldBe(["Sb-129/Blood", "Te-129m/<decay_from_Sb-129/Blood>"]);
            GetInflows(organs[9]).ShouldBe(["Te-129m/Blood", "Te-129/<decay_from_Te-129m/Blood>"]);
        }
    }
}
