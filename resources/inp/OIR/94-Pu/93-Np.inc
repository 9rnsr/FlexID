[{Np}:compartment]
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

# > ICRP Publ.141 p.235 Para.566 / アクチニド子孫元素としてのネプツニウム
# > (566) The models applied here to actinium, protactinium, neptunium, plutonium,
# > americium, curium, berkelium, californium, einsteinium, and fermium as actinide
# > progeny are modifications of their characteristic models described earlier in this
# > section. For a given element in this group, two compartments representing the
# > skin and spleen are added to the explicitly identified source regions in the element’s
# > characteristic model. ...
# > (566) アクチニドの娘核種であるアクチニウム、プロトアクチニウム、ネプツニウム、
# > プルトニウム、アメリシウム、キュリウム、バークリウム、カリホルニウム、
# > アインスタイニウム、およびフェルミウムに対してここで適用されるモデルは、
# > 本節の冒頭で述べた各元素の特性モデルを修正したものである。このグループの特定の元素に
# > ついて、その特性モデルにおいて明示的に特定された線源領域に加え、皮膚と脾臓を表す2つの
# > コンパートメントが追加される。 ...

  acc   Skin                  Skin
  acc   Spleen                Spleen

[{Np}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.141 p.234 Table 18.9
  Blood                   Liver1                    0.194
  Blood                   T-bone-S                  0.480
  Blood                   C-bone-S                  0.393
  Blood                   Kidneys1                  0.0291
  Blood                   Kidneys2                  0.0097
  Blood                   UB-con                    0.621
  Blood                   RC-con                    0.0136
  Blood                   Testes                    0.00068
  Blood                   Ovaries                   0.00021
  Blood                   ST0                       0.832
 #Blood                   ST1                       0.161       # fix
  Blood                   ST2                       0.0388
  Liver1                  SI-conRe                  0.000133
  Liver1                  Liver2                    0.00177
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
  Kidneys1                UB-con                    0.0495
  Kidneys2                Blood                     0.00139
  Testes                  Blood                     0.00038
  Ovaries                 Blood                     0.00038
  ST0                     Blood                     0.693
  ST1                     Blood                     0.00693
  ST2                     Blood                     1.9E-5

# > ICRP Publ.141 p.235 Para.566 / アクチニド子孫元素としてのネプツニウム
# > (566) ... For a given element in this group, two compartments representing the
# > skin and spleen are added to the explicitly identified source regions in the element’s
# > characteristic model. These compartments are taken from the intermediate soft tissue
# > compartment, ST1; that is, the deposition fraction for ST1 is reduced by the deposition
# > fractions assigned to the spleen and skin, and the removal half-time from ST1 is
# > assigned to these added compartments. Deposition of the element in the skin is
# > calculated as its mass fraction of other soft tissue times its deposition fraction in
# > other soft tissue excluding the rapid-turnover compartment, ST0. The deposition
# > fraction for the spleen is set at one-third of the deposition fraction for the skin,
# > considering the relative masses of these tissues and the typically higher concentrations
# > of the actinides in the spleen than the skin observed in laboratory animals and
# > humans. ...
# > (566) ... このグループの特定の元素に
# > ついて、その特性モデルにおいて明示的に特定された線源領域に加え、皮膚と脾臓を表す2つの
# > コンパートメントが追加される。これらのコンパートメントは、中間軟部組織コンパートメント
# > ST1から切り出されたものである。すなわち、ST1の沈着分率は、脾臓および皮膚に割り当てられた
# > 沈着分率の分だけ減らされ、ST1からの除去半減期が、これら追加されたコンパートメントに
# > 割り当てられる。皮膚への元素の沈着量は、「その他の軟部組織」に対する質量の割合に、
# > 急速代謝コンパートメントST0を除いた「その他の軟部組織」における沈着分率を乗じることで
# > 算出される。脾臓の沈着分率は、皮膚の沈着分率の3分の1に設定される。これは、これらの
# > 組織の相対的な質量、および実験動物やヒトにおいて皮膚よりも脾臓でアクチニドの濃度が
# > 高くなる傾向が観察されていることを考慮したものである。...

# 作成者注：
#   親核種としての、SkinとSpleenを明示していないネプツニウムモデルにおける軟組織の総質量は
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
#     Blood → ST1      = 0.161
#     Blood → ST2      = 0.0388
#   であるため、血液からST1,ST2の軟組織に沈着する総割合は 0.161 + 0.0388 = 0.1998 となる。
#
#   これらからの数値から、子孫核種における移行係数は以下のように計算される。
#   血液から皮膚への沈着割合は、軟組織(ST1 + ST2)に対する皮膚の質量分率より、
#     Blood → Skin     = (0.161 + 0.0388) * (3.300E+00 / 54.946076) ≒ 0.012
#
#   血液から脾臓への沈着割合は、皮膚への沈着率の1/3となることから、
#     Blood → Spleen   = 0.012 / 3 ≒ 0.004
#
#   血液からST1への沈着割合は、皮膚と脾臓に割り当てられた分を差し引くことになり、
#     Blood → ST1      = 0.161 - 0.012 - 0.004 ≒ 0.145

  Blood                   ST1                   $(0.161 - 0.012 - 0.012/3)
  Blood                   Skin                  $(0.012)
  Blood                   Spleen                $(0.012 / 3)
  Skin                    Blood                     0.00693     # ST1→Bloodの移行速度
  Spleen                  Blood                     0.00693     # ST1→Bloodの移行速度

# > ICRP Publ.141 p.235 Para.566 / アクチニド子孫元素としてのネプツニウム
# > (566) ... If the element is produced in a compartment that is not identifiable with a
# > compartment in its characteristic model, it is assumed to transfer to the element’s
# > blood compartment [Blood 1 in the case of plutonium, which has multiple blood
# > compartments (see Table 18.6)] at the rate 1000 d⁻¹ if produced in a blood compartment,
# > at the rate of transfer from the fast-turnover soft tissue compartment ST0 to
# > blood if produced in a soft tissue compartment, and at the rate of bone turnover if
# > produced in a bone volume compartment.
# > (566) ... もし元素が、その特性モデル上の
# > コンパートメントと同一視できないコンパートメント内で生成された場合、その元素の
# > 血液コンパートメント（複数の血液コンパートメントを持つプルトニウムの場合はBlood 1。
# > 表18.6参照）へ移行すると仮定される。その際の移行速度は、血液コンパートメント内で
# > 生成された場合は1000 d⁻¹、軟部組織コンパートメント内で生成された場合は急速代謝
# > 軟部組織コンパートメントST0から血液への移行速度、そして骨容積コンパートメント内で
# > 生成された場合は骨代謝回転速度となる。

  {*}/Plasma              Blood                  1000
  {Po}/Plasma1            Blood                  1000
  {Po}/Plasma2            Blood                  1000
  {Po}/Plasma3            Blood                  1000
  {*}/RBC                 Blood                  1000
  {Pu}/Blood0             Blood                  1000
  {Pu}/Blood1             Blood                  1000
  {Pu}/Blood2             Blood                  1000
  {*}/Blood               Blood                     ---

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {*}/C-bone-V            C-bone-V                  ---
  {*}/T-bone-V            T-bone-V                  ---
  {*}/Noch-C-bone-V       C-bone-V                  ---
  {*}/Noch-T-bone-V       T-bone-V                  ---
  {*}/Exch-C-bone-V       Blood                 $(0.03 / 365)
  {*}/Exch-T-bone-V       Blood                 $(0.18 / 365)

  {*}/C-marrow            C-marrow                  ---
  {*}/T-marrow            T-marrow                  ---
  {Pu}/Liver0             Blood                     0.693       # ST0→Bloodの移行速度
  {Pu}/Liver1             Blood                     0.693       # ST0→Bloodの移行速度
  {Pu}/Liver2             Blood                     0.693       # ST0→Bloodの移行速度
  {^Pu}/Liver1            Liver1                    ---
  {^Pu}/Liver2            Liver2                    ---
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Np}/ST0                ST0                       ---
  {Np}/ST1                ST1                       ---
  {Np}/ST2                ST2                       ---
  {^Np}/ST0               Blood                     0.693       # ST0→Bloodの移行速度
  {^Np}/ST1               Blood                     0.693       # ST0→Bloodの移行速度
  {^Np}/ST2               Blood                     0.693       # ST0→Bloodの移行速度
  {*}/Other               Blood                     0.693       # ST0→Bloodの移行速度
