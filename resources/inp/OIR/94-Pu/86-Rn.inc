#   ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としてのラドンは、ラジウム子孫元素としてのモデルを用いる。
# →ICRP Publ.137 p.331 Para.711 / ラジウム子孫元素としてのラドン
# なお、子孫核種としてのラドンの挙動については、ICRP Publ.141 p.235 Para.563にも
# 同等の記載が存在するため、ここではPubl.137 Para.711 と併せて引用する。

# ICRP Publ.137 p.331 Para.711 / ラジウム子孫元素としてのラドン
# > (711) A generic model structure is applied in the OIR series to radon, xenon, and
# > krypton produced in systemic compartments by decay of a parent radionuclide. ...
# > (711) OIRシリーズでは、親放射性核種の崩壊によって全身コンパートメントで生成されるラドン、
# > キセノン、およびクリプトンに、一般的なモデル構造が適用されている。 ...

# ICRP Publ.141 p.235 Para.563 / 子孫核種としてのラドンの取り扱い
# > (563) The model applied here to radon as an actinide progeny is a generic model
# > applied in the OIR series to radon produced by radioactive decay in a systemic
# > compartment. ...
# > (563) ここでアクチニド娘核種としてのラドンに適用したモデルは、OIRシリーズにおいて
# > 全身コンパートメント内で放射性崩壊によって生成されたラドンに適用された一般的な
# > モデルである。 ...

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
# > 消化器系モデルの部位間の移動は起こらないものと仮定される。体内で生成される
# > このような希ガスの子孫物質は、Para.37に述べた規則に従うものと仮定される。

# ICRP Publ.130 Para.129
# > (129) For calculation purposes, the assumption that noble gases, including radon,
# > that are formed as progeny within the respiratory tract escape from the body at a rate
# > of 100 d⁻¹is applied in this series of reports.
# > (129) 計算のために、この一連の報告書では、呼吸器系内で子孫核種として生成される
# > ラドンを含む希ガスが100 d⁻¹の速度で体から排出されるという仮定を適用している。

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
# > キセノン、およびクリプトンに、一般的なモデル構造が適用されている。これらのガスには、
# > Publication 67（ICRP, 1993）で導入された骨から血液へのラドンの移行モデルが適用されるが、
# > 軟部組織から血液への移行速度は元素ごとに異なる。具体的には、非交換性骨量、交換性骨量、
# > または骨表面で生成されたラドン、キセノン、またはクリプトンは、それぞれ0.36 d⁻¹、1.5 d⁻¹、
# > または100 d⁻¹の速度で血液に移行する。軟部組織コンパートメントで生成されたラドンは、
# > 半減期が30分であるのに対し、キセノンでは半減期が20分、クリプトンは15分である。血液中で
# > 生成された、または全身コンパートメントで生成された後に血液中に入るラドン、キセノン、
# > またはクリプトンは、1000 d⁻¹の割合で体外に排出されます（呼気）。これは半減期1分に相当します。

# ICRP Publ.141 p.235 Para.563 / 子孫核種としてのラドンの取り扱い
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

  {*}/Plasma              Blood                  1000       # Publ.151 Annex.B
  {Po}/Plasma1            Blood                  1000       # Publ.151 Annex.B
  {Po}/Plasma2            Blood                  1000       # Publ.151 Annex.B
  {Po}/Plasma3            Blood                  1000       # Publ.151 Annex.B
  {*}/RBC                 Blood                  1000       # Publ.151 Annex.B
  {Pu}/Blood0             Blood                  1000       # Publ.151 Annex.B
  {Pu}/Blood1             Blood                  1000       # Publ.151 Annex.B
  {Pu}/Blood2             Blood                  1000       # Publ.151 Annex.B
  {*}/Blood               Blood                     ---

  {*}/C-bone-S            Blood                   100
  {*}/T-bone-S            Blood                   100
  {*}/C-bone-V            Blood                     0.36
  {*}/T-bone-V            Blood                     0.36
  {*}/Noch-C-bone-V       Blood                     0.36
  {*}/Noch-T-bone-V       Blood                     0.36
  {*}/Exch-C-bone-V       Blood                     1.5
  {*}/Exch-T-bone-V       Blood                     1.5

  {*}/C-marrow            Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/T-marrow            Blood             $(0.693 / (30/60/24))   # 半減期30分
  {Pu}/Liver0             Blood             $(0.693 / (30/60/24))   # 半減期30分
  {Pu}/Liver1             Blood             $(0.693 / (30/60/24))   # 半減期30分
  {Pu}/Liver2             Blood             $(0.693 / (30/60/24))   # 半減期30分
  {^Pu}/Liver1            Blood             $(0.693 / (30/60/24))   # 半減期30分
  {^Pu}/Liver2            Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/Kidneys1            Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/Kidneys2            Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/Testes              Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/Ovaries             Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/Skin                Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/Spleen              Blood             $(0.693 / (30/60/24))   # 半減期30分

  {*}/ST0                 Blood             $(0.693 / (30/60/24))   # 半減期30分≒33.3 d⁻¹
  {*}/ST1                 Blood             $(0.693 / (30/60/24))   # 半減期30分≒33.3 d⁻¹
  {*}/ST2                 Blood             $(0.693 / (30/60/24))   # 半減期30分≒33.3 d⁻¹
  {*}/Other               Blood             $(0.693 / (30/60/24))   # 半減期30分≒33.3 d⁻¹

  Blood                   Exhaled                1000       # ≒半減期1分
