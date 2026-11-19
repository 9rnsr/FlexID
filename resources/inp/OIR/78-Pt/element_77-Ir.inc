[{Ir}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissue
  acc   ST0                   Other         # rapid turnover
  acc   ST1                   Other         # intermediate turnover
  acc   ST2                   Other         # slow turnover


[{Ir}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.193 Table 8.4
  Blood1                  SI-conRe                  4.0
  Blood1                  UB-con                   12
  Blood1                  Liver1                   12
  Blood1                  Kidneys1                  4.0
  Blood1                  Kidneys2                  2.0
  Blood1                  Blood2                   27
  Blood1                  ST0                      15
  Blood1                  ST1                      15
  Blood1                  ST2                       1.0
  Blood1                  C-bone-S                  2.0
  Blood1                  T-bone-S                  6.0
  Blood2                  Blood1                    0.693
  Liver1                  Blood1                    0.0231
  Liver1                  SI-conRe                  0.0462
  Liver1                  Liver2                    0.0693
  Liver2                  Blood1                    0.00693
  Kidneys1                UB-con                    0.139
  Kidneys2                Blood1                    0.00693
  ST0                     Blood1                    0.0693
  ST1                     Blood1                    0.00693
  ST2                     Blood1                    0.00095
  C-bone-S                Blood1                    0.0185
  T-bone-S                Blood1                    0.0185
  C-bone-S                C-bone-V                  0.00462
  T-bone-S                T-bone-V                  0.00462
  C-bone-V                Blood1                    0.0000821
  T-bone-V                Blood1                    0.000493

# ICRP Publ.151 p.307 Para.601
# > (601) Progeny of platinum addressed in this publication are isotopes of platinum,
# > rhenium, osmium, iridium, and gold. The characteristic models for platinum,
# > osmium, and iridium used in this publication are applied to these elements as progeny
# > of platinum. The models for rhenium and gold as progeny of platinum are
# > expansions of the characteristic models for these elements with added compartments
# > and associated transfer coefficients needed to solve the linked biokinetic models for
# > chains headed by platinum (see Annex B). The following transfer rates to the central
# > blood compartment are assigned to rhenium or gold produced in an ambiguous
# > compartment (i.e. a compartment of a preceding chain member that is not a compartment
# > in the characteristic model for the progeny: 1000 d⁻¹ if produced in a blood
# > compartment; at the rate of bone turnover if produced in a bone volume compartment;
# > and at the following element-specific rates if produced in any other ambiguous
# > compartment: rhenium, 0.462 d⁻¹; and gold, 0.0693 d⁻¹).
# > (601) 本刊行物で扱う白金の子孫元素は、白金、レニウム、オスミウム、イリジウム、および
# > 金の同位体である。本刊行物で用いられる白金、オスミウム、およびイリジウムの特性モデルは、
# > 白金の子孫元素としてのこれらの元素に適用される。白金の子孫元素としてのレニウムと
# > 金のモデルは、これらの元素の特性モデルを拡張したもので、白金を先頭とする連鎖の
# > 連結生体反応速度論モデルを解くために必要なコンパートメントと関連する移動係数が追加された
# > ものである（付録B参照）。不明瞭なコンパートメント（すなわち、子孫元素の特性モデル
# > におけるコンパートメントではない、先行する連鎖メンバーのコンパートメント）で生成された
# > レニウムまたは金には、中心血液コンパートメントへの移行速度として以下が割り当てられる。
# > 血液コンパートメントで生成された場合は1000 d⁻¹、骨量コンパートメントで生成された場合は
# > 骨代謝の速度、その他の不明瞭なコンパートメントで生成された場合は、以下の元素固有の
# > 移行速度が割り当てられる。レニウム：0.462 d⁻¹、金：0.0693 d⁻¹。

  {Pt,Ir}/Blood1          Blood1                    ---
  {Pt,Ir}/Blood2          Blood2                    ---

  {Pt,Ir}/C-bone-S        C-bone-S                  ---
  {Pt,Ir}/T-bone-S        T-bone-S                  ---
  {Pt,Ir}/C-bone-V        C-bone-V                  ---
  {Pt,Ir}/T-bone-V        T-bone-V                  ---

  {Pt,Ir}/Liver1          Liver1                    ---
  {Pt,Ir}/Liver2          Liver2                    ---
  {Pt,Ir}/Kidneys1        Kidneys1                  ---
  {Pt,Ir}/Kidneys2        Kidneys2                  ---

  {Pt,Ir}/ST0             ST0                       ---
  {Pt,Ir}/ST1             ST1                       ---
  {Pt,Ir}/ST2             ST2                       ---
