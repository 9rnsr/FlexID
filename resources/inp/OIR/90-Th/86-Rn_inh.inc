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

# ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としてのラドンは、ラジウム子孫元素としてのモデルを用いる。
# ↓
# ICRP Publ.137 p.331 Para.711 / ラジウム子孫元素としてのラドン
# > (711) A generic model structure is applied in the OIR series to radon, xenon, and
# > krypton produced in systemic compartments by decay of a parent radionuclide. ...
# > (711) OIRシリーズでは、親放射性核種の崩壊によって全身コンパートメントで生成されるラドン、
# > キセノン、およびクリプトンに、一般的なモデル構造が適用されています。 ...

# ICRP Publ.141 p.235 Para.563 / アクチニド子孫核種としてのラドン
# > (563) The model applied here to radon as an actinide progeny is a generic model
# > applied in the OIR series to radon produced by radioactive decay in a systemic
# > compartment. ...
# > (563) ここでアクチニド娘核種としてのラドンに適用したモデルは、OIRシリーズにおいて
# > 全身コンパートメント内で放射性崩壊によって生成されたラドンに適用された一般的な
# > モデルです。 ...

[{Rn}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  exc   Exhaled               ---

[{Rn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.130 Para.38
# > (38) Noble gases produced in compartments of the respiratory tract and in the
# > alimentary tract models by radioactive decay are assumed to escape from these
# > compartments directly to the environment at a rate of 100 d⁻¹, without transfer to
# > the blood compartment and without transfer between compartments of respiratory
# > tract and alimentary tract models. It is assumed that progeny of such noble gases
# > formed within the body follow the rules stated in Para. 37.
# > (38) 呼吸器系および消化器系モデルにおいて放射性崩壊により生成される希ガスは、
# > これらの部位から100 d⁻¹の速度で直接環境へ放出され、血液系への移行や呼吸器系と
# > 消化器系モデルの部位間の移動は起こらないものと仮定されます。体内で生成される
# > このような希ガスの子孫物質は、Para.37に述べた規則に従うものと仮定されます。

# ICRP Publ.130 Para.129
# > (129) For calculation purposes, the assumption that noble gases, including radon,
# > that are formed as progeny within the respiratory tract escape from the body at a rate
# > of 100 d⁻¹is applied in this series of reports.
# > (129) 計算のために、この一連の報告書では、呼吸器系内で子孫核種として生成される
# > ラドンを含む希ガスが100 d⁻¹の速度で体から排出されるという仮定を適用しています。

  ET1-F                   Exhaled                 100
  ET2-F                   Exhaled                 100
  ETseq-F                 Exhaled                 100
  LNET-F                  Exhaled                 100
  BB-F                    Exhaled                 100
  BBseq-F                 Exhaled                 100
  bb-F                    Exhaled                 100
  bbseq-F                 Exhaled                 100
  ALV-F                   Exhaled                 100
  INT-F                   Exhaled                 100
  LNTH-F                  Exhaled                 100
  ET1-S                   Exhaled                 100
  ET2-S                   Exhaled                 100
  ETseq-S                 Exhaled                 100
  LNET-S                  Exhaled                 100
  BB-S                    Exhaled                 100
  BBseq-S                 Exhaled                 100
  bb-S                    Exhaled                 100
  bbseq-S                 Exhaled                 100
  ALV-S                   Exhaled                 100
  INT-S                   Exhaled                 100
  LNTH-S                  Exhaled                 100
  ET2-B                   Exhaled                 100
  ETseq-B                 Exhaled                 100
  LNET-B                  Exhaled                 100
  BB-B                    Exhaled                 100
  BBseq-B                 Exhaled                 100
  bb-B                    Exhaled                 100
  bbseq-B                 Exhaled                 100
  ALV-B                   Exhaled                 100
  INT-B                   Exhaled                 100
  LNTH-B                  Exhaled                 100

  Oralcavity              Exhaled                 100
  Oesophagus-f            Exhaled                 100
  Oesophagus-s            Exhaled                 100
  St-con                  Exhaled                 100
  SI-con                  Exhaled                 100
  SI-conRe                Exhaled                 100
  RC-con                  Exhaled                 100
  LC-con                  Exhaled                 100
  RS-con                  Exhaled                 100

# ICRP Publ.130 p.85 Para.172
  UB-con                  Exhaled                  12

# ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としてのラドンは、ラジウム子孫元素としてのモデルを用いる。
# ↓
# ICRP Publ.137 p.331 Para.711 / ラジウム子孫元素としてのラドン
# > (711) ... These gases are assigned the model for transfer of radon from bone to blood introduced
# > in Publication 67 (ICRP, 1993), but are assigned element-specific rates of
# > transfer from soft tissues to blood. Specifically, radon, xenon, or krypton produced
# > in non-exchangeable bone volume, exchangeable bone volume, or bone surfaces
# > transfers to blood at the rate 0.36 d⁻¹, 1.5 d⁻¹, or 100 d⁻¹, respectively. Radon produced
# > in a soft tissue compartment transfers to blood with a half-time of 30 min,
# > compared with a half-time of 20 min for xenon and 15 min for krypton. Radon,
# > xenon, or krypton produced in blood or entering blood after its production in a
# > systemic compartment is removed from the body (exhaled) at the rate 1000 d⁻¹,
# > corresponding to a half-time of 1 min.
# > (711) OIRシリーズでは、親放射性核種の崩壊によって全身コンパートメントで生成されるラドン、
# > キセノン、およびクリプトンに、一般的なモデル構造が適用されています。これらのガスには、
# > Publication 67（ICRP, 1993）で導入された骨から血液へのラドンの移行モデルが適用されるが、
# > 軟部組織から血液への移行速度は元素ごとに異なります。具体的には、非交換性骨量、交換性骨量、
# > または骨表面で生成されたラドン、キセノン、またはクリプトンは、それぞれ0.36 d⁻¹、1.5 d⁻¹、
# > または100 d⁻¹の速度で血液に移行します。軟部組織コンパートメントで生成されたラドンは、
# > 半減期が30分であるのに対し、キセノンでは半減期が20分、クリプトンは15分です。血液中で
# > 生成された、または全身コンパートメントで生成された後に血液中に入るラドン、キセノン、
# > またはクリプトンは、1000 d⁻¹の割合で体外に排出されます（呼気）。これは半減期1分に相当します。

# ICRP Publ.141 p.235 Para.563 / アクチニド子孫核種としてのラドン
# > (563) ... Radon produced in a compartment identified as non-exchangeable
# > bone volume, exchangeable bone volume, or bone surface transfers to blood at the
# > rate 0.36, 1.5, or 100 d⁻¹, respectively; radon produced in a compartment identified
# > simply as ‘bone volume’ transfers to blood at 0.36 d⁻¹; radon produced in a soft
# > tissue compartment transfers to blood at 33.3 d⁻¹; and radon produced in blood or
# > entering blood is removed from the body (exhaled) at 1000 d⁻¹.
# > (563) ... 非交換性骨量、交換性骨量、または骨表面として識別されるコンパートメントで
# > 生成されたラドンは、それぞれ0.36、1.5、または100 d⁻¹の速度で血液に移行します。
# > 単に「骨量」として識別されるコンパートメントで生成されたラドンは、0.36 d⁻¹で
# > 血液に移行します。軟組織コンパートメントで生成されたラドンは、33.3 d⁻¹で血液に
# > 移行します。血液中で生成された、または血液に入ったラドンは、1000 d⁻¹で体から（呼気として）
# > 除去されます。

  {U,Bi,Pb,Tl}/Plasma         Blood              1000       # Publ.151 Annex.B
  {Po}/Plasma1                Blood              1000       # Publ.151 Annex.B
  {Po}/Plasma2                Blood              1000       # Publ.151 Annex.B
  {Po}/Plasma3                Blood              1000       # Publ.151 Annex.B
  {U,Po,Bi,Pb,Tl}/RBC         Blood              1000       # Publ.151 Annex.B
  {Pa,Th,Ac,Ra,Rn}/Blood      Blood                 ---

  {^Rn}/C-bone-S              Blood               100
  {^Rn}/T-bone-S              Blood               100
  {U,Ra,Pb}/Exch-C-bone-V     Blood                 1.5
  {U,Ra,Pb}/Exch-T-bone-V     Blood                 1.5
  {U,Ra,Pb}/Noch-C-bone-V     Blood                 0.36
  {U,Ra,Pb}/Noch-T-bone-V     Blood                 0.36
  {Pa,Th,Ac}/C-bone-V         Blood                 0.36
  {Pa,Th,Ac}/T-bone-V         Blood                 0.36

  {^Rn}/C-marrow              Blood         $(0.693 / (30/60/24))   # 半減期30分
  {^Rn}/T-marrow              Blood         $(0.693 / (30/60/24))   # 半減期30分
  {^Rn}/Liver1                Blood         $(0.693 / (30/60/24))   # 半減期30分
  {^Rn}/Liver2                Blood         $(0.693 / (30/60/24))   # 半減期30分
  {^Rn}/Kidneys1              Blood         $(0.693 / (30/60/24))   # 半減期30分
  {^Rn}/Kidneys2              Blood         $(0.693 / (30/60/24))   # 半減期30分
  {^Rn}/Testes                Blood         $(0.693 / (30/60/24))   # 半減期30分
  {^Rn}/Ovaries               Blood         $(0.693 / (30/60/24))   # 半減期30分
  {^Rn}/Skin                  Blood         $(0.693 / (30/60/24))   # 半減期30分
  {^Rn}/Spleen                Blood         $(0.693 / (30/60/24))   # 半減期30分

  {*}/ST0                     Blood         $(0.693 / (30/60/24))   # 半減期30分≒33.3 d⁻¹
  {*}/ST1                     Blood         $(0.693 / (30/60/24))   # 半減期30分≒33.3 d⁻¹
  {*}/ST2                     Blood         $(0.693 / (30/60/24))   # 半減期30分≒33.3 d⁻¹
  {*}/Other                   Blood         $(0.693 / (30/60/24))   # 半減期30分≒33.3 d⁻¹

  Blood                       Exhaled            1000       # ≒半減期1分
