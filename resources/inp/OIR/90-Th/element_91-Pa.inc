# ICRP Publ.141 p.235 Para.566 / アクチニド子孫元素としてのプロトアクチニウム
# > (566) The models applied here to actinium, protactinium, neptunium, plutonium,
# > americium, curium, berkelium, californium, einsteinium, and fermium as actinide
# > progeny are modifications of their characteristic models described earlier in this
# > section. ...
# > (566) アクチニドの娘核種であるアクチニウム、プロトアクチニウム、ネプツニウム、
# > プルトニウム、アメリシウム、キュリウム、バークリウム、カリホルニウム、
# > アインスタイニウム、およびフェルミウムに対してここで適用されるモデルは、
# > 本節の冒頭で述べた各元素の特性モデルを修正したものです。 ...

# ICRP Publ.137 p.370 Para.812 / トリウム子孫元素としてのプロトアクチニウム
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

# ICRP Publ.137 p.333 Para.715 / ラジウム子孫元素としてのトリウム
# > (715) The model for thorium as a progeny of radium is a modification of the systemic
# > model applied in this publication to thorium as a parent radionuclide. ...
# > (715) ラジウムの子孫核種としてのトリウムのモデルは、本論文でトリウムを親核種として
# > 適用した全身モデルを修正したものです。 ...

[{Pa}:compartment]
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

# ICRP Publ.141 p.235 Para.566 / アクチニド子孫元素としてのプロトアクチニウム
# > (566) ... For a given element in this group, two compartments representing the
# > skin and spleen are added to the explicitly identified source regions in the element's
# > characteristic model. ...
# > (566) ... このグループの特定の元素について、その特性モデルにおいて明示的に特定された
# > 線源領域に加え、皮膚と脾臓を表す2つのコンパートメントが追加されます。 ...

# ICRP Publ.137 p.333 Para.715 / ラジウム子孫元素としてのトリウム
# > (715) ... Two
# > compartments, one representing spleen and the other representing skin, are added to
# > that model.
# > (715) ... このモデルには、脾臓と皮膚を表す2つのコンパートメントが追加されています。

  acc   Skin                  Skin
  acc   Spleen                Spleen

$endif

[{Pa}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.141 p.234 Table 18.9
  Blood                   Liver1                    0.097
  Blood                   T-bone-S                  0.679
  Blood                   C-bone-S                  0.679
  Blood                   Kidneys1                  0.0679
  Blood                   Kidneys2                  0.0194
  Blood                   UB-con                    0.107
  Blood                   RC-con                    0.0097
  Blood                   Testes                    0.00068
  Blood                   Ovaries                   0.00021
  Blood                   ST0                       0.832
$if parent
  Blood                   ST1                       0.243
$endif
  Blood                   ST2                       0.0388
  Liver1                  SI-con                    0.000475
  Liver1                  Liver2                    0.00095
  Liver1                  Blood                     0.000475
  Liver2                  Blood                     0.000211
  T-bone-S                T-marrow                  4.93E-4
  T-bone-S                T-bone-V                  2.47E-4
  T-bone-V                T-marrow                  4.93E-4
  T-marrow                Blood                     0.0076
  C-bone-S                C-marrow                  8.21E-5
  C-bone-S                C-bone-V                  4.11E-5
  C-bone-V                C-marrow                  8.21E-5
  C-marrow                Blood                     0.0076
  C-marrow                C-bone-S                  0
  Kidneys1                UB-con                    0.0462
  Kidneys2                Blood                     0.00038
  Testes                  Blood                     0.00038
  Ovaries                 Blood                     0.00038
  ST0                     Blood                     0.462
  ST1                     Blood                     0.00095
  ST2                     Blood                     1.9E-5

$if progeny

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
# > (715) ... 脾臓と皮膚はトリウムを血液と交換すると仮定されます。
# > これらのコンパートメントのパラメータ値は、動物実験およびヒト剖検研究から得られた
# > 限られたデータに基づいている（Stover et al., 1960; Thomas et al., 1963; Larsen et al.,
# > 1984; Singh et al., 1983; Stehney and Lucas, 2000; Glover et al., 2001）。
# > 脾臓と皮膚は、第 14 節で定義される「循環系から排出される」トリウムのそれぞれ 0.5% と 2% を
# > 受け取ると仮定されます。血液から中間期の軟部組織コンパートメント ST1 への移行係数（図 14.2）は、
# > 血液からの元の除去半減期を維持するために低減されます。脾臓および皮膚から血液への
# > 除去半減期の仮定は 2 年であり、これは ST1 から血液への除去半減期と同じです。

  Blood                   Skin                      0.0554      # = 2.0% * 0.693/0.25
  Blood                   Spleen                    0.0139      # = 0.5% * 0.693/0.25
  Blood                   ST1                   $(0.243 - (0.0139 + 0.0554))
 
  Skin                    Blood                 $(0.693 / 2/365)
  Spleen                  Blood                 $(0.693 / 2/365)

# ICRP Publ.137 p.333 Para.715 / ラジウム子孫元素としてのトリウム
# > (715) ... Thorium produced in a compartment
# > of a preceding chain member that is not identifiable with a compartment in
# > the thorium model is assumed to transfer to the blood compartment of the thorium
# > model at the following rate: 1000 d⁻¹ if produced in a blood compartment; 0.462 d⁻¹ if
# > produced in a soft tissue compartment (the highest rate of removal from a compartment
# > of other soft tissues in the thorium model); and at the rate of bone turnover if
# > produced in an exchangeable bone volume compartment.
# > (715) ... トリウムモデル内のコンパートメントと識別できない先行する連鎖メンバーのコンパートメントで
# > 生成されたトリウムは、トリウムモデルの血液コンパートメントに以下の速度で移行すると仮定されます。
# > 血液コンパートメントで生成された場合は 1000 d⁻¹、軟部組織コンパートメントで生成された
# > 場合は 0.462 d⁻¹です（トリウムモデルにおける他の軟部組織のコンパートメントからの除去速度が
# > 最も高い）。交換可能な骨量コンパートメントで生成された場合は骨代謝速度です。

  {*}/Blood               Blood                     ---
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
  {*}/Liver1              Liver1                    ---
  {*}/Liver2              Liver2                    ---
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Pa}/ST0                ST0                       ---
  {Pa}/ST1                ST1                       ---
  {Pa}/ST2                ST2                       ---
  {^Pa}/ST0               Blood                     0.462
  {^Pa}/ST1               Blood                     0.462
  {^Pa}/ST2               Blood                     0.462
  {*}/Other               Blood                     0.462

$endif
