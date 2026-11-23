[{W}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-T-bone-V         T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Spleen                Spleen
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Other0                Other         # Rapid turnover
  acc   Other1                Other         # Intermediate turnover
  acc   Other2                Other         # Slow turnover
  exc   Excreta               ---


[{W}:transfer]
#-----------------------+---------------------+--------------
# From                  | To                  | Coefficient[/d] or [%]
#-----------------------+---------------------+--------------

# ICRP Publ.151 p.284 Table 31.3
  Plasma                  Other0                    4.99
  Plasma                  RBC                       0.0582
  Plasma                  UB-con                    8.74
  Plasma                  Kidneys1                  0.524
  Plasma                  Kidneys2                  0.0582
  Plasma                  RC-con                    0.582
  Plasma                  Spleen                    0.00582
  Plasma                  Liver1                    0.466
  Plasma                  Other1                    0.262
  Plasma                  Other2                    0.0233
  Plasma                  T-bone-S                  0.518
  Plasma                  C-bone-S                  0.414
  Other0                  Plasma                    8.32
  RBC                     Plasma                    0.347
  Kidneys1                UB-con                    1.39
  Kidneys2                Plasma                    0.0019
  Liver1                  Plasma                    0.312
  Liver1                  Liver2                    0.0347
  Other1                  Plasma                    0.0693
  Other2                  Excreta                   0.0019
  Spleen                  Plasma                    0.0019
  T-bone-S                Plasma                    0.578
  T-bone-S                Exch-T-bone-V             0.116
  C-bone-S                Plasma                    0.578
  C-bone-S                Exch-C-bone-V             0.116
  Liver2                  Plasma                    0.0019
  Noch-T-bone-V           Plasma                    0.000493
  Noch-C-bone-V           Plasma                    0.0000821
  Exch-T-bone-V           T-bone-S                  0.00277
  Exch-T-bone-V           Noch-T-bone-V             0.00416
  Exch-C-bone-V           C-bone-S                  0.00277
  Exch-C-bone-V           Noch-C-bone-V             0.00416

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

  {Ta}/Blood1             Plasma                    ---
  {Ta}/Blood2             Plasma                 1000

  {Ta}/C-bone-S           C-bone-S                  ---
  {Ta}/T-bone-S           T-bone-S                  ---
  {Ta}/C-bone-V           Plasma                $( 3%/ 365)
  {Ta}/T-bone-V           Plasma                $(18%/ 365)

  {Ta}/Liver0             Plasma                    8.32
  {Ta}/Liver1             Plasma                    8.32
  {Ta}/Kidneys            Plasma                    8.32

  {Ta}/ST0                Plasma                    8.32
  {Ta}/ST1                Plasma                    8.32
