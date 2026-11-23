[{Ta}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver0                Liver
  acc   Liver1                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other


[{Ta}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.276 Table 30.3
  Blood1                  Blood2                    3.2
  Blood1                  Liver0                    0.24
  Blood1                  Kidneys                   0.04
  Blood1                  ST0                       3.2
  Blood1                  ST1                       0.12
  Blood1                  UB-con                    0.88
  Blood1                  SI-conRe                  0.08
  Blood1                  T-bone-S                  0.12
  Blood1                  C-bone-S                  0.12
  Blood2                  Blood1                    1.39
  Liver0                  SI-conRe                  0.0578
  Liver0                  Blood1                    0.0578
  Liver0                  Liver1                    0.231
  Liver1                  Blood1                    0.005
  Kidneys                 Blood1                    0.005
  ST0                     Blood1                    1.39
  ST1                     Blood1                    0.01
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821

$if progeny

# ICRP Publ.151 p.277 Para.556
# > (556) Progeny of tantalum addressed in this publication are isotopes of tantalum,
# > hafnium, lutetium, and tungsten. The characteristic models for tantalum and hafnium
# > are applied to these elements as progeny of tantalum. The models for lutetium
# > and tungsten as progeny of tantalum are the characteristic models for these elements
# > with added transfer coefficients needed to solve the linked biokinetic models for
# > chains headed by tantalum. Lutetium or tungsten produced in an ambiguous compartment
# > (i.e. a compartment of the model for a preceding chain that is not a compartment
# > in the model for the progeny) is assumed to transfer to the central blood
# > compartment of the progeny’s model, and to follow that model thereafter. The following
# > transfer rates are assigned to lutetium or tungsten produced in ambiguous
# > compartments: 1000 d⁻¹ if produced in a blood compartment; at the rate of bone
# > turnover for the indicated bone type if produced in a bone volume compartment; and
# > at the following element-specific rates if produced in any other compartment: lutetium,
# > 1.39 d⁻¹; and tungsten, 8.32 d⁻¹.
# > (556) 本書で扱うタンタルの娘核種には、タンタル、ハフニウム、ルテチウム、および
# > タングステンの同位体が含まれます。タンタルおよびハフニウムについては、それら自身の
# > 特性モデルが、タンタルの娘核種としてのモデルとして適用されます。ルテチウムおよび
# > タングステンをタンタルの娘核種とするモデルは、それぞれの特性モデルに、タンタルを
# > 起点とする崩壊系列の連鎖的な生物学的動態モデルを解くために必要な移行係数を追加したもの
# > となります。不明確なコンパートメント（すなわち、先行する崩壊系列のモデルには存在するが、
# > 娘核種のモデルには存在しないコンパートメント）で生成されたルテチウムまたはタングステンは、
# > 娘核種のモデルにおける中心血液コンパートメントへ移行し、その後はそのモデルに従うものと
# > 仮定されます。不明確なコンパートメントで生成されたルテチウムまたはタングステンには、
# > 以下の移行速度が割り当てられます。血液コンパートメントで生成された場合は1000 d⁻¹、
# > 骨体積コンパートメントで生成された場合は当該骨種における骨代謝回転速度、その他の
# > コンパートメントで生成された場合は、元素ごとに以下の移行速度（ルテチウム：1.39 d⁻¹、
# > タングステン：8.32 d⁻¹）が適用されます。

  {Ta}/Blood1             Blood1                    ---
  {Ta}/Blood2             Blood2                    ---

  {Ta}/C-bone-S           C-bone-S                  ---
  {Ta}/T-bone-S           T-bone-S                  ---
  {Ta}/C-bone-V           C-bone-V                  ---
  {Ta}/T-bone-V           T-bone-V                  ---

  {Ta}/Liver0             Liver0                    ---
  {Ta}/Liver1             Liver1                    ---
  {Ta}/Kidneys            Kidneys                   ---

  {Ta}/ST0                ST0                       ---
  {Ta}/ST1                ST1                       ---

$endif
