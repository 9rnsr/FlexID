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

$if progeny

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
# > およびイリジウムの同位体です。オスミウムとイリジウムの特性モデルは、オスミウムの
# > 子孫元素としてのこれらの元素にもそのまま適用されます。オスミウムの子孫元素としての
# > レニウムとタングステンのモデルは、本刊行物で使用されているこれらの元素の特性モデルの
# > 拡張版であり、オスミウムを先頭とする連鎖の連結生体動態モデルを解くために必要な
# > コンパートメントと関連する移動係数が追加されています（付録B参照）。
# > あいまいなコンパートメント（すなわち、レニウムまたはタングステンのモデルで明示的に
# > 名前が付けられていないコンパートメント）で生成された場合、子孫元素はその特性
# > 生体動態モデルの中央血液コンパートメントに指定された速度で移行し、その後は
# > そのモデルに従うと仮定されます。不明瞭なコンパートメントで生成されたレニウムまたは
# > タングステンの中心血液コンパートメントへの移行速度は、血液コンパートメントで
# > 生成された場合は1000 d⁻¹、骨量コンパートメントで生成された場合は骨代謝の速度、
# > その他の不明瞭なコンパートメントで生成された場合は、以下の元素固有の移行速度と
# > なります：レニウム：0.462 d⁻¹、タングステン：8.32 d⁻¹。

  {Os}/Blood1             Blood                     ---
  {Os}/Blood2             Blood                  1000

  {Os}/C-bone-S           C-bone-S                  ---
  {Os}/T-bone-S           T-bone-S                  ---
  {Os}/C-bone-V           C-bone-V                  ---
  {Os}/T-bone-V           T-bone-V                  ---

  {Os}/Liver1             Liver1                    ---
  {Os}/Liver2             Liver2                    ---
  {Os}/Kidneys1           Kidneys1                  ---
  {Os}/Kidneys2           Kidneys2                  ---

  {Os}/ST0                Blood                     0.462
  {Os}/ST1                Blood                     0.462
  {Os}/ST2                Blood                     0.462

$endif
