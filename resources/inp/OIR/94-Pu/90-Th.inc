[{Th}:compartment]
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
  acc   Liver1                Liver         # hepatocytes
  acc   Liver2                Liver         # reticulo-endothelial cells
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissues
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other

#    ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としてのトリウムは、ラジウム子孫元素としてのモデルを用いる。
# → ICRP Publ.137 p.333 Para.715 / ラジウム子孫元素としてのトリウム
# > (715) The model for thorium as a progeny of radium is a modification of the systemic
# > model applied in this publication to thorium as a parent radionuclide. Two
# > compartments, one representing spleen and the other representing skin, are added to
# > that model.
# > (715) ラジウムの子孫核種としてのトリウムのモデルは、本論文でトリウムを親核種として
# > 適用した全身モデルを修正したものである。このモデルには、脾臓と皮膚を表す2つの
# > コンパートメントが追加されている。

  acc   Skin                  Skin
  acc   Spleen                Spleen

[{Th}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.368 Table 14.3
  Blood                   Liver1                    0.097
  Blood                   C-bone-S                  0.6793
  Blood                   T-bone-S                  0.6793
  Blood                   UB-con                    0.1067
  Blood                   Kidneys1                  0.0679
  Blood                   Kidneys2                  0.0194
  Blood                   RC-con                    0.0097
  Blood                   Testes                    0.00068
  Blood                   Ovaries                   0.00021
  Blood                   ST0                       0.832
 #Blood                   ST1                       0.243
  Blood                   ST2                       0.0388
  Liver1                  Blood                     0.000475
  Liver1                  Liver2                    0.00095
  Liver1                  SI-conRe                  0.000475
  C-bone-S                C-marrow                  0.0000821
  C-bone-S                C-bone-V                  0.0000411
  T-bone-S                T-marrow                  0.000493
  T-bone-S                T-bone-V                  0.000247
  Kidneys1                UB-con                    0.0462
  Kidneys2                Blood                     0.00038
  Testes                  Blood                     0.00038
  Ovaries                 Blood                     0.00038
  ST0                     Blood                     0.462
  ST1                     Blood                     0.00095
  ST2                     Blood                     0.000019
  Liver2                  Blood                     0.000211
  C-marrow                Blood                     0.0076
  C-bone-V                C-marrow                  0.0000821
  T-marrow                Blood                     0.0076
  T-bone-V                T-marrow                  0.000493

# ICRP Publ.137 p.333 Para.715 / ラジウム子孫元素としてのトリウム
# > (715) ... Spleen and skin are assumed to exchange thorium with blood. Parameter
# > values for these compartments are based on limited data from animal studies and
# > human autopsy studies (Stover et al., 1960; Thomas et al., 1963; Larsen et al., 1984;
# > Singh et al., 1983; Stehney and Lucas, 2000; Glover et al., 2001). Spleen and skin are
# > assumed to receive 0.5% and 2%, respectively, of thorium ‘leaving the circulation’ as
# > defined in Section 14. The transfer coefficient from blood to the intermediate-term soft
# > tissue compartment ST1 (Fig. 14.2) is reduced to maintain the original removal halftime
# > from blood. The assumed removal half-time from spleen and skin to blood is 2 y,
# > the same as the removal half-time from ST1 to blood.
# > (715) ... 脾臓と皮膚はトリウムを血液と交換すると仮定されている。
# > これらのコンパートメントのパラメータ値は、動物実験およびヒト剖検研究から得られた
# > 限られたデータに基づいている（Stover et al., 1960; Thomas et al., 1963; Larsen et al.,
# > 1984; Singh et al., 1983; Stehney and Lucas, 2000; Glover et al., 2001）。
# > 脾臓と皮膚は、第 14 節で定義される「循環系から排出される」トリウムのそれぞれ 0.5% と 2% を
# > 受け取ると仮定する。血液から中間期の軟部組織コンパートメント ST1 への移行係数（図 14.2）は、
# > 血液からの元の除去半減期を維持するために低減される。脾臓および皮膚から血液への
# > 除去半減期の仮定は 2 年であり、これは ST1 から血液への除去半減期と同じである。

  Blood                   ST1                   $(0.243 - (0.0139 + 0.0554))
  Blood                   Spleen                    0.0139      # = 0.5% * 0.693/0.25
  Blood                   Skin                      0.0554      # = 2.0% * 0.693/0.25
  Spleen                  Blood                 $(0.693 / 2/365)
  Skin                    Blood                 $(0.693 / 2/365)

# ICRP Publ.137 p.333 Para.715 / ラジウム子孫元素としてのトリウム
# > (715) ... Thorium produced in a compartment
# > of a preceding chain member that is not identifiable with a compartment in
# > the thorium model is assumed to transfer to the blood compartment of the thorium
# > model at the following rate: 1000 d⁻¹ if produced in a blood compartment; 0.462 d⁻¹ if
# > produced in a soft tissue compartment (the highest rate of removal from a compartment
# > of other soft tissues in the thorium model); and at the rate of bone turnover if
# > produced in an exchangeable bone volume compartment.
# > (715) ... トリウムモデル内のコンパートメントと識別できない先行する連鎖メンバーのコンパートメントで
# > 生成されたトリウムは、トリウムモデルの血液コンパートメントに以下の速度で移行すると仮定する。
# > 血液コンパートメントで生成された場合は 1000 d⁻¹、軟部組織コンパートメントで生成された
# > 場合は 0.462 d⁻¹（トリウムモデルにおける他の軟部組織のコンパートメントからの除去速度が
# > 最も高い）。交換可能な骨量コンパートメントで生成された場合は骨代謝速度です。

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
  {Pu}/Liver0             Blood                     0.462
  {Pu}/Liver1             Blood                     0.462
  {Pu}/Liver2             Blood                     0.462
  {^Pu}/Liver1            Liver1                    ---
  {^Pu}/Liver2            Liver2                    ---
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Th}/ST0                ST0                       ---
  {Th}/ST1                ST1                       ---
  {Th}/ST2                ST2                       ---
  {^Th}/ST0               Blood                     0.462
  {^Th}/ST1               Blood                     0.462
  {^Th}/ST2               Blood                     0.462
  {*}/Other               Blood                     0.462
