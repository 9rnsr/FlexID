# ICRP Publ.141 p.235 Para.566 / アクチニド子孫元素としてのアクチニウム
# > (566) The models applied here to actinium, protactinium, neptunium, plutonium,
# > americium, curium, berkelium, californium, einsteinium, and fermium as actinide
# > progeny are modifications of their characteristic models described earlier in this
# > section. ...
# > (566) アクチニドの娘核種であるアクチニウム、プロトアクチニウム、ネプツニウム、
# > プルトニウム、アメリシウム、キュリウム、バークリウム、カリホルニウム、
# > アインスタイニウム、およびフェルミウムに対してここで適用されるモデルは、
# > 本節の冒頭で述べた各元素の特性モデルを修正したものです。 ...

# ICRP Publ.137 p.421 Para.919 / ウラン子孫元素としてのアクチニウム
# > (919) The models for actinium, thorium, radium, radon, polonium, lead, bismuth,
# > thallium, francium, and astatine as radium or thorium progeny are applied to these
# > elements as uranium progeny. The models for protactinium and uranium as thorium
# > progeny are applied to protactinium and uranium, respectively, as uranium progeny.
# > (919) ラジウムまたはトリウムの娘核種としての、アクチニウム、トリウム、ラジウム、
# > ラドン、ポロニウム、鉛、ビスマス、タリウム、フランシウム、およびアスタチンのモデルは、
# > ウランの娘核種としてのこれら元素にも適用されます。トリウムの娘核種としてのプロトアクチニウム
# > およびウランのモデルは、ウランの娘核種としてのプロトアクチニウムおよびウランに、
# > それぞれ適用されます。

# ICRP Publ.137 p.370 Para.812 / トリウム子孫元素としてのアクチニウム
# > (812) The models for actinium, thorium, radium, radon, polonium, lead, bismuth,
# > thallium, francium, and astatine produced systemically by serial decay of members of
# > a thorium chain are the same as the models applied to these elements as progeny of
# > radium (see Section 13). The model for thorium as a radium progeny is applied to
# > protactinium as a thorium progeny based on the similar systemic behaviours of
# > protactinium and thorium in rats (Lanz et al., 1946; Schuppler et al., 1988;
# > Durbin, 2011). A radionuclide produced in a bone volume compartment that is
# > ambiguous with regard to the model for that element (e.g. radium produced in trabecular
# > or cortical bone volume in the thorium model) is assumed to be produced in
# > non-exchangeable bone.
# > (812) トリウム系列の各元素の連続崩壊によって体系的に生成されるアクチニウム、
# > トリウム、ラジウム、ラドン、ポロニウム、鉛、ビスマス、タリウム、フランシウム、
# > およびアスタチンのモデルは、ラジウムの子孫元素としてのこれらの元素に適用される
# > モデルと同じになります（第13節参照）。ラジウムの子孫元素としてのトリウムのモデルは、
# > ラットにおけるプロトアクチニウムとトリウムの類似した全身的挙動に基づいて、
# > トリウムの子孫元素としてのプロトアクチニウムにも適用されます(Lanz et al., 1946;
# > Schuppler et al., 1988; Durbin, 2011)。骨体積コンパートメントで生成される
# > 放射性核種のうち、その元素のモデルに関して曖昧なもの（例えば、トリウムモデルにおいて
# > 骨梁骨または皮質骨体積で生成されるラジウム）は、非交換性骨で生成されると仮定されます。

