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

# ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としてのビスマスは、ラジウム子孫元素としてのモデルを用いる
# ↓
# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としてのビスマスには、鉛子孫元素としてのモデルを修正して用いる
# ↓
# ICRP Publ.137 p.231 Para.483 / 鉛子孫元素としてのビスマス
# > (483) The systemic model for bismuth as a progeny of lead is based on the characteristic
# > model for bismuth applied in this publication. ...
# > (483) 鉛の子孫元素としてのビスマスの全身モデルは、本論文で適用されているビスマスの特性モデルに
# > 基づいています。 ...

[{Bi}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissues
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other

# ICRP Publ.137 p.231 Para.483 / 鉛子孫元素としてのビスマス
# > (483) ... The structure of the characteristic
# > model for bismuth is modified by the addition of compartments
# > representing red marrow, spleen, skin, testes, and ovaries, which are explicitly identified
# > in models for some other elements appearing in lead chains. Each of these
# > compartments is assumed to exchange bismuth with plasma. ...
# > (483) ... ビスマスの特性モデルの構造は、鉛連鎖に現れる他のいくつかの元素のモデルで
# > 明示的に特定されている赤色骨髄、脾臓、皮膚、精巣、および卵巣を表すコンパートメントを追加する
# > ことによって修正されています。これらのコンパートメントはそれぞれ、血漿とビスマスを交換すると
# > 仮定されます。 ...

 #acc   RedMarrow             R-marrow      # Publ.137 Para.712
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen

# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としてのビスマス
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

[{Bi}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.258 Table 10.3
  Plasma                  UB-con                   20
  Plasma                  RC-con                    4.0
  Plasma                  RBC                       0.5
  Plasma                  ST0                     300
 #Plasma                  ST1                       4.2         # fix(1)
  Plasma                  ST2                       1.3
  Plasma                  Liver1                   30
  Plasma                  Kidneys1                 30
  Plasma                  Kidneys2                  5.0
  Plasma                  C-bone-S                  2.5
  Plasma                  T-bone-S                  2.5
  RBC                     Plasma                    0.173
  ST0                     Plasma                   66.54
  ST1                     Plasma                    0.0347
  ST2                     Plasma                    0.00116
 #Liver1                  Plasma        # Fig. 10.1 にはあるが、Table 10.3には数値の記載がない。
  Liver1                  SI-conRe                  0.208
  Liver1                  Liver2                    0.139
  Liver2                  Plasma                    0.0693
  Kidneys1                UB-con                    0.693
  Kidneys2                Plasma                    0.139
  C-bone-S                Plasma                    0.0347
  T-bone-S                Plasma                    0.0347

# ICRP Publ.137 p.231 Para.483 / 鉛子孫元素としてのビスマス
# > (483) ... Transfer coefficients for
# > the added compartments are selected for reasonable consistency with the biokinetic
# > database underlying the characteristic model for bismuth, and with the retention
# > curve for total soft tissues based on that original model. The specific changes to
# > the characteristic model for bismuth are as follows: (1) the transfer coefficients
# > from plasma to the added compartments are 0.3 d⁻¹ for red marrow, 0.3 d⁻¹ for
# > skin, 0.02 d⁻¹ for spleen, 0.003 d⁻¹ for testes, and 0.001 d⁻¹ for ovaries; (2) the transfer
# > coefficient from plasma to the other soft tissue compartment with a moderate
# > turnover rate (0.0347 d⁻¹, equivalent to a half-time of 20 d) is reduced from 4.2 d⁻¹ to
# > 3.576 d⁻¹; and (3) the assigned transfer coefficient from each of the added compartments
# > to plasma is 0.0347 d⁻¹. ...
# > (483) ... 追加されたコンパートメントの移動係数は、ビスマスの特性モデルの基礎となる
# > 生体動態データベース、および元のモデルに基づく全軟組織の保持曲線と妥当な整合性を持つように
# > 選択されています。ビスマスの特性モデルへの具体的な変更点は以下のとおりです。
# > (1) 血漿から追加されたコンパートメントへの移行係数は、赤色骨髄では0.3 d⁻¹、皮膚では0.3 d⁻¹、
# > 脾臓では0.02 d⁻¹、精巣では0.003 d⁻¹、卵巣では0.001 d⁻¹です。
# > (2) 血漿から中程度の代謝率（0.0347 d⁻¹、半減期20日に相当）を持つ他の軟部組織コンパートメントへの
# > 移行係数は4.2 d⁻¹から3.576 d⁻¹に減少します。
# > (3) 追加された各コンパートメントから血漿への割り当て移行係数は0.0347 d⁻¹です。 ...

 #Plasma                  RedMarrow                 0.3         # fix(2)
  Plasma                  Skin                      0.3
  Plasma                  Spleen                    0.02
  Plasma                  Testes                    0.003
  Plasma                  Ovaries                   0.001
 #Plasma                  ST1                       3.576       # fix(2)
 #RedMarrow               Plasma                    0.0347      # fix(2)
  Skin                    Plasma                    0.0347
  Spleen                  Plasma                    0.0347
  Testes                  Plasma                    0.0347
  Ovaries                 Plasma                    0.0347

# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としてのビスマス
# > (712) ... the transfer coefficient from plasma to
# > cortical marrow is one-third of the transfer coefficient from plasma to trabecular
# > marrow; the removal half-time from cortical marrow is the same as that for trabecular
# > marrow; and the transfer coefficient from plasma to ST1 is reduced to maintain the
# > original total outflow rate from plasma to all destinations.
# > (712) ... 血漿から皮質骨髄への移行係数は、血漿から骨梁骨髄への移行係数の3分の1です。
# > 皮質骨髄からの除去半減期は、骨梁骨髄の場合と同じです。血漿からST1への移行係数は、
# > 血漿からすべての目的地への元の総流出速度を維持するために低減されます。

  Plasma                  T-marrow                  0.3         # RedMarrowをT-marrowに置換
  Plasma                  C-marrow                $(0.3 / 3)    # Plasma→T-marrowの移行速度の1/3
  Plasma                  ST1             $(3.576 - 0.3 / 3)    # Plasma→C-marrowの速度を差し引く
  T-marrow                Plasma                    0.0347      # RedMarrowをT-marrowに置換
  C-marrow                Plasma                    0.0347      # T-marrow→Plasmaの移行速度と同じ

# ICRP Publ.137 p.231 Para.483 / 鉛子孫元素としてのビスマス
# > (483) ... Bismuth produced in a blood compartment that is not
# > identifiable with a blood compartment of the bismuth model is assumed to transfer
# > to plasma at the rate 1000 d⁻¹. Bismuth produced in a trabecular or cortical bone
# > volume compartment is assumed to transfer to plasma at the reference turnover rate
# > for that bone type.
# > (483) ... ビスマスモデルの血液コンパートメントと識別できない血液コンパートメントで
# > 生成されたビスマスは、1000 d⁻¹の速度で血漿に移行すると仮定されます。海綿骨または
# > 皮質骨の体積コンパートメントで生成されたビスマスは、その骨の種類に応じた基準代謝速度で
# > 血漿に移行すると仮定されます。

  {U,Bi,Pb,Tl}/Plasma         Plasma                ---
  {Po}/Plasma1                Plasma             1000
  {Po}/Plasma2                Plasma             1000
  {Po}/Plasma3                Plasma             1000
  {U,Po,Bi,Pb,Tl}/RBC         RBC                   ---
  {Pa,Th,Ac,Ra,Rn}/Blood      Plasma             1000

  {^Rn}/C-bone-S              C-bone-S              ---
  {^Rn}/T-bone-S              T-bone-S              ---
  {U,Ra,Pb}/Exch-C-bone-V     Plasma            $(0.03 / 365)
  {U,Ra,Pb}/Exch-T-bone-V     Plasma            $(0.18 / 365)
  {U,Ra,Pb}/Noch-C-bone-V     Plasma            $(0.03 / 365)
  {U,Ra,Pb}/Noch-T-bone-V     Plasma            $(0.18 / 365)
  {Pa,Th,Ac}/C-bone-V         Plasma            $(0.03 / 365)
  {Pa,Th,Ac}/T-bone-V         Plasma            $(0.18 / 365)

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

# 作成者注：
#   Publ.137 Para.483には軟組織で生成されたビスマスの血液への移行速度についての記載がないため、
#   ビスマスモデルにて軟組織から血漿へ移行する速度のうち最も大きなもの(ST0→Plasma, 66.54 d⁻¹)
#   を採用する。

  {Bi}/ST0                    ST0                   ---
  {Bi}/ST1                    ST1                   ---
  {Bi}/ST2                    ST2                   ---
  {^Bi}/ST0                   Plasma               66.54
  {^Bi}/ST1                   Plasma               66.54
  {^Bi}/ST2                   Plasma               66.54
  {*}/Other                   Plasma               66.54
