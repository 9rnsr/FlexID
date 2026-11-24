[{Hg}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
 #acc   Plasma0               Blood     # Gas,Vapor only
  acc   Plasma1               Blood
  acc   Plasma2               Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Brain1                Brain
  acc   Brain2                Brain
  acc   Other1                Other
  acc   Other2                Other
  exc   Excreta               ---


[{Hg}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.339 Table 36.5
  Plasma1                 RBC                       0.48
  Plasma1                 Plasma2                   2.4
  Plasma1                 Kidneys                   7.2
  Plasma1                 Liver                     4.8
  Plasma1                 Brain1                    0.048
  Plasma1                 T-bone-S                  0.024
  Plasma1                 C-bone-S                  0.024
  Plasma1                 Other1                    5.184
  Plasma1                 Other2                    0.72
  Plasma1                 SI-conRe                  1.92
  Plasma1                 Excreta                   1.2
  RBC                     Plasma1                   0.33
  Plasma2                 Plasma1                   0.6
  Kidneys                 UB-con                    0.0198
  Liver                   SI-conRe                  0.1733
  Brain1                  Plasma1                   0.0329
  Brain1                  Brain2                    0.00173
  Brain2                  Plasma1                   0.00038
  T-bone-S                Plasma1                   0.0347
  C-bone-S                Plasma1                   0.0347
  Other1                  Plasma1                   0.0347
  Other2                  Plasma1                   0.00693

# ICRP Publ.151 p.346 Para.709
# > (709) Progeny of thallium addressed in this publication are isotopes of thallium,
# > mercury, and gold. The model for thallium as a parent is applied to thallium produced
# > by decay of another isotope of thallium. The characteristic models for gold
# > and divalent mercury are applied to these elements as members of chains headed by
# > thallium with added transfer coefficients needed to solve the linked biokinetic models
# > of chains headed by thallium. The following transfer rates to the central blood
# > compartment are added to the characteristic model for mercury or gold: 1000 d⁻¹
# > if produced in a blood compartment not contained in the progeny’s model; and at the
# > following element-specific rates if produced in any other ambiguous compartment:
# > mercury, 0.0347 d⁻¹; and gold, 0.0693 d⁻¹.
# > (709) 本刊行物で扱うタリウムの子孫元素は、タリウム、水銀、および金の同位体です。
# > タリウムを親元素とするモデルは、タリウムの他の同位体の崩壊によって生成されるタリウムに
# > 適用されます。金および二価水銀の特性モデルは、タリウムを先頭とする連鎖のメンバーとして
# > これらの元素に適用され、タリウムを先頭とする連鎖の連結生体運動モデルを解くために
# > 必要な移動係数が加算されます。水銀または金の特性モデルには、中心血液コンパートメントへの
# > 以下の移動速度が追加されます：子孫元素のモデルに含まれない血液コンパートメントで生成される
# > 場合、1000 d⁻¹、その他のあいまいなコンパートメントで生成される場合、
# > 以下の元素固有の移動速度：水銀：0.0347 d⁻¹；金：0.0693 d⁻¹。

  {Tl}/Plasma             Plasma1                1000
  {Tl}/RBC                Plasma1                1000
  {Hg}/Plasma1            Plasma1                   ---
  {Hg}/Plasma2            Plasma2                   ---
  {Hg}/RBC                RBC                       ---

  {Tl,Hg}/C-bone-S        C-bone-S                  ---
  {Tl,Hg}/T-bone-S        T-bone-S                  ---

  {Tl,Hg}/Liver           Liver                     ---
  {Tl,Hg}/Kidneys         Kidneys                   ---
  {Hg}/Brain1             Brain1                    ---
  {Hg}/Brain2             Brain2                    ---

  {Tl}/Other              Plasma1                   0.0347
  {Hg}/Other1             Other1                    ---
  {Hg}/Other2             Other2                    ---

  {Hg}/Excreta            Excreta                   ---