[{Ac}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other

$if progeny

# ICRP Publ.141 p.235 Para.566 / アクチニド子孫元素としてのアクチニウム
# > (566) ... For a given element in this group, two compartments representing the
# > skin and spleen are added to the explicitly identified source regions in the element's
# > characteristic model. ...
# > (566) ... このグループの特定の元素について、その特性モデルにおいて明示的に特定された
# > 線源領域に加え、皮膚と脾臓を表す2つのコンパートメントが追加されます。 ...

# ICRP Publ.137 p.333 Para.714 / ラジウム子孫元素としてのアクチニウム
# > (714) For application to actinium as a progeny of radium, two compartments representing
# > the spleen and skin are added to the explicitly identified source regions in the
# > characteristic model for actinium for consistency with the source regions addressed in
# > models for other radium progeny. ...
# > (714) ラジウムの子孫核種としてのアクチニウムへの適用では、他のラジウム子孫核種のモデルで
# > 扱われている線源領域との整合性を図るため、アクチニウムの特性モデルで明示的に特定された
# > 線源領域に、脾臓と皮膚を表す2つのコンパートメントが追加されます。 ...

  acc   Skin                  Skin
  acc   Spleen                Spleen

$endif

[{Ac}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.141 p.234 Table 18.9
  Blood                   Liver1                   11.6
  Blood                   T-bone-S                  3.49
  Blood                   C-bone-S                  3.49
  Blood                   Kidneys1                  0.466
  Blood                   Kidneys2                  0.116
  Blood                   UB-con                    1.63
  Blood                   RC-con                    0.303
  Blood                   Testes                    0.0082
  Blood                   Ovaries                   0.0026
  Blood                   ST0                      10.0
$if parent
  Blood                   ST1                       1.67
$endif
  Blood                   ST2                       0.466
  Liver1                  SI-conRe                  0.0006
  Liver1                  Liver2                    0.0225
  Liver1                  Blood                     0
  Liver2                  Blood                     0.0019
  T-bone-S                T-marrow                  4.93E-4
  T-bone-S                T-bone-V                  2.47E-4
  T-bone-V                T-marrow                  4.93E-4
  T-marrow                Blood                     0.0076
  C-bone-S                C-marrow                  8.21E-5
  C-bone-S                C-bone-V                  4.11E-5
  C-bone-V                C-marrow                  8.21E-5
  C-marrow                Blood                     0.0076
  C-marrow                C-bone-S                  0
  Kidneys1                UB-con                    0.099
  Kidneys2                Blood                     0.00139
  Testes                  Blood                     0.00038
  Ovaries                 Blood                     0.00038
  ST0                     Blood                     1.39
  ST1                     Blood                     0.0139
  ST2                     Blood                     1.9E-5

$if progeny

# ICRP Publ.141 p.235 Para.566 / アクチニド子孫元素としてのアクチニウム
# > (566) ... These compartments are taken from the intermediate soft tissue
# > compartment, ST1; that is, the deposition fraction for ST1 is reduced by the deposition
# > fractions assigned to the spleen and skin, and the removal half-time from ST1 is
# > assigned to these added compartments. Deposition of the element in the skin is
# > calculated as its mass fraction of other soft tissue times its deposition fraction in
# > other soft tissue excluding the rapid-turnover compartment, ST0. The deposition
# > fraction for the spleen is set at one-third of the deposition fraction for the skin,
# > considering the relative masses of these tissues and the typically higher concentrations
# > of the actinides in the spleen than the skin observed in laboratory animals and
# > humans. ...
# > (566) ... これらのコンパートメント(訳注：皮膚と脾臓)は、中間代謝軟組織コンパートメントST1から
# > 取得されます。すなわち、ST1の沈着割合は脾臓と皮膚に割り当てられた沈着割合によって減少し、
# > ST1からの除去半減期がこれらの追加されたコンパートメントに割り当てられます。皮膚における
# > 元素の沈着は、その他の軟組織の質量割合と、急速代謝コンパートメントST0を除く
# > その他の軟組織の沈着割合を乗じて計算されます。脾臓への沈着割合は、これらの組織の相対質量と、
# > 実験動物やヒトにおいて脾臓の方が皮膚よりもアクチニドの濃度が通常高いことを考慮して、
# > 皮膚への沈着割合の3分の1に設定されます。 ...

# ICRP Publ.137 p.333 Para.714 / ラジウム子孫元素としてのアクチニウム
# > (714) ... Skin and spleen are taken from the intermediate soft
# > tissue compartment ST1; that is, the deposition fraction for ST1 is reduced by the
# > deposition fractions assigned to spleen and skin, and the removal half-time from ST1
# > to blood is assigned to spleen and skin. Deposition in skin is calculated as its mass
# > fraction of other soft tissues times its deposition fraction in other soft tissues, excluding
# > deposition in the fast-turnover compartment ST0. The deposition fraction for spleen is
# > set at one-third of the deposition fraction for skin, considering the relative masses of
# > these tissues and the typically higher concentrations of actinides in spleen than skin
# > observed in laboratory animals and human subjects.
# > (714) ... 皮膚と脾臓は中間代謝軟組織コンパートメントST1から取得されます。すなわち、
# > ST1の沈着割合は脾臓と皮膚に割り当てられた沈着割合によって減少し、ST1から血液への
# > 除去半減期が脾臓と皮膚に割り当てられます。皮膚における沈着は、その他の軟組織の質量割合と、
# > 急速代謝コンパートメントST0への沈着を除くその他の軟組織への沈着割合を乗じて計算されます。
# > 脾臓への沈着割合は、これらの組織の相対質量と、実験動物やヒトにおいて脾臓の方が皮膚よりも
# > アクチニドの濃度が通常高いことを考慮して、皮膚への沈着割合の3分の1に設定されます。

# 作成者注：
#   親核種としての、SkinとSpleenを明示していないアクチニウムモデルにおける軟組織の総質量は
#   sregions_2016-08-12.NDX で定義された男性の線源領域から以下のように計算される。
#     O-mucosa  + Teeth-V   + Tonsils   + Oesophag-w + St-wall   + SI-wall   +
#     RC-wall   + LC-wall   + RS-wall   + LN-ET      + LN-Th     + Adrenals  +
#     Brain     + Breast    + Eye-lens  + GB-wall    + Ht-wall   + LN-Sys    +
#     Pancreas  + P-gland   + Prostate  + S-glands   + Thymus    + Thyroid   +
#     Ureters   + UB-wall   + Uterus    + Adipose    + Cartilage + Muscle    +
#     ET1-wall  + ET2-wall  + Lung-Tis  + Skin       + Spleen
#   = 3.583E-02 + 5.000E-02 + 3.000E-03 + 4.000E-02  + 1.500E-01 + 6.500E-01 +
#     1.500E-01 + 1.500E-01 + 7.000E-02 + 1.500E-02  + 1.500E-02 + 1.400E-02 +
#     1.450E+00 + 2.500E-02 + 4.000E-04 + 1.000E-02  + 3.300E-01 + 1.484E-01 +
#     1.400E-01 + 6.000E-04 + 1.700E-02 + 8.500E-02  + 2.500E-02 + 2.000E-02 +
#     1.600E-02 + 5.000E-02 + 0.000E+00 + 1.723E+01  + 1.100E+00 + 2.900E+01 +
#     2.923E-03 + 2.923E-03 + 5.000E-01 + 3.300E+00  + 1.500E-01
#   = 54.946076
#
#   同様に、親核種としてのモデルにおいて血液からST0以外の軟組織に沈着する割合は
#     Blood → ST1      = 1.67
#     Blood → ST2      = 0.466
#   であるため、血液からST1,ST2の軟組織に沈着する総割合は 1.67 + 0.466 = 2.136 となる。
#
#   これらからの数値から、子孫核種における移行係数は以下のように計算される。
#   血液から皮膚への沈着割合は、軟組織(ST1 + ST2)に対する皮膚の質量分率より、
#     Blood → Skin     = (1.67 + 0.466) * (3.300E+00 / 54.946076) ≒ 0.128286
#
#   血液から脾臓への沈着割合は、皮膚への沈着率の1/3となることから、
#     Blood → Spleen   = 0.128286 / 3 ≒ 0.042762
#
#   血液からST1への沈着割合は、皮膚と脾臓に割り当てられた分を差し引くことになり、
#     Blood → ST1      = 1.67 - 0.128286 - 0.042762 ≒ 1.498952

  Blood                   Skin                  $(0.128286)
  Blood                   Spleen                $(0.128286 / 3)
  Blood                   ST1                   $(1.67 - 0.128286 - 0.128286/3)

  Skin                    Blood                     0.0139      # ST1→Bloodの移行速度
  Spleen                  Blood                     0.0139      # ST1→Bloodの移行速度

# ICRP Publ.141 p.235 Para.566 / アクチニド子孫元素としてのアクチニウム
# > (566) ... If the element is produced in a compartment that is not identifiable with a
# > compartment in its characteristic model, it is assumed to transfer to the element's
# > blood compartment [Blood 1 in the case of plutonium, which has multiple blood
# > compartments (see Table 18.6)] at the rate 1000 d⁻¹ if produced in a blood compartment,
# > at the rate of transfer from the fast-turnover soft tissue compartment ST0 to
# > blood if produced in a soft tissue compartment, and at the rate of bone turnover if
# > produced in a bone volume compartment.
# > (566) ... もし元素が、その特性モデル上のコンパートメントと同一視できない
# > コンパートメント内で生成された場合、血液コンパートメント内で生成された場合は1000 d⁻¹の
# > 速度で、軟部組織コンパートメント内で生成された場合は代謝の早い軟部組織コンパートメント
# > ST0から血液への移行速度で、骨体積コンパートメント内で生成された場合は骨代謝速度で、
# > その元素の血液コンパートメント［複数の血液コンパートメントを持つプルトニウムの場合は
# > Blood 1（表18.6参照）］へ移行すると仮定されます。

# ICRP Publ.137 p.333 Para.714 / ラジウム子孫元素としてのアクチニウム
# > (714) ... If actinium is produced in a
# > compartment that is not identifiable with a compartment in its characteristic model,
# > it is assumed to transfer to actinium’s central blood compartment at the rate 1000 d⁻¹
# > if produced in a blood compartment, at the rate of transfer from the fast-turnover soft
# > tissue compartment ST0 to blood (1.386 d⁻¹) if produced in a soft tissue compartment,
# > and at the rate of bone turnover if produced in a bone volume compartment.
# > (714) ... アクチニウムが、その特性モデルのコンパートメントと識別
# > できないコンパートメントで生成される場合、血液コンパートメントで生成された場合は1000 d⁻¹の
# > 速度でアクチニウムの中心血液コンパートメントに、軟部組織コンパートメントで生成された場合は
# > 回転の速い軟部組織コンパートメントST0から血液への移行速度（1.386 d⁻¹）で、骨量コンパートメントで
# > 生成された場合は骨代謝速度でアクチニウムの中心血液コンパートメントに移行すると仮定されます。

  {*}/Blood               Blood                     ---
  {Pu}/Blood0             Blood                  1000
  {Pu}/Blood1             Blood                  1000
  {Pu}/Blood2             Blood                  1000
  {*}/Plasma              Blood                  1000
  {Po}/Plasma1            Blood                  1000
  {Po}/Plasma2            Blood                  1000
  {Po}/Plasma3            Blood                  1000
  {*}/RBC                 Blood                  1000

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {*}/Exch-C-bone-V       Blood                 $(0.03 / 365)
  {*}/Exch-T-bone-V       Blood                 $(0.18 / 365)
  {*}/Noch-C-bone-V       C-bone-V                  ---
  {*}/Noch-T-bone-V       T-bone-V                  ---
  {*}/C-bone-V            C-bone-V                  ---
  {*}/T-bone-V            T-bone-V                  ---

  {*}/C-marrow            C-marrow                  ---
  {*}/T-marrow            T-marrow                  ---
  {Pu}/Liver0             Blood                     1.39    # ≒1.386   # ST0→Bloodの移行速度
  {*}/Liver1              Liver1                    ---
  {*}/Liver2              Liver2                    ---
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Ac}/ST0                ST0                       ---
  {Ac}/ST1                ST1                       ---
  {Ac}/ST2                ST2                       ---
  {^Ac}/ST0               Blood                     1.39    # ≒1.386   # ST0→Bloodの移行速度
  {^Ac}/ST1               Blood                     1.39    # ≒1.386   # ST0→Bloodの移行速度
  {^Ac}/ST2               Blood                     1.39    # ≒1.386   # ST0→Bloodの移行速度
  {*}/Other               Blood                     1.39    # ≒1.386   # ST0→Bloodの移行速度

$endif
