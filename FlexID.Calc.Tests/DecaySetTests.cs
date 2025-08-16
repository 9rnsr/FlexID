namespace FlexID.Calc.Tests;

using static InputErrorTestHelpers;

[TestClass]
public class DecaySetTests
{
    // N1 --> N2 --> N3 --> N4 --> (stable)
    [TestClass]
    public class StraightDecayChain
    {
        const int LineNum = 1;
        const int Line1 = LineNum + 0;
        const int Line2 = LineNum + 1;
        const int Line3 = LineNum + 2;
        const int Line4 = LineNum + 3;
        const int Line5 = LineNum + 4;
        const int Line6 = LineNum + 5;
        const int Line7 = LineNum + 6;

        readonly NuclideData N1; readonly Organ C11, C12, C13, C14;
        readonly NuclideData N2; readonly Organ C21, C22;
        readonly NuclideData N3; readonly Organ C31, C32;
        readonly NuclideData N4; readonly Organ C41;

        readonly DecaySet decaySet;
        readonly InputErrors errors = new();

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

        // [Line1] N1/C11 --> N2/C21
        // =========================
        //         N1/C11 --> N2/C21
        [TestMethod]
        [DataRow(null)]
        [DataRow(1.23)]
        public void SinglePath(double? coeff)
        {
            var organDecay = decaySet.AddDecayPath(Line1, C11, C21, (decimal?)coeff);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            if (coeff is null)
            {
                chain1[N1].ShouldBe((Line1, C11));
                chain1[N2].ShouldBe((Line1, C21));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[C21].ShouldBe([chain1]);
            }
            else
            {
                chain1[N1].ShouldBe((Line1, C11));
                chain1[N2].ShouldBe((Line1, organDecay));

                decaySet.JointChains[C11].ShouldBe([chain1]);
                decaySet.JointChains[organDecay].ShouldBe([chain1]);
            }
        }

