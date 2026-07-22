# ICRP Publ.137 p.370 Para.812
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

# ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としてのラジウムは、ラジウム子孫元素としてのモデルを用いる
# ↓
# ICRP Publ.137 p.334 Para.716 / ラジウム子孫元素としてのラジウム
# > (716) ... Single compartments representing spleen, trabecular marrow,
# > cortical marrow, testes, ovaries, and skin are added for consistency
# > with the models for other progeny of radium.  ...
# > (716) ... ラジウムの他の子孫核種のモデルとの整合性を保つため、脾臓、
# > 骨梁骨髄、皮質骨髄、精巣、卵巣、皮膚を表す個々のコンパートメントが
# > 追加されます。 ...

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
# > 0.0021 d⁻¹、0.00068 d⁻¹、0.2 d⁻¹です。追加された各コンパートメントからの
# > 除去率は0.02 d⁻¹に設定されており、これはST1+ST2からの有効除去半減時間に
# > 基づいて丸められた値です。 ...

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
# > 移行すると仮定されます。血液コンパートメントで生成された場合は1000 d⁻¹、
# > 軟部組織コンパートメントで生成された場合は6.98 d⁻¹です。6.98 d⁻¹という値は、
# > ラジウムの特性モデルにおける他の軟部組織コンパートメントからの除去速度の最大値です。

  {U,Bi,Pb,Tl}/Plasma         Blood              1000
  {Po}/Plasma1                Blood              1000
  {Po}/Plasma2                Blood              1000
  {Po}/Plasma3                Blood              1000
  {U,Po,Bi,Pb,Tl}/RBC         Blood              1000
  {Pa,Th,Ac,Ra,Rn}/Blood      Blood                 ---

  {^Rn}/C-bone-S              C-bone-S              ---
  {^Rn}/T-bone-S              T-bone-S              ---
  {U,Ra,Pb}/Exch-C-bone-V     Exch-C-bone-V         ---
  {U,Ra,Pb}/Exch-T-bone-V     Exch-T-bone-V         ---
  {U,Ra,Pb}/Noch-C-bone-V     Noch-C-bone-V         ---
  {U,Ra,Pb}/Noch-T-bone-V     Noch-T-bone-V         ---
  {Pa,Th,Ac}/C-bone-V         Noch-C-bone-V         ---     # Publ.137 Para.812
  {Pa,Th,Ac}/T-bone-V         Noch-T-bone-V         ---     # Publ.137 Para.812

  {^Rn}/C-marrow              C-marrow              ---
  {^Rn}/T-marrow              T-marrow              ---
  {^Rn}/Liver1                Liver1                ---
  {^Rn}/Liver2                Liver2                ---
  {^Rn}/Kidneys1              Kidneys1              ---
  {^Rn}/Kidneys2              Kidneys2              ---
  {^Rn}/Testes                Testes                ---
  {^Rn}/Ovaries               Ovaries               ---
  {^Rn}/Skin                  Skin                  ---
  {^Rn}/Spleen                Spleen                ---

  {Ra}/ST0                    ST0                   ---
  {Ra}/ST1                    ST1                   ---
  {Ra}/ST2                    ST2                   ---
  {^Ra}/ST0                   Blood                 6.98
  {^Ra}/ST1                   Blood                 6.98
  {^Ra}/ST2                   Blood                 6.98
  {*}/Other                   Blood                 6.98
