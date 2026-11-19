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

# ICRP Publ.151 p.320 Para.651
# > (651) Progeny of gold addressed in this publication are radioisotopes of gold,
# > rhenium, osmium, iridium, and platinum. The model for gold as a parent is applied
# > to gold as a progeny of gold. The models for rhenium, osmium, iridium, and platinum
# > as progeny of gold are expansions of the characteristic models for these elements
# > with added compartments and associated transfer coefficients needed to solve
# > the linked biokinetic models for chains headed by gold (see Annex B). If produced in
# > an ambiguous compartment (i.e. a compartment not explicitly named in the progeny’s
# > model), the progeny is assumed to transfer at a specified rate to the central
# > blood compartment of its characteristic biokinetic model, and to follow that model
# > thereafter. The following transfer rates to the central blood compartment are
# > assigned to rhenium, osmium, iridium, and platinum produced in an ambiguous
# > compartment: 1000 d⁻¹ if produced in a blood compartment; and at the following
# > element-specific rates if produced in a tissue compartment: rhenium, 0.462 d⁻¹;
# > osmium or platinum, 0.09902 d⁻¹; and iridium, 0.0693 d⁻¹.
# > (651) 本刊行物で扱う金の子孫元素は、金、レニウム、オスミウム、イリジウム、および
# > 白金の放射性同位元素である。親元素としての金のモデルは、金の子孫元素としての金に
# > 適用される。金の子孫元素としてのレニウム、オスミウム、イリジウム、および白金のモデルは、
# > これらの元素の特性モデルの拡張版であり、金を先頭とする連鎖の連結生体動態モデルを
# > 解くために必要なコンパートメントと関連する移動係数が追加されている（付録B参照）。
# > あいまいなコンパートメント（すなわち、子孫元素のモデルで明示的に名前が付けられていない
# > コンパートメント）で生成された場合、子孫元素は、その特性生体動態モデルの
# > 中心血液コンパートメントに特定の速度で移行し、その後はそのモデルに従うと仮定される。
# > 不明瞭なコンパートメントで生成されたレニウム、オスミウム、イリジウム、および白金の
# > 中心血液コンパートメントへの移行速度は、血液コンパートメントで生成された場合は1000 d⁻¹、
# > 組織コンパートメントで生成された場合は以下の元素固有の移行速度となる：
# > レニウム：0.462 d⁻¹、オスミウムまたは白金：0.09902 d⁻¹、イリジウム：0.0693 d⁻¹。

  {*}/Blood1              Blood                  1000
  {*}/Blood2              Blood                  1000

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {*}/C-bone-V            C-bone-V                  ---
  {*}/T-bone-V            T-bone-V                  ---

  {Au}/Liver              Blood                     0.462
  {*}/Liver1              Liver1                    ---
  {*}/Liver2              Liver2                    ---
  {Au}/Kidneys            Blood                     0.462
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---

  {Re}/ST0                ST0                       ---
  {Re}/ST1                ST1                       ---
  {Re}/ST2                ST2                       ---
  {^Re}/ST0               ST0                       0.462
  {^Re}/ST1               ST1                       0.462
  {^Re}/ST2               ST2                       0.462
  {Au}/Other1             Blood                     0.462
  {Au}/Other2             Blood                     0.462
