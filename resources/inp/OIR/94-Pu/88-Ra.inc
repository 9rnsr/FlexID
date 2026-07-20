[{Ra}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   Noch-T-bone-V         T-bone-V
  acc   Liver1                Liver         # Para.694, fast
  acc   Liver2                Liver         # Para.694, slow
  acc   Kidneys1              Kidneys       # Para.694, fast
  acc   Kidneys2              Kidneys       # Para.694, slow
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other

#    ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としてのラジウムは、ラジウム子孫元素としてのモデルを用いる
# → ICRP Publ.137 p.334 Para.716 / ラジウム子孫元素としてのラジウム
# > (716) ... Single compartments representing spleen, trabecular marrow,
# > cortical marrow, testes, ovaries, and skin are added for consistency
# > with the models for other progeny of radium.  ...
# > (716) ... ラジウムの他の子孫核種のモデルとの整合性を保つため、脾臓、
# > 骨梁骨髄、皮質骨髄、精巣、卵巣、皮膚を表す個々のコンパートメントが
# > 追加されました。 ...

  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen

[{Ra}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.332 Table 13.3
  Blood                   UB-con                    0.606
  Blood                   RC-con                   21.79
  Blood                   T-bone-S                  9.72
  Blood                   C-bone-S                  7.78
  Blood                   ST0                      20.93
 #Blood                   ST1                       3.5         # fix
 #Blood                   ST2                       0.07        # fix
  Blood                   Liver1                    4.2
  Blood                   Kidneys1                  1.4
  T-bone-S                Blood                     0.578
  T-bone-S                Exch-T-bone-V             0.116
  C-bone-S                Blood                     0.578
  C-bone-S                Exch-C-bone-V             0.116
  ST0                     Blood                     6.98
  ST1                     Blood                     0.693
  ST2                     Blood                     0.00038
  Liver1                  Blood                     0.691
  Liver1                  Liver2                    0.00208
  Liver2                  Blood                     0.0019
  Kidneys1                Blood                     2.073
  Kidneys1                Kidneys2                  0.00624
  Kidneys2                Blood                     0.0019
  Exch-T-bone-V           T-bone-S                  0.0185
  Exch-T-bone-V           Noch-T-bone-V             0.0046
  Exch-C-bone-V           C-bone-S                  0.0185
  Exch-C-bone-V           Noch-C-bone-V             0.0046
  Noch-C-bone-V           Blood                     0.0000821
  Noch-T-bone-V           Blood                     0.000493

# ICRP Publ.137 p.334 Para.716 / ラジウム子孫元素としてのラジウム
# > (716) ... The six added compartments are taken from the intermediate-and
# > long-term soft tissue compartments ST1 and ST2, respectively, in the
# > model for radium as a parent. ... The deposition fractions for ST1 and ST2
# > are reduced uniformly (each by the same percentage) for mass balance. ...
# > (716) ... 追加された6つのコンパートメントは、ラジウムを親核種とする
# > モデルにおける中期および長期の軟部組織コンパートメントST1およびST2から
# > それぞれ抽出されます。 ... ST1とST2の沈着分率は、質量バランスを保つため、
# > 均一に（それぞれ同じ割合で）減少します。 ...
#
#   Blood → {Spleen + T-marrow + C-marrow + Testes + Overies + Skin}
#   0.43208 = 0.0093 + 0.165    + 0.055    + 0.0021 + 0.00068 + 0.2

  Blood                   ST1               $(  3.5  * (1 - 0.43208/(3.5+0.07))  )
  Blood                   ST2               $(  0.07 * (1 - 0.43208/(3.5+0.07))  )

# ICRP Publ.137 p.334 Para.716 / ラジウム子孫元素としてのラジウム
# > (716) ... The derived transfer cofficients from blood to spleen,
# > trabecular marrow, cortical marrow, testes, ovaries, and skin are
# > 0.0093 d⁻¹, 0.165 d⁻¹, 0.055 d⁻¹, 0.0021 d⁻¹, 0.00068 d⁻¹, and
# > 0.2 d⁻¹, respectively. The removal rate from each added compartment is
# > set at 0.02 d⁻¹, which is a rounded value based on an effective
# > removal half-time from ST1+ST2. ...
# > (716) ... 血液から脾臓、骨梁骨髄、皮質骨髄、精巣、卵巣、皮膚への
# > 導出された移行係数は、それぞれ0.0093 d⁻¹、0.165 d⁻¹、0.055 d⁻¹、
# > 0.0021 d⁻¹、0.00068 d⁻¹、0.2 d⁻¹である。追加された各コンパートメントからの
# > 除去率は0.02 d⁻¹に設定されており、これはST1+ST2からの有効除去半減時間に
# > 基づいて丸められた値である。 ...

  Blood                   Spleen                    0.0093
  Blood                   T-marrow                  0.165
  Blood                   C-marrow                  0.055
  Blood                   Testes                    0.0021
  Blood                   Ovaries                   0.00068
  Blood                   Skin                      0.2
  Spleen                  Blood                     0.02
  T-marrow                Blood                     0.02
  C-marrow                Blood                     0.02
  Testes                  Blood                     0.02
  Ovaries                 Blood                     0.02
  Skin                    Blood                     0.02

# ICRP Publ.137 p.334 Para.716 / ラジウム子孫元素としてのラジウム
# > (716) ... Radium produced in a blood compartment of
# > a preceding chain member that is not identifiable with a compartment in the radium
# > model is assumed to transfer to the central blood compartment of the radium model at
# > the following rates: 1000 d⁻¹ if produced in a blood compartment, and 6.98 d⁻¹ if
# > produced in a soft tissue compartment. The value 6.98 d⁻¹ is the highest rate of removal
# > from a compartment of other soft tissue in the characteristic model for radium.
# > ラジウムモデル内のコンパートメントと識別できない先行する連鎖メンバーの血液コンパートメントで
# > 生成されたラジウムは、ラジウムモデルの中心となる血液コンパートメントに、以下の速度で
# > 移行すると仮定される。血液コンパートメントで生成された場合は1000 d⁻¹、
# > 軟部組織コンパートメントで生成された場合は6.98 d⁻¹である。6.98 d⁻¹という値は、
# > ラジウムの特性モデルにおける他の軟部組織コンパートメントからの除去速度の最大値である。

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
  {*}/C-bone-V            Noch-C-bone-V             ---     # Publ.137 Para.812
  {*}/T-bone-V            Noch-T-bone-V             ---     # Publ.137 Para.812
  {*}/Noch-C-bone-V       Noch-C-bone-V             ---
  {*}/Noch-T-bone-V       Noch-T-bone-V             ---
  {*}/Exch-C-bone-V       Exch-C-bone-V             ---
  {*}/Exch-T-bone-V       Exch-T-bone-V             ---

  {*}/C-marrow            C-marrow                  ---
  {*}/T-marrow            T-marrow                  ---
  {Pu}/Liver0             Blood                     6.98
  {Pu}/Liver1             Blood                     6.98
  {Pu}/Liver2             Blood                     6.98
  {^Pu}/Liver1            Liver1                    ---
  {^Pu}/Liver2            Liver2                    ---
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Ra}/ST0                ST0                       ---
  {Ra}/ST1                ST1                       ---
  {Ra}/ST2                ST2                       ---
  {^Ra}/ST0               Blood                     6.98
  {^Ra}/ST1               Blood                     6.98
  {^Ra}/ST2               Blood                     6.98
  {*}/Other               Blood                     6.98
