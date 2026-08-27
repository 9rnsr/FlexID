# ICRP Publ.141 p.235 Para.563 / アクチニド子孫元素としてのラドン
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

# ICRP Publ.141 p.235 Para.563 / アクチニド子孫元素としてのラドン
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

  {*}/Blood               Blood                     ---
  {Pu}/Blood0             Blood                  1000       # Publ.151 Annex.B
  {Pu}/Blood1             Blood                  1000       # Publ.151 Annex.B
  {Pu}/Blood2             Blood                  1000       # Publ.151 Annex.B
  {*}/Plasma              Blood                  1000       # Publ.151 Annex.B
  {Po}/Plasma1            Blood                  1000       # Publ.151 Annex.B
  {Po}/Plasma2            Blood                  1000       # Publ.151 Annex.B
  {Po}/Plasma3            Blood                  1000       # Publ.151 Annex.B
  {*}/RBC                 Blood                  1000       # Publ.151 Annex.B

  {*}/C-bone-S            Blood                   100
  {*}/T-bone-S            Blood                   100
  {*}/Exch-C-bone-V       Blood                     1.5
  {*}/Exch-T-bone-V       Blood                     1.5
  {*}/Noch-C-bone-V       Blood                     0.36
  {*}/Noch-T-bone-V       Blood                     0.36
  {*}/C-bone-V            Blood                     0.36
  {*}/T-bone-V            Blood                     0.36

  {*}/C-marrow            Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/T-marrow            Blood             $(0.693 / (30/60/24))   # 半減期30分
  {Pu}/Liver0             Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/Liver1              Blood             $(0.693 / (30/60/24))   # 半減期30分
  {*}/Liver2              Blood             $(0.693 / (30/60/24))   # 半減期30分
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
