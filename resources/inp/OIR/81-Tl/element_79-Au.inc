[{Au}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other1                Other
  acc   Other2                Other


[{Au}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.321 Table 35.3
  Blood1                  Blood2                    0.1
  Blood1                  Kidneys                   0.1
  Blood1                  Liver                     0.1
  Blood1                  Other1                    0.18
  Blood1                  Other2                    0.1
  Blood1                  UB-con                    0.3
  Blood1                  RC-con                    0.1
  Blood1                  T-bone-S                  0.01
  Blood1                  C-bone-S                  0.01
  Blood2                  Blood1                    0.139
  Kidneys                 UB-con                    0.0693
  Liver                   Blood1                    0.0693
  Other1                  Blood1                    0.0693
  Other2                  Blood1                    0.0139
  T-bone-S                Blood1                    0.0693
  C-bone-S                Blood1                    0.0693

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

  {Tl}/Plasma             Blood1                 1000
  {Tl}/RBC                Blood1                 1000
  {Hg}/Plasma1            Blood1                 1000
  {Hg}/Plasma2            Blood1                 1000
  {Hg}/RBC                Blood1                 1000

  {Tl,Hg}/C-bone-S        C-bone-S                  ---
  {Tl,Hg}/T-bone-S        T-bone-S                  ---

  {Tl,Hg}/Liver           Liver                     ---
  {Tl,Hg}/Kidneys         Kidneys                   ---
  {Hg}/Brain1             Blood1                    0.0693
  {Hg}/Brain2             Blood1                    0.0693

  {Tl}/Other              Blood1                    0.0693
  {Hg}/Other1             Blood1                    0.0693
  {Hg}/Other2             Blood1                    0.0693
