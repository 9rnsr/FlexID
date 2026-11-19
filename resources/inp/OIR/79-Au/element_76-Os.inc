[{Os}:compartment]
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
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Os}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.301 Table 33.3
  Blood1                  SI-conRe                  3.0
  Blood1                  UB-con                   23
  Blood1                  Liver1                   12
  Blood1                  Kidneys1                 10.67
  Blood1                  Kidneys2                  0.33
  Blood1                  Blood2                   27
  Blood1                  ST0                      15
  Blood1                  ST1                       2.5
  Blood1                  ST2                       2.5
  Blood1                  C-bone-S                  1.0
  Blood1                  T-bone-S                  3.0
  Blood2                  Blood1                    0.6931
  Liver1                  Blood1                    0.09704
  Liver1                  SI-conRe                  0.03466
  Liver1                  Liver2                    0.006931
  Liver2                  Blood1                    0.003798
  Kidneys1                UB-con                    0.1386
  Kidneys2                Blood1                    0.003798
  ST0                     Blood1                    0.09902
  ST1                     Blood1                    0.0231
  ST2                     Blood1                    0.0009495
  C-bone-S                Blood1                    0.07922
  T-bone-S                Blood1                    0.07922
  C-bone-S                C-bone-V                  0.0198
  T-bone-S                T-bone-V                  0.0198
  C-bone-V                Blood1                    0.0000821
  T-bone-V                Blood1                    0.000493

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
# > 白金の放射性同位元素です。親元素としての金のモデルは、金の子孫元素としての金に
# > 適用されます。金の子孫元素としてのレニウム、オスミウム、イリジウム、および白金のモデルは、
# > これらの元素の特性モデルの拡張版であり、金を先頭とする連鎖の連結生体動態モデルを
# > 解くために必要なコンパートメントと関連する移動係数が追加されています（付録B参照）。
# > あいまいなコンパートメント（すなわち、子孫元素のモデルで明示的に名前が付けられていない
# > コンパートメント）で生成された場合、子孫元素は、その特性生体動態モデルの
# > 中心血液コンパートメントに特定の速度で移行し、その後はそのモデルに従うと仮定されます。
# > 不明瞭なコンパートメントで生成されたレニウム、オスミウム、イリジウム、および白金の
# > 中心血液コンパートメントへの移行速度は、血液コンパートメントで生成された場合は1000 d⁻¹、
# > 組織コンパートメントで生成された場合は以下の元素固有の移行速度となります：
# > レニウム：0.462 d⁻¹、オスミウムまたは白金：0.09902 d⁻¹、イリジウム：0.0693 d⁻¹。

  {*}/Blood1              Blood1                    ---
  {*}/Blood2              Blood2                    ---

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {*}/C-bone-V            C-bone-V                  ---
  {*}/T-bone-V            T-bone-V                  ---

  {Au}/Liver              Blood1                    0.09902
  {*}/Liver1              Liver1                    ---
  {*}/Liver2              Liver2                    ---
  {Au}/Kidneys            Blood1                    0.09902
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---

  {Os}/ST0                ST0                       ---
  {Os}/ST1                ST1                       ---
  {Os}/ST2                ST2                       ---
  {^Os}/ST0               ST0                       0.09902
  {^Os}/ST1               ST1                       0.09902
  {^Os}/ST2               ST2                       0.09902
  {Au}/Other1             Blood1                    0.09902
  {Au}/Other2             Blood1                    0.09902
