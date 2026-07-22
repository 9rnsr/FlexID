# ICRP Publ.137 p.369 Para.811 / トリウム系列子孫元素としてのウラン
# ICRP Publ.137 p.369 Para.811
# > (811) ... The model for uranium
# > produced by serial decay of members of a uranium chain is a modification of the
# > model for uranium as a parent radionuclide (see Section 15). ...
# > (811) ... ウラン系列の核種の逐次崩壊によって
# > 生成されるウランのモデルは、親放射性核種としてのウランのモデルを修正したものである
# > （セクション15を参照）。 ...


[{U}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   Noch-T-bone-V         T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissues
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other

# ICRP Publ.137 p.369 Para.811 / トリウム系列子孫元素としてのウラン
# ICRP Publ.137 p.369 Para.811
# > (811) ... Single compartments
# > representing spleen, trabecular marrow, cortical marrow, testes, ovaries, and skin are
# > added for consistency with the models for other progeny of thorium. 
# > (811) ... トリウムの他の娘核種のモデルとの整合性をとるため、脾臓、海綿骨髄、
# > 皮質骨髄、精巣、卵巣、および皮膚を表す単一のコンパートメントが追加されています。

  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen

[{U}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.415 Table 15.4
  Plasma                  ST0                      10.5
  Plasma                  RBC                       0.245
  Plasma                  UB-con                   15.43
  Plasma                  Kidneys1                  2.94
  Plasma                  Kidneys2                  0.0122
  Plasma                  RC-con                    0.122
  Plasma                  Liver1                    0.367
 #Plasma                  ST1                       1.63        # fix
  Plasma                  ST2                       0.0735
  Plasma                  T-bone-S                  2.04
  Plasma                  C-bone-S                  1.63
  ST0                     Plasma                    8.32
  RBC                     Plasma                    0.347
  Kidneys1                UB-con                    0.099
  Kidneys2                Plasma                    0.00038
  Liver1                  Plasma                    0.092
  Liver1                  Liver2                    0.00693
  Liver2                  Plasma                    0.00019
  ST1                     Plasma                    0.0347
  ST2                     Plasma                    0.000019
  T-bone-S                Plasma                    0.0693
  T-bone-S                Exch-T-bone-V             0.0693
  C-bone-S                Plasma                    0.0693
  C-bone-S                Exch-C-bone-V             0.0693
  Noch-T-bone-V           Plasma                    0.000493
  Noch-C-bone-V           Plasma                    0.0000821
  Exch-T-bone-V           T-bone-S                  0.0173
  Exch-T-bone-V           Noch-T-bone-V             0.00578
  Exch-C-bone-V           C-bone-S                  0.0173
  Exch-C-bone-V           Noch-C-bone-V             0.00578

# ICRP Publ.137 p.369 Para.811
# (811) ... Single compartments
# representing spleen, trabecular marrow, cortical marrow, testes, ovaries, and skin are
# added for consistency with the models for other progeny of thorium. The six added
# compartments are taken from the intermediate-term soft tissue compartment ST1 in
# the model for uranium as a parent. Deposition of uranium as a progeny in spleen,
# trabecular marrow + cortical marrow (i.e. total combined marrow), testes, ovaries,
# or skin is calculated as its mass fraction of other soft tissues times the deposition
# fraction for ST1 (0.0665 of uranium ‘leaving the circulation’, as defined in Section
# 15). Deposition in trabecular marrow is assumed to be three times greater than
# deposition in cortical marrow. The derived transfer cofficients from blood to
# spleen, trabecular marrow, cortical marrow, testes, ovaries, and skin are 0.004 d-¹,
# 0.075 d-¹, 0.025 d-¹, 0.001 d-¹, 0.0003 d-¹, and 0.09 d-¹, respectively. The removal
# half-time from each added compartment is set at 20 d, the removal half-time from
# ST1. The deposition fraction for ST1 is reduced by the sum of deposition fractions
# for the six added compartments. 
# (811) ... トリウムの他の娘核種のモデルとの整合性をとるため、脾臓、海綿骨髄、
# 皮質骨髄、精巣、卵巣、および皮膚を表す単一のコンパートメントが追加されている。
# これら6つの追加コンパートメントは、親核種としてのウランのモデルにおける中期的軟部組織
# コンパートメントST1から採用されたものである。脾臓、海綿骨髄＋皮質骨髄（すなわち、合わせた全骨髄）、
# 精巣、卵巣、または皮膚における娘核種としてのウランの沈着量は、ST1の沈着割合（セクション15で
# 定義される「循環から離脱する」ウランの0.0665）に、他の軟部組織に対する当該組織の質量分率を
# 乗じることによって算出される。海綿骨髄における沈着量は、皮質骨髄における沈着量の3倍であると
# 仮定される。血液から脾臓、海綿骨髄、皮質骨髄、精巣、卵巣、および皮膚への移行係数は、それぞれ
# 0.004 d⁻¹、0.075 d⁻¹、0.025 d⁻¹、0.001 d⁻¹、0.0003 d⁻¹、および0.09 d⁻¹と導出される。
# 各追加コンパートメントからの除去半減期は、ST1からの除去半減期である20日と設定される。
# ST1の沈着割合は、これら6つの追加コンパートメントの沈着割合の合計分だけ低減される。

  Plasma                  ST1           $(1.63 - (0.025+0.075+0.001+0.0003+0.09+0.004))
  Plasma                  C-marrow                  0.025
  Plasma                  T-marrow                  0.075
  Plasma                  Testes                    0.001
  Plasma                  Ovaries                   0.0003
  Plasma                  Skin                      0.09
  Plasma                  Spleen                    0.004

  C-marrow                Plasma                $(0.693 / 20)   # 半減期20日
  T-marrow                Plasma                $(0.693 / 20)   # 半減期20日
  Testes                  Plasma                $(0.693 / 20)   # 半減期20日
  Ovaries                 Plasma                $(0.693 / 20)   # 半減期20日
  Skin                    Plasma                $(0.693 / 20)   # 半減期20日
  Spleen                  Plasma                $(0.693 / 20)   # 半減期20日

# ICRP Publ.137 p.369 Para.811
# (811) ... Uranium produced in a soft tissue compartment
# that is not identifiable with a compartment in the characteristic model for uranium is
# assumed to transfer to plasma at the rate 8.32 d-¹, the highest rate of loss from any
# compartment of other soft tissue in the characteristic model for uranium. Uranium
# produced in a bone volume compartment that is ambiguous with regard to the
# uranium model (e.g. trabecular or cortical bone volume in the thorium model) is
# assumed to be produced in non-exchangeable bone.
# (811) ... ウランの特性モデルにおけるコンパートメントと同一視できない
# 軟部組織コンパートメント内で生成されたウランは、8.32 d⁻¹の速度で血漿へ移行すると仮定される。
# この速度は、ウランの特性モデルにおける他の軟部組織のどのコンパートメントからの消失速度よりも
# 高いものである。ウランのモデルに関してその帰属が不明確な骨体積区画（例：トリウムモデルにおける
# 海綿骨または皮質骨の体積）で生成されるウランは、非交換性骨で生成されるものと仮定される。

  {U,Bi,Pb,Tl}/Plasma         Plasma                ---
  {Po}/Plasma1                Plasma             1000       # Publ.151 Annex.B
  {Po}/Plasma2                Plasma             1000       # Publ.151 Annex.B
  {Po}/Plasma3                Plasma             1000       # Publ.151 Annex.B
  {U,Po,Bi,Pb,Tl}/RBC         RBC                   ---
  {Pa,Th,Ac,Ra,Rn}/Blood      Plasma             1000       # Publ.151 Annex.B

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

  {U}/ST0                     ST0                   ---
  {U}/ST1                     ST1                   ---
  {U}/ST2                     ST2                   ---
  {^U}/ST0                    Plasma                8.32
  {^U}/ST1                    Plasma                8.32
  {^U}/ST2                    Plasma                8.32
  {*}/Other                   Plasma                8.32
