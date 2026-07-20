[{Pu}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
$if parent
  acc   Blood0                Blood
$endif
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   Liver0                Liver
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow
  acc   Kidneys1              Kidneys       # Renal tubules
  acc   Kidneys2              Kidneys       # Other kidney
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other

$if progeny

# > ICRP Publ.141 p.235 Para.566
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

$endif

[{Pu}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.141 p.225 Table 18.6
$if parent
  Blood0                  ST0                       3.0000E+2
  Blood0                  Blood1                    7.0000E+2
$endif
  Blood1                  Liver0                    4.6200E-1
  Blood1                  C-bone-S                  8.7780E-2
  Blood1                  C-bone-V                  4.6200E-3
  Blood1                  T-bone-S                  1.2474E-1
  Blood1                  T-bone-V                  1.3860E-2
  Blood1                  UB-con                    1.5400E-2
  Blood1                  Kidneys1                  7.7000E-3
  Blood1                  Kidneys2                  3.8500E-4
  Blood1                  RC-con                    1.1550E-2
  Blood1                  Testes                    2.6950E-4
  Blood1                  Ovaries                   0.8470E-4
$if parent
  Blood1                  ST1                       1.8511E-2
$endif
  Blood1                  ST2                       2.3100E-2
  ST0                     Blood1                    9.9000E-2
  Blood2                  UB-con                    3.5000E+0
  Blood2                  Blood1                    6.7550E+1
  Blood2                  ST0                       2.8950E+1
  Kidneys1                UB-con                    1.7329E-2
  Kidneys2                Blood2                    1.2660E-4
  ST1                     Blood2                    1.3860E-3
  ST2                     Blood2                    1.2660E-4
  Liver0                  SI-conRe                  9.2420E-4
  Liver0                  Liver1                    4.5286E-2
  Liver1                  Blood2                    1.5200E-3
  Liver1                  Liver2                    3.8000E-4
  Liver2                  Blood2                    1.2660E-4
  Testes                  Blood2                    3.8000E-4
  Ovaries                 Blood2                    3.8000E-4
  C-bone-S                C-marrow                  8.2100E-5
  C-bone-S                C-bone-V                  2.0500E-5
  C-bone-V                C-marrow                  8.2100E-5
  T-bone-S                T-marrow                  4.9300E-4
  T-bone-S                T-bone-V                  1.2300E-4
  T-bone-V                T-marrow                  4.9300E-4
  C-marrow                Blood2                    7.6000E-3
  T-marrow                Blood2                    7.6000E-3

$if progeny

# > ICRP Publ.141 p.235 Para.566
# > (566) ... For a given element in this group, two compartments representing the
# > skin and spleen are added to the explicitly identified source regions in the element’s
# > characteristic model. These compartments are taken from the intermediate soft tissue
# > compartment, ST1; that is, the deposition fraction for ST1 is reduced by the deposition
# > fractions assigned to the spleen and skin, and the removal half-time from ST1 is
# > assigned to these added compartments. Deposition of the element in the skin is
# > calculated as its mass fraction of other soft tissue times its deposition fraction in
# > other soft tissue excluding the rapid-turnover compartment, ST0. The deposition
# > fraction for the spleen is set at one-third of the deposition fraction for the skin, ...
# > (566) ... このグループの特定の元素に
# > ついて、その特性モデルにおいて明示的に特定された線源領域に加え、皮膚と脾臓を表す2つの
# > コンパートメントが追加される。これらのコンパートメントは、中間軟部組織コンパートメント
# > ST1から切り出されたものである。すなわち、ST1の沈着分率は、脾臓および皮膚に割り当てられた
# > 沈着分率の分だけ減らされ、ST1からの除去半減期が、これら追加されたコンパートメントに
# > 割り当てられる。皮膚への元素の沈着量は、「その他の軟部組織」に対する質量の割合に、
# > 急速代謝コンパートメントST0を除いた「その他の軟部組織」における沈着分率を乗じることで
# > 算出される。脾臓の沈着分率は、皮膚の沈着分率の3分の1に設定される。 ...

# 作成者注：
#   親核種としての、SkinとSpleenを明示していないプルトニウムモデルにおける軟組織の総質量は
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
#     2.923E-03 + 2.923E-03 + 5.000E-01 + 3.300E+00  + 1.500E-01 +
#   = 54.946076
#
#   同様に、親核種としてのモデルにおいて血液からST0以外の軟組織に沈着する割合は
#     Blood1 → ST1     = 1.8511E-2
#     Blood1 → ST2     = 2.3100E-2
#   であるため、血液からST1,ST2の軟組織に沈着する総割合は 1.8511E-2 + 2.3100E-2 = 4.1611E-2 となる。
#
#   これらからの数値から、子孫核種における移行係数は以下のように計算される。
#   血液から皮膚への沈着割合は、軟組織(ST1 + ST2)に対する皮膚の質量分率より、
#     Blood1 → Skin    = (1.8511E-2 + 2.3100E-2) * (3.300E+00 / 54.946076) ≒ 2.4991E-03
#
#   血液から脾臓への沈着割合は、皮膚への沈着率の1/3となることから、
#     Blood1 → Spleen  = 2.4991E-03 / 3 ≒ 8.3303E-04
#
#   血液からST1への沈着割合は、皮膚と脾臓に割り当てられた分を差し引くことになり、
#     Blood1 → ST1     = 1.8511E-2 - 2.4991E-03 - 8.3303E-04 ≒ 1.5179E-02

  Blood1                  ST1                   $(1.8511E-2 - 2.4991E-03 - 2.4991E-03/3)
  Blood1                  Skin                  $(2.4991E-03)
  Blood1                  Spleen                $(2.4991E-03 / 3)

  Skin                    Blood2                    1.3860E-3       # ST1→Blood2の移行速度
  Spleen                  Blood2                    1.3860E-3       # ST1→Blood2の移行速度

# > ICRP Publ.141 p.236 Para.567
# > (567) The model for plutonium as a progeny is further modified by removing the
# > transfers from blood to ST0 and from blood to Blood 1 (Table 18.6), and adding a
# > transfer of 0.33 d⁻¹ from Blood 1 to ST0. This simplifies the model for plutonium as
# > a progeny by eliminating a blood compartment (‘Blood 0’ in Table 18.6). The added
# > transfer coefficient of 0.33 d⁻¹ from Blood 1 to ST0 implies that ST0 receives 30% of
# > plutonium leaving Blood 1. Total deposition in ST0 is virtually the same as in the
# > model for plutonium as a parent, but the rate of accumulation of plutonium in ST0 is
# > substantially lower in this simplified version of the model.
# > (567) 娘核種としてのプルトニウムに関するモデルは、血液からST0への移行および血液から
# > Blood 1への移行（表18.6）を削除し、Blood 1からST0への0.33 d⁻¹の移行を追加することに
# > よって、さらに修正される。これにより、血液コンパートメント（表18.6における「Blood 0」）が
# > 排除され、娘核種としてのプルトニウムのモデルが簡素化される。Blood 1からST0への0.33 d⁻¹
# > という追加された移行係数は、Blood 1から流出するプルトニウムの30%がST0に移行することを
# > 意味する。ST0における総沈着量は、親核種としてのプルトニウムのモデルにおける量と実質的に
# > 同じであるが、ST0におけるプルトニウムの蓄積速度は、この簡素化されたモデルでは大幅に
# > 低くなっている。

  Blood1                  ST0                       0.33

# > ICRP Publ.141 p.235 Para.566
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

  {*}/Plasma              Blood1                 1000
  {Po}/Plasma1            Blood1                 1000
  {Po}/Plasma2            Blood1                 1000
  {Po}/Plasma3            Blood1                 1000
  {*}/RBC                 Blood1                 1000
  {Pu}/Blood0             Blood0                    ---
  {Pu}/Blood1             Blood1                    ---
  {Pu}/Blood2             Blood2                    ---
  {*}/Blood               Blood1                 1000

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {*}/C-bone-V            C-bone-V                  ---
  {*}/T-bone-V            T-bone-V                  ---
  {*}/Noch-C-bone-V       C-bone-V                  ---
  {*}/Noch-T-bone-V       T-bone-V                  ---
  {*}/Exch-C-bone-V       Blood1                $(0.03 / 365)
  {*}/Exch-T-bone-V       Blood1                $(0.18 / 365)

  {*}/C-marrow            C-marrow                  ---
  {*}/T-marrow            T-marrow                  ---
  {Pu}/Liver0             Liver0                    ---
  {Pu}/Liver1             Liver1                    ---
  {Pu}/Liver2             Liver2                    ---
  {^Pu}/Liver1            Blood1                    9.9000E-2   # ST0→Blood1の移行速度
  {^Pu}/Liver2            Blood1                    9.9000E-2   # ST0→Blood1の移行速度
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Pu}/ST0                ST0                       ---
  {Pu}/ST1                ST1                       ---
  {Pu}/ST2                ST2                       ---
  {^Pu}/ST0               Blood1                    9.9000E-2   # ST0→Blood1の移行速度
  {^Pu}/ST1               Blood1                    9.9000E-2   # ST0→Blood1の移行速度
  {^Pu}/ST2               Blood1                    9.9000E-2   # ST0→Blood1の移行速度
  {*}/Other               Blood1                    9.9000E-2   # ST0→Blood1の移行速度

$endif