        // [Line1] N1/C11 --> N2/C21
        // [Line2] N1/C12 --> N2/C22
        // =========================
        //         N1/C11 --> N2/C21
        //         N1/C12 --> N2/C22
        [TestMethod]
        public void ParallelPaths()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C12, C22, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain2[N1].ShouldBe((Line2, C12));
            chain2[N2].ShouldBe((Line2, C22));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C22].ShouldBe([chain2]);
        }

        // [Line1] N1/C11 ----------> N2/C21
        // [Line2] N1/C12 -(coeff.)-> N2/C21
        // ===============================
        //         N1/C11 ----------> N2/C21
        //                              ↑
        //                            (coeff.)
        //                              ↑
        //         N1/C12 ----------> N2/d21
        [TestMethod]
        public void ParallelPaths2()
        {
            // 2つの経路設定は、終端側がC21で合流しているように見えるが
            // 2つ目の経路は移行速度付きのため実際はN2位置で壊変コンパートメントが自動作成され、
            // 従ってこれらは独立した2つの壊変経路を作成する。
            var d11 = decaySet.AddDecayPath(Line1, C11, C21, null);   // 壊変経路1
            var d21 = decaySet.AddDecayPath(Line2, C12, C21, 123m);   // 壊変経路2: 経路1とは関係ない
            errors.GetErrorLines().ShouldBe([]);
            d11.ShouldBeNull();
            d21.ShouldNotBeNull();

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));

            chain2[N1].ShouldBe((Line2, C12));
            chain2[N2].ShouldBe((Line2, d21));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[d21].ShouldBe([chain2]);
        }

        // [Line1] N1/C11 -----> N2/C21
        // [Line2] N1/C12 -----> N2/C21
        // =========================
        //         N1/C11 ---+-> N2/C21
        //                  /
        //         N1/C12 -+
        [TestMethod]
        public void JoinPathsAtN2()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C12, C21, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain2[N1].ShouldBe((Line2, C12));
            chain2[N2].ShouldBe((Line2, C21));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
        }

        // [Line1] N1/C11 -----> N2/C21
        // [Line2] N1/C12 -----> N2/C21
        // [Line3]               N2/C21 --> N3/C31
        // =======================================
        //         N1/C11 ---+-> N2/C21 --> N3/C31
        //                  /
        //         N1/C12 -+
        [TestMethod]
        public void JoinPathsAtN2_AndShareN3_Order1()
        {
            // 核種N2で合流が発生した場合、その子孫核種であるN3のコンパートメントは
            // 自動的に合流した2つの系列で共有される。
            var d11 = decaySet.AddDecayPath(Line1, C11, C21, null);
            var d21 = decaySet.AddDecayPath(Line2, C12, C21, null);
            var d31 = decaySet.AddDecayPath(Line3, C21, C31, null);
            errors.GetErrorLines().ShouldBe([]);
            d11.ShouldBeNull();
            d21.ShouldBeNull();
            d31.ShouldBeNull();

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line3, C31));

            chain2[N1].ShouldBe((Line2, C12));
            chain2[N2].ShouldBe((Line2, C21));
            chain2[N3].ShouldBe((Line3, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
        }

        // [Line1] N1/C11 -----> N2/C21
        // [Line3]               N2/C21 --> N3/C31
        // [Line2] N1/C12 -----> N2/C21
        // =======================================
        //         N1/C11 ---+-> N2/C21 --> N3/C31
        //                  /
        //         N1/C12 -+
        [TestMethod]
        public void JoinPathsAtN2_AndShareN3_Order2()
        {
            // 核種N2で合流が発生した場合、その子孫核種であるN3のコンパートメントは
            // 自動的に合流した2つの系列で共有される。
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line3, C21, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];
            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line3, C31));

            // chain1に対してN2位置で合流するchain2を追加する。
            decaySet.AddDecayPath(Line2, C12, C21, null);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain2 = decaySet.DecayChains[1];
            chain2[N1].ShouldBe((Line2, C12));
            chain2[N2].ShouldBe((Line2, C21));
            chain2[N3].ShouldBe((Line3, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
        }

        // [Line1] N1/C11 -------> N2/C21
        // [Line2] N1/C12 -------> N2/C21
        // [Line3]                 N2/C21 -(coeff.)-> N3/C31
        // ===============================================
        //         N1/C11 -----+-> N2/C21 ----------> N3/d31
        //                    /                         ↓
        //                   /                        (coeff.)
        //                  /                           ↓
        //         N1/C12 -+                          N3/C31
        [TestMethod]
        public void JoinPathsAtN2_AndShareN3_WithCoeff()
        {
            // 合流が発生した核種N2の子孫核種N3の位置が壊変コンパートメントである場合も、
            // 自動的に合流した2つの系列で共有される。
            var d11 = decaySet.AddDecayPath(Line1, C11, C21, null);
            var d21 = decaySet.AddDecayPath(Line2, C12, C21, null);
            var d31 = decaySet.AddDecayPath(Line3, C21, C31, 123m);
            errors.GetErrorLines().ShouldBe([]);
            d11.ShouldBeNull();
            d21.ShouldBeNull();
            d31.ShouldNotBeNull();

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line3, d31));

            chain2[N1].ShouldBe((Line2, C12));
            chain2[N2].ShouldBe((Line2, C21));
            chain2[N3].ShouldBe((Line3, d31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[d31].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains.ContainsKey(C31).ShouldBeFalse();
        }

        // [Line1] N1/C11 -------> N2/C21
        // [Line2]                 N2/C21 --> N3/C31
        // [Line3]                            N3/C31 --> N4/C41
        // [Line4] N1/C12 -------> N2/C21
        // ====================================================
        //         N1/C11 -----+-> N2/C21 --> N3/C31 --> N4/C41
        //                    /
        //                   /
        //                  /
        //         N1/C12 -+
        [TestMethod]
        public void JoinPathsAtN2_AndShareN3N4()
        {
            // 核種N2で合流が発生した場合、その子孫核種であるN3,N4のコンパートメントは
            // 自動的に合流した2つの系列で共有される。
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C21, C31, null);
            decaySet.AddDecayPath(Line3, C31, C41, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];
            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line2, C31));
            chain1[N4].ShouldBe((Line3, C41));

            decaySet.AddDecayPath(Line4, C12, C21, null);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain2 = decaySet.DecayChains[1];
            chain2[N1].ShouldBe((Line4, C12));
            chain2[N2].ShouldBe((Line4, C21));
            chain2[N3].ShouldBe((Line2, C31));
            chain2[N4].ShouldBe((Line3, C41));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C41].ShouldBe([chain1, chain2], ignoreOrder: true);
        }

        // [Line1] N1/C11 -------> N2/C21
        // [Line2] N1/C12 -------> N2/C21
        // [Line3]                 N2/C21 -------> N3/C31
        // [Line4]                                 N3/C31 --> N4/C41
        // [Line5] N1/C13 -------> N2/C22
        // [Line6] N1/C14 -------> N2/C22
        // [Line7]                 N2/C22 -------> N3/C31
        // =========================================================
        //         N1/C11 ---+---> N2/C21 -----+-> N3/C31 --> N4/C41
        //                  /                 /
        //         N1/C12 -+                 /
        //                                  /
        //         N1/C13 ---+---> N2/C22 -+
        //                  /
        //         N1/C14 -+
        [TestMethod]
        public void JoinPathsAtN2N3_AndShareN4()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C12, C21, null);
            decaySet.AddDecayPath(Line3, C21, C31, null);
            decaySet.AddDecayPath(Line4, C31, C41, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];
            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line3, C31));
            chain1[N4].ShouldBe((Line4, C41));
            chain2[N1].ShouldBe((Line2, C12));
            chain2[N2].ShouldBe((Line2, C21));
            chain2[N3].ShouldBe((Line3, C31));
            chain2[N4].ShouldBe((Line4, C41));

            decaySet.AddDecayPath(Line5, C13, C22, null);
            decaySet.AddDecayPath(Line6, C14, C22, null);

            decaySet.DecayChains.Count.ShouldBe(4);
            var chain3 = decaySet.DecayChains[2];
            var chain4 = decaySet.DecayChains[3];
            chain3[N1].ShouldBe((Line5, C13));
            chain3[N2].ShouldBe((Line5, C22));
            chain3[N3].ShouldBe(default);
            chain3[N4].ShouldBe(default);
            chain4[N1].ShouldBe((Line6, C14));
            chain4[N2].ShouldBe((Line6, C22));
            chain4[N3].ShouldBe(default);
            chain4[N4].ShouldBe(default);

            decaySet.AddDecayPath(Line7, C22, C31, null);

            decaySet.DecayChains.Count.ShouldBe(4);
            chain3[N3].ShouldBe((Line7, C31));
            chain3[N4].ShouldBe((Line4, C41));
            chain4[N3].ShouldBe((Line7, C31));
            chain4[N4].ShouldBe((Line4, C41));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[C13].ShouldBe([chain3]);
            decaySet.JointChains[C14].ShouldBe([chain4]);
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C22].ShouldBe([chain3, chain4], ignoreOrder: true);
            decaySet.JointChains[C31].ShouldBe([chain1, chain2, chain3, chain4], ignoreOrder: true);
            decaySet.JointChains[C41].ShouldBe([chain1, chain2, chain3, chain4], ignoreOrder: true);
        }

        // [Line1] N1/C11 -----> N2/C21
        // [Line2] N1/C12 -----> N2/C21
        // [Line3] N1/C13 -----> N2/C22
        // [Line4] N1/C14 -----> N2/C22
        // [Line5]               N2/C21 -------> N3/C31
        // [Line6]               N2/C22 -------> N3/C31
        // =========================
        //         N1/C11 ---+-> N2/C21 -----+-> N3/C31
        //                  /               /
        //         N1/C12 -+               /
        //                                /
        //         N1/C13 ---+-> N2/C22 -+
        //                  /
        //         N1/C14 -+
        [TestMethod]
        public void JoinPathsAtN3_Order1()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C12, C21, null);
            decaySet.AddDecayPath(Line3, C13, C22, null);
            decaySet.AddDecayPath(Line4, C14, C22, null);

            decaySet.DecayChains.Count.ShouldBe(4);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];
            var chain3 = decaySet.DecayChains[2];
            var chain4 = decaySet.DecayChains[3];
            chain1[N1].ShouldBe((Line1, C11));
            chain2[N1].ShouldBe((Line2, C12));
            chain3[N1].ShouldBe((Line3, C13));
            chain4[N1].ShouldBe((Line4, C14));
            chain1[N2].ShouldBe((Line1, C21));
            chain2[N2].ShouldBe((Line2, C21));
            chain3[N2].ShouldBe((Line3, C22));
            chain4[N2].ShouldBe((Line4, C22));
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C22].ShouldBe([chain3, chain4], ignoreOrder: true);

            decaySet.AddDecayPath(Line5, C21, C31, null);
            decaySet.AddDecayPath(Line6, C22, C31, null);

            chain1[N1].ShouldBe((Line1, C11));
            chain2[N1].ShouldBe((Line2, C12));
            chain3[N1].ShouldBe((Line3, C13));
            chain4[N1].ShouldBe((Line4, C14));
            chain1[N2].ShouldBe((Line1, C21));
            chain2[N2].ShouldBe((Line2, C21));
            chain3[N2].ShouldBe((Line3, C22));
            chain4[N2].ShouldBe((Line4, C22));
            chain1[N3].ShouldBe((Line5, C31));
            chain2[N3].ShouldBe((Line5, C31));
            chain3[N3].ShouldBe((Line6, C31));
            chain4[N3].ShouldBe((Line6, C31));
            decaySet.JointChains[C31].ShouldBe([chain1, chain2, chain3, chain4], ignoreOrder: true);
        }

        // [Line1] N1/C11 -----> N2/C21
        // [Line2] N1/C12 -----> N2/C21
        // [Line3] N1/C13 -----> N2/C22
        // [Line4] N1/C14 -----> N2/C22
        // [Line5] N1/C11 ---------------------> N3/C31
        // [Line6] N2/C14 ---------------------> N3/C31
        // =========================
        //         N1/C11 ---+-> N2/C21 -----+-> N3/C31
        //                  /               /
        //         N1/C12 -+               /
        //                                /
        //         N1/C13 ---+-> N2/C22 -+
        //                  /
        //         N1/C14 -+
        [TestMethod]
        public void JoinPathsAtN3_Order2()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C12, C21, null);
            decaySet.AddDecayPath(Line3, C13, C22, null);
            decaySet.AddDecayPath(Line4, C14, C22, null);

            decaySet.DecayChains.Count.ShouldBe(4);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];
            var chain3 = decaySet.DecayChains[2];
            var chain4 = decaySet.DecayChains[3];
            chain1[N1].ShouldBe((Line1, C11));
            chain2[N1].ShouldBe((Line2, C12));
            chain3[N1].ShouldBe((Line3, C13));
            chain4[N1].ShouldBe((Line4, C14));
            chain1[N2].ShouldBe((Line1, C21));
            chain2[N2].ShouldBe((Line2, C21));
            chain3[N2].ShouldBe((Line3, C22));
            chain4[N2].ShouldBe((Line4, C22));
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C22].ShouldBe([chain3, chain4], ignoreOrder: true);

            decaySet.AddDecayPath(Line5, C11, C31, null);
            decaySet.AddDecayPath(Line6, C14, C31, null);

            chain1[N1].ShouldBe((Line1, C11));
            chain2[N1].ShouldBe((Line2, C12));
            chain3[N1].ShouldBe((Line3, C13));
            chain4[N1].ShouldBe((Line4, C14));
            chain1[N2].ShouldBe((Line1, C21));
            chain2[N2].ShouldBe((Line2, C21));
            chain3[N2].ShouldBe((Line3, C22));
            chain4[N2].ShouldBe((Line4, C22));
            chain1[N3].ShouldBe((Line5, C31));
            chain2[N3].ShouldBe((Line5, C31));
            chain3[N3].ShouldBe((Line6, C31));
            chain4[N3].ShouldBe((Line6, C31));
            decaySet.JointChains[C31].ShouldBe([chain1, chain2, chain3, chain4], ignoreOrder: true);
        }

        // [Line1] N1/C11 -----> N2/C21
        // [Line2] N1/C11 -----> N2/C22
        // ============================
        //         N1/C11 -+---> N2/C21
        //                  \ 
        //                   +-> N2/C22 (error)
        [TestMethod]
        [DataRow(null, null)]
        [DataRow(1.23, null)]
        [DataRow(null, 2.34)]
        [DataRow(1.23, 2.34)]
        public void DivergePathsOnTheToPoint1(double? coeff1, double? coeff2)
        {
            decaySet.AddDecayPath(Line1, C11, C21, (decimal?)coeff1);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.AddDecayPath(Line2, C11, C22, (decimal?)coeff2);
            errors.GetErrorLines().ShouldBe(
            [
                $"Line {Line2}: Decay paths conflict each other:",
                $"    the previous: '{C11}' --> '{C21}'" + (coeff1 is null ? "": $" (with coeff. {coeff1.Value})") + $" at Line {Line1}",
                $"    and here    : '{C11}' --> '{C22}'" + (coeff2 is null ? "": $" (with coeff. {coeff2.Value})"),
            ]);
        }

        // [Line1] N1/C11 -----> N2/C21
        // [Line2] N1/C12 -----> N2/C21
        // [Line3]               N2/C21 -----> N3/C31
        // [Line4] N1/C12 -------------------> N3/C32
        // ==========================================
        //         N1/C11 ---+-> N2/C21 -+---> N3/C31
        //                  /             \
        //         N1/C12 -+               +-> N3/C32 (error)
        [TestMethod]
        public void DivergePathsOnTheToPoint2()
        {
            // 核種N2で合流が発生した場合、その子孫核種であるN3のコンパートメントは
            // 自動的に合流した2つの系列で共有される。
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C12, C21, null);
            decaySet.AddDecayPath(Line3, C21, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            {
                chain1[N1].ShouldBe((Line1, C11));
                chain1[N2].ShouldBe((Line1, C21));
                chain1[N3].ShouldBe((Line3, C31));
            }
            var chain2 = decaySet.DecayChains[1];
            {
                chain2[N1].ShouldBe((Line2, C12));
                chain2[N2].ShouldBe((Line2, C21));
                chain2[N3].ShouldBe((Line3, C31));
            }
            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);

            // 新しい経路の末端側の位置(核種N3)で壊変経路の分岐が生じる。
            decaySet.AddDecayPath(Line4, C12, C32, null);
            errors.GetErrorLines().ShouldBe(
            [
                $"Line {Line4}: Decay paths conflict each other:",
                $"    the previous: '{C12}' --> '{C31}'" + $" at Line {Line3}",
                $"    and here    : '{C12}' --> '{C32}'",
            ]);
        }

        // [Line1] N1/C11 -------------------> N3/C31
        // [Line2] N1/C12 -------------------> N3/C32
        // [Line3] N1/C11 -----> N2/C21
        // [Line4] N1/C12 -----> N2/C21
        // ==========================================
        //         N1/C11 ---+-> N2/C21 -+---> N3/C31
        //                  /             \
        //         N1/C12 -+               +-> N3/C32 (error)
        [TestMethod]
        public void DivergePathsOnTheProgenyOfToPoint()
        {
            decaySet.AddDecayPath(Line1, C11, C31, null);
            decaySet.AddDecayPath(Line2, C12, C32, null);
            decaySet.AddDecayPath(Line3, C11, C21, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line3, C21));
            chain1[N3].ShouldBe((Line1, C31));

            chain2[N1].ShouldBe((Line2, C12));
            chain2[N2].ShouldBe(default);
            chain2[N3].ShouldBe((Line2, C32));

            // 新しい経路の末端側(核種N2)より子孫の位置(核種N3)で壊変経路の分岐が生じる。
            decaySet.AddDecayPath(Line4, C12, C21, null);
            errors.GetErrorLines().ShouldBe(
            [
                $"Line 4: Decay paths conflict each other:",
                $"    the previous: '{C21}' --> '{C31}' at Line {Line1}",
                $"    and another : '{C21}' --> '{C32}' at Line {Line2}",
            ]);
        }

        // [Line1] N1/C11 --> N2/C21
        // [Line2]            N2/C21 --> N3/C31
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order1a()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C21, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line2, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1]);
        }

        // [Line1]            N2/C21 --> N3/C31
        // [Line2] N1/C11 --> N2/C21
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order1b()
        {
            decaySet.AddDecayPath(Line1, C21, C31, null);
            decaySet.AddDecayPath(Line2, C11, C21, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            chain1[N1].ShouldBe((Line2, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line1, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1]);
        }

        // [Line1] N1/C11 -------------> N3/C31
        // [Line2]         N2/C21 -----> N3/C31
        // ====================================
        //         N1/C11 -----------+-> N3/C31
        //                          /
        //                 N2/C21 -+
        [TestMethod]
        public void FillChainPaths_Order2a()
        {
            decaySet.AddDecayPath(Line1, C11, C31, null);
            decaySet.AddDecayPath(Line2, C21, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe(default);
            chain1[N3].ShouldBe((Line1, C31));

            chain2[N1].ShouldBe(default);
            chain2[N2].ShouldBe((Line2, C21));
            chain2[N3].ShouldBe((Line2, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain2]);
            decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
        }

        // [Line1]         N2/C21 ---> N3/C31
        // [Line2] N1/C11 -----------> N3/C31
        // ==================================
        //                 N2/C21 -+-> N3/C31
        //                        /
        //         N1/C11 -------+
        [TestMethod]
        public void FillChainPaths_Order2b()
        {
            decaySet.AddDecayPath(Line1, C21, C31, null);
            decaySet.AddDecayPath(Line2, C11, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe(default);
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line1, C31));

            chain2[N1].ShouldBe((Line2, C11));
            chain2[N2].ShouldBe(default);
            chain2[N3].ShouldBe((Line2, C31));

            decaySet.JointChains[C11].ShouldBe([chain2]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
        }

        // [Line1] N1/C11 -------------> N3/C31
        // [Line2] N1/C11 --> N2/C21
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order3a()
        {
            decaySet.AddDecayPath(Line1, C11, C31, null);
            decaySet.AddDecayPath(Line2, C11, C21, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line2, C21));
            chain1[N3].ShouldBe((Line1, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1]);
        }

        // [Line1] N1/C11 --> N2/C21
        // [Line2] N1/C11 -------------> N3/C31
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order3b()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C11, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line2, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1]);
        }

        // [Line1] N1/C11 --> N2/C21
        // [Line2]            N2/C21 --> N3/C31
        // [Line3] N1/C11 -------------> N3/C31
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order4a()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C21, C31, null);
            decaySet.AddDecayPath(Line3, C11, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line2, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1]);
        }

        // [Line1]            N2/C21 --> N3/C31
        // [Line2] N1/C11 --> N2/C21
        // [Line3] N1/C11 -------------> N3/C31
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order4b()
        {
            decaySet.AddDecayPath(Line1, C21, C31, null);
            decaySet.AddDecayPath(Line2, C11, C21, null);
            decaySet.AddDecayPath(Line3, C11, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            chain1[N1].ShouldBe((Line2, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line1, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1]);
        }

        // [Line1] N1/C11 --> N2/C21
        // [Line2] N1/C11 -------------> N3/C31
        // [Line3]            N2/C21 --> N3/C31
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order4c()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C11, C31, null);
            decaySet.AddDecayPath(Line3, C21, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line2, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1]);
        }

        // [Line1]            N2/C21 --> N3/C31
        // [Line2] N1/C11 -------------> N3/C31
        // [Line3] N1/C11 --> N2/C21
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order4d()
        {
            decaySet.AddDecayPath(Line1, C21, C31, null);
            decaySet.AddDecayPath(Line2, C11, C31, null);
            decaySet.AddDecayPath(Line3, C11, C21, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe(default);
            chain1[N2].ShouldBe((Line1, C21));
            chain1[N3].ShouldBe((Line1, C31));

            chain2[N1].ShouldBe((Line2, C11));
            chain2[N2].ShouldBe((Line3, C21));
            chain2[N3].ShouldBe((Line2, C31));

            decaySet.JointChains[C11].ShouldBe([chain2]);
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
        }

        // [Line1] N1/C11 -------------> N3/C31
        // [Line2] N1/C11 --> N2/C21
        // [Line3]            N2/C21 --> N3/C31
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order4e()
        {
            decaySet.AddDecayPath(Line1, C11, C31, null);
            decaySet.AddDecayPath(Line2, C11, C21, null);
            decaySet.AddDecayPath(Line3, C21, C31, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line2, C21));
            chain1[N3].ShouldBe((Line1, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1]);
        }

        // [Line1] N1/C11 -------------> N3/C31
        // [Line2]            N2/C21 --> N3/C31
        // [Line3] N1/C11 --> N2/C21
        // ====================================
        //         N1/C11 --> N2/C21 --> N3/C31
        [TestMethod]
        public void FillChainPaths_Order4f()
        {
            decaySet.AddDecayPath(Line1, C11, C31, null);
            decaySet.AddDecayPath(Line2, C21, C31, null);
            decaySet.AddDecayPath(Line3, C11, C21, null);
            errors.GetErrorLines().ShouldBe([]);

            decaySet.DecayChains.Count.ShouldBe(2);
            var chain1 = decaySet.DecayChains[0];
            var chain2 = decaySet.DecayChains[1];

            chain1[N1].ShouldBe((Line1, C11));
            chain1[N2].ShouldBe((Line3, C21));
            chain1[N3].ShouldBe((Line1, C31));

            chain2[N1].ShouldBe(default);
            chain2[N2].ShouldBe((Line2, C21));
            chain2[N3].ShouldBe((Line2, C31));

            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain2, chain1], ignoreOrder: true);
            decaySet.JointChains[C31].ShouldBe([chain1, chain2], ignoreOrder: true);
        }
    }

    //        +--> N2 --+
    //       /           \
    // N1 --+             +-> N4
    //       \           /
    //        +--> N3 --+
    [TestClass]
    public class DiamondDecayChain
    {
        const int LineNum = 1;
        const int Line1 = LineNum + 0;
        const int Line2 = LineNum + 1;
        const int Line3 = LineNum + 2;
        const int Line4 = LineNum + 3;
        const int Line5 = LineNum + 4;

        readonly NuclideData N1; readonly Organ C11, C12, C13;
        readonly NuclideData N2; readonly Organ C21;
        readonly NuclideData N3; readonly Organ C31;
        readonly NuclideData N4; readonly Organ C41;

        readonly DecaySet decaySet;
        readonly InputErrors errors = new();

        public DiamondDecayChain()
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

        // [Line1] N1/C11 --------> N2/C21
        // [Line2] N1/C11 ------------------> N3/C31
        // [Line1] N1/C12 --------> N2/C21
        // [Line2] N1/C13 ------------------> N3/C31
        // [Line3] N1/C11 ---------------------------------> N4/C41
        // =================================================
        //         N1/C21 -----+--> N2/C21 ------------+
        //                    /                         \
        //         N1/C11 ---+                           +-> N4/C41
        //                    \                         /
        //                     +------------> N3/C31 --+
        //                    /
        //         N1/C13 ---+
        [TestMethod]
        public void JoinPathsAndShareGrandDaughter()
        {
            decaySet.AddDecayPath(Line1, C11, C21, null);
            decaySet.AddDecayPath(Line2, C11, C31, null);

            decaySet.DecayChains.Count.ShouldBe(1);
            var chain1 = decaySet.DecayChains[0];
            {
                chain1[N1].ShouldBe((Line1, C11));
                chain1[N2].ShouldBe((Line1, C21));
                chain1[N3].ShouldBe((Line2, C31));
            }
            decaySet.JointChains[C11].ShouldBe([chain1]);
            decaySet.JointChains[C21].ShouldBe([chain1]);
            decaySet.JointChains[C31].ShouldBe([chain1]);

            decaySet.AddDecayPath(Line3, C12, C21, null);
            decaySet.AddDecayPath(Line4, C13, C31, null);

            decaySet.DecayChains.Count.ShouldBe(3);
            var chain2 = decaySet.DecayChains[1];
            {
                chain2[N1].ShouldBe((Line3, C12));
                chain2[N2].ShouldBe((Line3, C21));
                chain2[N3].ShouldBe(default);
            }
            var chain3 = decaySet.DecayChains[2];
            {
                chain3[N1].ShouldBe((Line4, C13));
                chain3[N2].ShouldBe(default);
                chain3[N3].ShouldBe((Line4, C31));
            }
            decaySet.JointChains[C12].ShouldBe([chain2]);
            decaySet.JointChains[C13].ShouldBe([chain3]);
            decaySet.JointChains[C21].ShouldBe([chain1, chain2], ignoreOrder: true);
            decaySet.JointChains[C31].ShouldBe([chain1, chain3], ignoreOrder: true);

            // chain1の中でC41への経路を設定した時点で、chain1に加えて
            // C21に合流している崩壊系列chain2とC31に合流している崩壊系列chain3も
            // C41を共有する崩壊系列として判定される。
            decaySet.AddDecayPath(Line5, C11, C41, null);

            decaySet.DecayChains.Count.ShouldBe(3);
            {
                chain1[N4].ShouldBe((Line5, C41));
                chain2[N4].ShouldBe((Line5, C41));
                chain3[N4].ShouldBe((Line5, C41));
            }
            decaySet.JointChains[C41].ShouldBe([chain1, chain2, chain3], ignoreOrder: true);
        }
    }

    /// <summary>
    /// 直接的・間接的な壊変経路の設定を混合した場合でも、系列内の壊変経路が正しく接続されることを確認する。
    /// </summary>
    // N1 -+-> N2 -+-+-> N3 -+-> N4 --> (stable)
    //      \       X       /
    //       +-----+ +-----+
    [TestClass]
    public class ComplexDecayChain
    {
        readonly NuclideData N1; readonly Organ C1;
        readonly NuclideData N2; readonly Organ C2;
        readonly NuclideData N3; readonly Organ C3;
        readonly NuclideData N4; readonly Organ C4;

        readonly DecaySet decaySet;
        readonly InputErrors errors = new();

        public ComplexDecayChain()
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

            var line = 1;
            foreach (var (from, to) in paths)
            {
                var organFrom = compartments.First(c => c.Name == from);
                var organTo = compartments.First(c => c.Name == to);
                decaySet.AddDecayPath(line++, organFrom, organTo, null);
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
    public class DecayChainTestsX
    {
        // Sb-129 -+-> Te-129m -+-+-> Te-129 -+-> I-129 --> (Xe-129)
        //          \            X           /
        //           +----------+ +---------+

        private static string[] GetInflows(Organ organ) => [.. organ.Inflows.Select(i => i.Organ.ToString())];

#if false
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

                    "[Sb-129:compartment]",  "inp  input  ---",
                                             "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input         Blood  100%",
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
                ]),
            };

            // 先に定義した間接的な経路の一部を、後から直接的な経路で明示する。
            yield return new object[]
            {
                CreateReader(
                [
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
                ]),
            };

            // 先に定義した直接的な経路を包含する間接的な経路を後から定義する。
            yield return new object[]
            {
                CreateReader(
                [
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
                ]),
            };

            // 先に定義した直接的な経路＋系列全体を包絡しない間接的な経路の組み合わせを設定する。
            yield return new object[]
            {
                CreateReader(
                [
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
                ]),
            };
        }
