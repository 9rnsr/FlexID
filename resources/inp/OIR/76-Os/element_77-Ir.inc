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

# ICRP Publ.151 p.299 Para.588
# > (588) Progeny of osmium addressed in this publication are isotopes of osmium,
# > rhenium, tungsten, and iridium. The characteristic models for osmium and iridium
# > are applied without change to these elements as progeny of osmium. The models for
# > rhenium and tungsten as progeny of osmium are expansions of the characteristic
# > models for these elements used in this publication, with added compartments and
# > associated transfer coefficients needed to solve the linked biokinetic models for
# > chains headed by osmium (see Annex B). If produced in an ambiguous compartment
# > (i.e. a compartment not explicitly named in the model for rhenium or tungsten), the
# > progeny is assumed to transfer at a specified rate to the central blood compartment
# > of its characteristic biokinetic model, and to follow that model thereafter. The following
# > transfer rates to the central blood compartment are assigned to rhenium or
# > tungsten produced in an ambiguous compartment: 1000 d⁻¹ if produced in a blood
# > compartment; at the rate of bone turnover if produced in a bone volume
# > compartment; and at the following element-specific rates if produced in any other
# > ambiguous compartment: rhenium, 0.462 d⁻¹; and tungsten, 8.32 d⁻¹.
# > (588) 本刊行物で扱うオスミウムの子孫元素は、オスミウム、レニウム、タングステン、
# > およびイリジウムの同位体である。オスミウムとイリジウムの特性モデルは、オスミウムの
# > 子孫元素としてのこれらの元素にもそのまま適用される。オスミウムの子孫元素としての
# > レニウムとタングステンのモデルは、本刊行物で使用されているこれらの元素の特性モデルの
# > 拡張版であり、オスミウムを先頭とする連鎖の連結生体動態モデルを解くために必要な
# > コンパートメントと関連する移動係数が追加されている（付録B参照）。
# > あいまいなコンパートメント（すなわち、レニウムまたはタングステンのモデルで明示的に
# > 名前が付けられていないコンパートメント）で生成された場合、子孫元素は、
# > その特性生体動態モデルの中央血液コンパートメントに指定された速度で移行し、その後は
# > そのモデルに従うと仮定される。不明瞭なコンパートメントで生成されたレニウムまたは
# > タングステンの中心血液コンパートメントへの移行速度は、血液コンパートメントで
# > 生成された場合は1000 d⁻¹、骨量コンパートメントで生成された場合は骨代謝の速度、
# > その他の不明瞭なコンパートメントで生成された場合は、以下の元素固有の移行速度と
# > なります：レニウム：0.462 d⁻¹、タングステン：8.32 d⁻¹。

  {Os}/Blood1             Blood1                    ---
  {Os}/Blood2             Blood2                    ---

  {Os}/C-bone-S           C-bone-S                  ---
  {Os}/T-bone-S           T-bone-S                  ---
  {Os}/C-bone-V           C-bone-V                  ---
  {Os}/T-bone-V           T-bone-V                  ---

  {Os}/Liver1             Liver1                    ---
  {Os}/Liver2             Liver2                    ---
  {Os}/Kidneys1           Kidneys1                  ---
  {Os}/Kidneys2           Kidneys2                  ---

  {Os}/ST0                ST0                       ---
  {Os}/ST1                ST1                       ---
  {Os}/ST2                ST2                       ---
