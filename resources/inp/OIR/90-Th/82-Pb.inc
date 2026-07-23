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

# ICRP Publ.137 p.331 Para.712
# > (712) The models for thallium, lead, bismuth, and polonium produced in vivo following
# > intake of radium are modifications of their systemic models as progeny of lead,
# > described in Section 9. For each of these four elements, the red marrow compartment
# > is replaced by a compartment with the same kinetics (i.e. the same paths and rates of
# > inflow and outflow) named ‘trabecular marrow’ for consistency with the models for
# > other radium progeny; a compartment is added to represent cortical marrow, which is
# > assumed to exchange activity with plasma; the transfer coefficient from plasma to
# > cortical marrow is one-third of the transfer coefficient from plasma to trabecular
# > marrow; the removal half-time from cortical marrow is the same as that for trabecular
# > marrow; and the transfer coefficient from plasma to ST1 is reduced to maintain the
# > original total outflow rate from plasma to all destinations.
# > (712) ラジウム摂取後に体内で生成されるタリウム、鉛、ビスマス、およびポロニウムのモデルは、
# > 第9節で説明した鉛の子孫元素としての全身モデルを修正したものです。これら4元素それぞれに
# > ついて、赤色骨髄コンパートメントは、他のラジウム子孫元素のモデルとの整合性を図るため、
# > 同じ動態（すなわち、同じ経路および流入・流出速度）を持つ「骨梁骨髄」と呼ばれる
# > コンパートメントに置き換えられます。皮質骨髄を表すコンパートメントが追加され、この
# > コンパートメントは血漿と放射能を交換すると仮定されます。血漿から皮質骨髄への移行係数は、
# > 血漿から骨梁骨髄への移行係数の3分の1です。皮質骨髄からの除去半減期は、骨梁骨髄の場合と
# > 同じです。血漿からST1への移行係数は、血漿からすべての目的地への元の総流出速度を
# > 維持するために低減されます。

# ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としての鉛は、ラジウム子孫元素としてのモデルを用いる
# ↓
# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としての鉛には、鉛子孫元素としてのモデルを修正して用いる
# ↓
# ICRP Publ.137 p.230 Para.482 / 鉛子孫元素としての鉛
# > (482) The systemic model for lead as a progeny of lead is an extension of the
# > characteristic model for lead described above. ...
# > (482) 鉛の子孫核種としての鉛の全身モデルは、上述の(訳注：親核種としての)
# > 鉛の特性モデルの拡張です。 ...

[{Pb}:compartment]
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
  exc   Excreta(sweat)              ---
  exc   Excreta(hair,skin,nails)    ---

# ICRP Publ.137 p.230 Para.482 / 鉛子孫元素としての鉛
# > (482) ... Compartments representing red
# > marrow, spleen, skin, testes, and ovaries are added to the model structure for consistency
# > with the models for other lead progeny. ...
# > (482) ... 他の鉛子孫核種のモデルとの整合性を図るため、赤色骨髄、脾臓、皮膚、精巣、および卵巣を
# > 表すコンパートメントがモデル構造に追加されます。...

 #acc   RedMarrow             R-marrow      # Publ.137 Para.712
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen

# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としての鉛
# > (712) ... For each of these four elements, the red marrow compartment
# > is replaced by a compartment with the same kinetics (i.e. the same paths and rates of
# > inflow and outflow) named ‘trabecular marrow’ for consistency with the models for
# > other radium progeny; a compartment is added to represent cortical marrow, which is
# > assumed to exchange activity with plasma; ...
# > (712) ... これら4元素それぞれに
# > ついて、赤色骨髄コンパートメントは、他のラジウム子孫元素のモデルとの整合性を図るため、
# > 同じ動態（すなわち、同じ経路および流入・流出速度）を持つ「骨梁骨髄」と呼ばれる
# > コンパートメントに置き換えられます。皮質骨髄を表すコンパートメントが追加され、この
# > コンパートメントは血漿と放射能を交換すると仮定されます。...

  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow

