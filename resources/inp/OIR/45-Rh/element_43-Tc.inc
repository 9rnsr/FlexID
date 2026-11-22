[{Tc}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   S-glands              S-glands
  acc   OralcavityRe          O-cavity
  acc   Oesophagus-sRe        Oesophag-s
  acc   St-conRe              St-cont
  acc   St-wall               St-wall
  acc   RC-wall               RC-wall
  acc   Thyroid1              Thyroid       # fast turnover
  acc   Thyroid2              Thyroid       # slow turnover
  acc   Liver1                Liver         # fast turnover
  acc   Liver2                Liver         # slow turnover
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissues
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Tc}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.130 p.76 Table 3.4
# ICRP Publ.130 p.77 Para.143
# ICRP Publ.134 p.334 Fig.15.1
  OralcavityRe            Oesophagus-sRe          720
  Oesophagus-sRe          St-conRe               2160
  St-conRe                SI-conRe                 20.57

# ICRP Publ.134 p.334 Table 15.3
  Blood                   Thyroid1                  7.0
  Blood                   ST0                      71.88
  Blood                   ST1                       3.0
  Blood                   ST2                       0.18
  Blood                   UB-con                    1.7
  Blood                   S-glands                  2.6
  Blood                   St-wall                   4.3
  Blood                   Kidneys1                  0.7
  Blood                   Kidneys2                  0.04
  Blood                   Liver1                    4.5
  Blood                   RC-wall                   3.4
  Blood                   T-bone-S                  0.35
  Blood                   C-bone-S                  0.35
  Thyroid1                Blood                   100
  Thyroid1                Thyroid2                  1.0
  Thyroid2                Blood                     1.0
  ST0                     Blood                    50
  ST1                     Blood                     0.462
  ST2                     Blood                     0.0347
  S-glands                OralcavityRe             50
  St-wall                 St-conRe                 50
  Kidneys1                UB-con                    8.32
  Kidneys2                Blood                     0.0347
  Liver1                  Blood                     8.234
  Liver1                  Liver2                    0.0832
  Liver2                  Blood                     0.0347
  RC-wall                 RC-con                    1.39
  T-bone-S                Blood                     0.457
  T-bone-S                T-bone-V                  0.00462
  C-bone-S                Blood                     0.457
  C-bone-S                C-bone-V                  0.00462
  T-bone-V                Blood                     0.000493
  C-bone-V                Blood                     0.0000821

# ICRP Publ.151 p.215 Para.421
# > (421) Progeny of rhodium addressed in this publication are isotopes of rhodium,
# > ruthenium, and technetium. The common model for rhodium and ruthenium as
# > parents applied in the OIR series is applied to isotopes of these two elements as
# > progeny of rhodium. The model for technetium as a progeny of ruthenium applied in
# > OIR Part 3 (ICRP, 2017) is applied to technetium as a progeny of rhodium.
# > (421) 本出版物で扱うロジウムの娘核種は、ロジウム、ルテニウム、およびテクネチウムの
# > 同位体です。OIRシリーズにおいて、親核種としてのロジウムおよびルテニウムに対して
# > 用いられている共通のモデルが、ロジウムの娘核種であるこれら2元素の同位体にも適用されます。
# > また、OIR第3部（ICRP, 2017）において、ルテニウムの娘核種であるテクネチウムに対して
# > 用いられているモデルが、ロジウムの娘核種であるテクネチウムにも適用されます。

