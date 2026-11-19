[{Re}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   S-glands              S-glands
  acc   OralcavityRe          O-cavity
  acc   Oesophagus-sRe        Oesophag-s
  acc   St-conRe              St-cont
  acc   St-wall               St-wall
  acc   RC-wall               RC-wall
  acc   Thyroid1              Thyroid
  acc   Thyroid2              Thyroid
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other tissue
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Re}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.130 p.76 Table 3.4
# ICRP Publ.130 p.77 Para.143
# ICRP Publ.151 p.289 Fig.32.1
  OralcavityRe            Oesophagus-sRe          720
  Oesophagus-sRe          St-conRe               2160
  St-conRe                SI-conRe                 20.57

# ICRP Publ.151 p.290 Table 32.3
  Blood                   Thyroid1                  7.0
  Blood                   ST0                      71.88
  Blood                   ST1                       3.0
  Blood                   ST2                       0.18
  Blood                   UB-con                    1.7
  Blood                   S-glands                  2.6
  Blood                   St-wall                   4.3
  Blood                   Kidneys1                  0.7
  Blood                   Kidneys2                  0.04
  Blood                   Liver1                    4.5
  Blood                   RC-wall                   3.4
  Blood                   T-bone-S                  0.35
  Blood                   C-bone-S                  0.35
  Thyroid1                Blood                   100
  Thyroid1                Thyroid2                  1.0
  Thyroid2                Blood                     1.0
  ST0                     Blood                    50
  ST1                     Blood                     0.462
  ST2                     Blood                     0.0347
  S-glands                OralcavityRe             50
  St-wall                 St-conRe                 50
  Kidneys1                UB-con                    8.32
  Kidneys2                Blood                     0.0347
  Liver1                  Blood                     8.234
  Liver1                  Liver2                    0.0832
  Liver2                  Blood                     0.0347
  RC-wall                 RC-con                    1.39
  T-bone-S                Blood                     0.457
  T-bone-S                T-bone-V                  0.00462
  C-bone-S                Blood                     0.457
  C-bone-S                C-bone-V                  0.00462
  T-bone-V                Blood                     0.000493
  C-bone-V                Blood                     0.0000821

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

  {Pt,Os}/Blood1          Blood                  1000
  {Pt,Os}/Blood2          Blood                  1000

  {Pt,Os}/C-bone-S        C-bone-S                  ---
  {Pt,Os}/T-bone-S        T-bone-S                  ---
  {Pt,Os}/C-bone-V        C-bone-V                  ---
  {Pt,Os}/T-bone-V        T-bone-V                  ---

  {Pt,Os}/Liver1          Liver1                    ---
  {Pt,Os}/Liver2          Liver2                    ---
  {Pt,Os}/Kidneys1        Kidneys1                  ---
  {Pt,Os}/Kidneys2        Kidneys2                  ---

  {Pt,Os}/ST0             Blood                     0.462
  {Pt,Os}/ST1             Blood                     0.462
  {Pt,Os}/ST2             Blood                     0.462