[{Pb}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.225 Table 9.3
  Plasma                  UB-con                    1.75
  Plasma                  RC-con                    0.7
  Plasma                  T-bone-S                  4.86
  Plasma                  C-bone-S                  3.89
 #Plasma                  ST0                      22.16            # fix(1)
 #Plasma                  ST1                       0.7             # fix(1)
 #Plasma                  ST2                       0.14            # fix(1)
  Plasma                  Liver1                    4.9
  Plasma                  Kidneys1                  2.45
  Plasma                  Kidneys2                  0.0245
  Plasma                  RBC                      28
  Plasma                  Excreta(sweat)            0.42
  RBC                     Plasma                    0.139
  T-bone-S                Plasma                    0.5
  T-bone-S                Exch-T-bone-V             0.5
  C-bone-S                Plasma                    0.5
  C-bone-S                Exch-C-bone-V             0.5
  Exch-T-bone-V           T-bone-S                  0.0185
  Exch-T-bone-V           Noch-T-bone-V             0.0046
  Exch-C-bone-V           C-bone-S                  0.0185
  Exch-C-bone-V           Noch-C-bone-V             0.0046
  Noch-T-bone-V           Plasma                    0.000493
  Noch-C-bone-V           Plasma                    0.0000821
  Liver1                  Plasma                    0.0312
  Liver1                  SI-conRe                  0.0312
  Liver1                  Liver2                    0.00693
  Liver2                  Plasma                    0.0019
  Kidneys1                UB-con                    0.139
  Kidneys2                Plasma                    0.0019
  ST0                     Plasma                    7.39
  ST1                     Plasma                    0.00416
  ST2                     Plasma                    0.00038
  ST1                     Excreta(hair,skin,nails)  0.00277

# ICRP Publ.137 p.230 Para.482 / 鉛子孫元素としての鉛
# > (482) ... Each of the added compartments is
# > assumed to exchange activity with plasma alone. Transfer coefficients between
# > plasma and these compartments are set for reasonable consistency with the
# > equilibrium concentration of lead in these compartments relative to the equilibrium
# > concentration in liver as indicated by results of autopsy studies on human subjects
# > (Blanchard and Moore, 1970, 1971; Gross et al., 1975; Skerving et al., 1983). The
# > removal half-time from each of the added compartments is set at 1 y, which is the
# > long-term removal half-time from liver (t½ from Liver 2 to plasma). It is assumed
# > that 0.6% of outflow from plasma goes to red marrow, 0.03% goes to spleen, 0.2%
# > goes to skin, 0.005% goes to testes, and 0.002% goes to ovaries. The transfer coefficients
# > from plasma to each of the soft tissue compartments Other 0, Other 1, and
# > Other 2 are reduced by approximately 2.5% to maintain a total outflow rate from
# > plasma of approximately 70 d⁻¹. ...
# > (482) ... 追加された各コンパートメントは、
# > 血漿のみと放射能を交換すると仮定されます。血漿とこれらのコンパートメント間の
# > 移動係数は、ヒトを対象とした剖検研究の結果に示されている肝臓中の平衡濃度に対する
# > これらのコンパートメント中の鉛の平衡濃度と妥当な整合性を持つように設定されています
# > （Blanchard and Moore, 1970, 1971; Gross et al., 1975; Skerving et al., 1983）。
# > 追加された各コンパートメントからの除去半減期は1年と設定されており、これは肝臓からの
# > 長期除去半減期（肝臓2から血漿へのt½）です。血漿からの流出量の0.6%が赤色骨髄へ、
# > 0.03%が脾臓へ、0.2%が皮膚へ、0.005%が精巣へ、0.002%が卵巣へ向かうと仮定されます。
# > 血漿からその他0、その他1、その他2の各軟部組織コンパートメントへの移行係数は、
# > 血漿からの総流出速度を約70 d⁻¹に維持するために約2.5%減少します。 ...

# 作成者注：
#   Plasmaからの元々の流出量の総和は
#      1.75 + 0.7 + 4.86 + 3.89 + 22.16 + 0.7 + 0.14 + 4.9 + 2.45 + 0.0245 + 28 + 0.42 = 69.9945
#
#   Plasmaから追加コンパートメントへの流出量＝ST0,1,2への流出量から差し引くべき量は
#      69.9945 * (0.6% + 0.03% + 0.2% + 0.005% + 0.002%) = 0.585854
#
#   PlasmaからST0,1,2への流出量の減少率は
#      (22.16 + 0.7 + 0.14 - 0.585854) / (22.16 + 0.7 + 0.14) ≒ 0.974528
#   ※ 1 - 0.974528 ≒ 約2.54%減少 ⇔「約2.5%減少」

 #Plasma                  RedMarrow         $(69.9945 * 0.6%)       # fix(2)
  Plasma                  Skin              $(69.9945 * 0.2%)
  Plasma                  Spleen            $(69.9945 * 0.03%)
  Plasma                  Testes            $(69.9945 * 0.005%)
  Plasma                  Ovaries           $(69.9945 * 0.002%)
  Plasma                  ST0               $(22.16 * 0.974528)
 #Plasma                  ST1               $( 0.7  * 0.974528)     # fix(2)
  Plasma                  ST2               $( 0.14 * 0.974528)
 #RedMarrow               Plasma                    0.0019          # fix(2)
  Skin                    Plasma                    0.0019
  Spleen                  Plasma                    0.0019
  Testes                  Plasma                    0.0019
  Ovaries                 Plasma                    0.0019

# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としての鉛
# > (712) ... the transfer coefficient from plasma to
# > cortical marrow is one-third of the transfer coefficient from plasma to trabecular
# > marrow; the removal half-time from cortical marrow is the same as that for trabecular
# > marrow; and the transfer coefficient from plasma to ST1 is reduced to maintain the
# > original total outflow rate from plasma to all destinations.
# > (712) ... 血漿から皮質骨髄への移行係数は、血漿から骨梁骨髄への移行係数の3分の1です。
# > 皮質骨髄からの除去半減期は、骨梁骨髄の場合と同じです。血漿からST1への移行係数は、
# > 血漿からすべての目的地への元の総流出速度を維持するために低減されます。

  Plasma                  T-marrow          $(69.9945 * 0.6%)                           # RedMarrowをT-marrowに置換
  Plasma                  C-marrow          $(69.9945 * 0.6% / 3)                       # Plasma→T-marrowの移行速度の1/3
  Plasma                  ST1               $( 0.7  * 0.974528 - 69.9945 * 0.6% / 3)    # Plasma→C-marrowの速度を差し引く
  T-marrow                Plasma                    0.0019                              # RedMarrowをT-marrowに置換
  C-marrow                Plasma                    0.0019                              # T-marrow→Plasmaの移行速度と同じ

# ICRP Publ.137 p.230 Para.482 / 鉛子孫元素としての鉛
# > (482) ... Lead produced in a soft tissue compartment of a
# > preceding chain member not identifiable with a compartment of the lead model is
# > assumed to transfer to plasma at the rate 7.39 d⁻¹; the highest rate of transfer to
# > plasma in the lead model. Lead produced in a blood compartment of a preceding
# > chain member that is not identifiable with a blood compartment of the lead model is
# > assigned the transfer rate 1000 d⁻¹ to plasma.
# > (482) ... 先行する連鎖メンバーの軟部組織コンパートメントで生成された鉛は、
# > 鉛モデルのコンパートメントと識別できない場合、血漿へ7.39 d⁻¹の速度で移行すると仮定されます。
# > これは鉛モデルにおける血漿への移行速度の最高値です。先行する連鎖メンバーの血液コンパートメントで
# > 生成された鉛は、鉛モデルの血液コンパートメントと識別できない場合、血漿へ1000 d⁻¹の速度で
# > 移行すると仮定されます。

  {U,Bi,Pb,Tl}/Plasma         Plasma                ---
  {Po}/Plasma1                Plasma             1000
  {Po}/Plasma2                Plasma             1000
  {Po}/Plasma3                Plasma             1000
  {U,Po,Bi,Pb,Tl}/RBC         RBC                   ---
  {Pa,Th,Ac,Ra,Rn}/Blood      Plasma             1000

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

  {Pb}/ST0                    ST0                   ---
  {Pb}/ST1                    ST1                   ---
  {Pb}/ST2                    ST2                   ---
  {^Pb}/ST0                   Plasma                7.39
  {^Pb}/ST1                   Plasma                7.39
  {^Pb}/ST2                   Plasma                7.39
  {*}/Other                   Plasma                7.39