# ICRP Publ.137 p.37 Para.78
# > (78) The radioactive progeny addressed in the derivation of dose coefficients for
# > ruthenium isotopes are isotopes of rhodium or technetium. Rhodium and ruthenium
# > have similar chemical properties and appear, from limited comparative data, to have
# > broadly similar biokinetic in rats. Therefore, rhodium produced in vivo following
# > intake of ruthenium is assigned the systemic biokinetic model for ruthenium.
# > Technetium as a member of a ruthenium chain is assigned the systemic model for
# > technetium as a parent described in OIR Part 2 (ICRP, 2016). Technetium atoms
# > produced in a systemic compartment of the ruthenium model that is identifiable with
# > a compartment of the characteristic model for technetium [i.e. the model applied in
# > OIR Part 2 to technetium as a parent radionuclide; ICRP (2016)] are assigned the
# > characteristic model for technetium from their time of production.
# > (79) Technetium atoms produced in compartments of the ruthenium model that
# > are ambiguous with regard to the characteristic model for technetium (Blood 2 and
# > other soft tissue compartments) are assigned a transfer coefficient to the blood compartment
# > of the technetium model (blood), and upon reaching blood are assigned the
# > characteristic model for technetium. The blood compartment of the technetium
# > model is identified with the central blood compartment of the ruthenium model,
# > named Blood 1. Technetium atoms produced in compartments of other soft tissues
# > of the ruthenium model are assumed to transfer to blood at the rate 0.462 d⁻¹, the
# > highest transfer coefficient from a compartment of other soft tissues to blood in the
# > technetium model, excluding a rapid-turnover compartment representing extracellular
# > fluid. Technetium atoms produced in Blood 2 of the ruthenium model are
# > assumed to transfer to blood at the rate 1000 d⁻¹, a default value used in this publication
# > to represent rapid biological removal.
# > (78) ルテニウム同位体の線量係数の導出において考慮される放射性娘核種は、ロジウムまたは
# > テクネチウムの同位体です。ロジウムとルテニウムは化学的性質が類似しており、限られた
# > 比較データによれば、ラットにおける生体内動態も概ね同様であると考えられます。したがって、
# > ルテニウムの摂取後に生体内で生成されるロジウムには、ルテニウムの全身生体内動態モデルが
# > 適用されます。ルテニウムの崩壊系列に含まれるテクネチウムには、『OIR Part 2』（ICRP, 2016）で
# > 示されている、親核種としてのテクネチウムの全身モデルが適用されます。ルテニウムモデルの
# > 全身コンパートメント内で生成されたテクネチウム原子のうち、テクネチウムの固有モデル
# > （すなわち、『OIR Part 2』（ICRP, 2016）において親放射性核種としてのテクネチウムに
# > 適用されるモデル）のコンパートメントと対応付けられるものについては、生成された時点から
# > テクネチウムの固有モデルが適用されます。(79) ルテニウムモデルのコンパートメント内で
# > 生成されたテクネチウム原子のうち、テクネチウムの固有モデルとの対応関係が明確でないもの
# > （「血液2」およびその他の軟組織コンパートメント）については、テクネチウムモデルの
# > 血液コンパートメント（「血液」）への移行係数が割り当てられ、血液に到達した時点で
# > テクネチウムの固有モデルが適用されます。テクネチウムモデルの血液コンパートメントは、
# > ルテニウムモデルにおける中心的な血液コンパートメントである「血液1」と同一視されます。
# > ルテニウムモデルの「その他の軟組織」コンパートメント内で生成されたテクネチウム原子は、
# > 0.462 d⁻¹の速度で血液へ移行すると仮定されます。この値は、テクネチウムモデルにおいて
# > 「その他の軟組織」コンパートメントから血液への移行係数の中で最大のものです
# > （細胞外液を表す急速代謝コンパートメントは除く）。ルテニウムモデルの「血液2」内で
# > 生成されたテクネチウム原子は、1000 d⁻¹の速度で血液へ移行すると仮定されます。この値は、
# > 急速な生物学的除去を表すために本報告書で用いられるデフォルト値です。

  {Rh,Ru}/Blood1          Blood                     ---
  {Rh,Ru}/Blood2          Blood                  1000

  {Rh,Ru}/C-bone-S        C-bone-S                  ---
  {Rh,Ru}/T-bone-S        T-bone-S                  ---
  {Rh,Ru}/C-bone-V        C-bone-V                  ---
  {Rh,Ru}/T-bone-V        T-bone-V                  ---

  {Rh,Ru}/Liver1          Liver1                    ---
  {Rh,Ru}/Liver2          Liver2                    ---
  {Rh,Ru}/Kidneys1        Kidneys1                  ---
  {Rh,Ru}/Kidneys2        Kidneys2                  ---

  {Rh,Ru}/ST0             Blood                    50
  {Rh,Ru}/ST1             Blood                     0.462
  {Rh,Ru}/ST2             Blood                     0.462
