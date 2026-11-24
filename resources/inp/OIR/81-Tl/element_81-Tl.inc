[{Tl}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other                 Other


[{Tl}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.346 Table 37.3
  Plasma                  Liver                    10
  Plasma                  Kidneys                  10
  Plasma                  RBC                       5
  Plasma                  T-bone-S                 15
  Plasma                  C-bone-S                 15
  Plasma                  Other                   140
  Plasma                  UB-con                    1.5
  Plasma                  RC-con                    3.5
  RBC                     Plasma                    3.7
  Liver                   Plasma                    2.5
  Kidneys                 Plasma                    2.5
  T-bone-S                Plasma                    2.5
  C-bone-S                Plasma                    2.5
  Other                   Plasma                    2.5

$if progeny

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

  {Tl}/Plasma             Plasma                    ---
  {Tl}/RBC                RBC                       ---

  {Tl}/C-bone-S           C-bone-S                  ---
  {Tl}/T-bone-S           T-bone-S                  ---

  {Tl}/Liver              Liver                     ---
  {Tl}/Kidneys            Kidneys                   ---

  {Tl}/Other              Other                     ---

$endif