#endif

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

                    "[Sb-129:compartment]",  "inp  input  ---",
                                             "acc  Blood  ---",
                    "[Te-129m:compartment]", "acc  Blood  ---",
                    "[Te-129:compartment]",  "acc  Blood  ---",
                    "[I-129:compartment]",   "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input         Blood  100%",
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

                    "[Sb-129:compartment]",   "inp  input  ---",
                                              "acc  Blood  ---",
                    "[Te-129m:compartment]",  "acc  Blood  ---",
                    "[Te-129:compartment]",   "acc  Blood  ---",
                    "[I-129:compartment]",    "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input          Blood  100%",
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

                    "[Sb-129:compartment]",   "inp  input  ---",
                                              "acc  Blood  ---",
                    "[Te-129m:compartment]",  "acc  Blood  ---",
                    "[Te-129:compartment]",   "acc  Blood  ---",
                    "[I-129:compartment]",    "acc  Blood  ---",

                    "[Sb-129:transfer]",  "input          Blood  100%",
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

                "[Sb-129:compartment]",  "inp  input  ---",
                                            "acc  Blood  ---",
                "[Te-129m:compartment]", "acc  Blood  ---",
                "[Te-129:compartment]",  "acc  Blood  ---",
                "[I-129:compartment]",   "acc  Blood  ---",

                "[Sb-129:transfer]",  "input         Blood   100%",
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

                "[Sb-129:compartment]",  "inp  input  ---",
                                         "acc  Blood  ---",
                "[Te-129m:compartment]", "acc  Blood  ---",
                "[Te-129:compartment]",  "acc  Blood  ---",
                "[I-129:compartment]",   "acc  Blood  ---",

                "[Sb-129:transfer]",  "input          Sb-129/Blood   100%",
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
