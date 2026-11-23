[{Lu}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Lu}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.141 p.40 Table 2.9
  Blood                   Liver1                    1.16
  Blood                   T-bone-S                  6.41
  Blood                   C-bone-S                  6.41
  Blood                   Kidneys1                  0.349
  Blood                   Kidneys2                  0.117
  Blood                   UB-con                    4.66
  Blood                   RC-con                    0.233
  Blood                   Testes                    0.00815
  Blood                   Ovaries                   0.00256
  Blood                   ST0                       9.98
  Blood                   ST1                       3.48
  Blood                   ST2                       0.466
  Liver1                  SI-conRe                  0.00231
  Liver1                  Liver2                    0.0208
  Liver2                  Blood                     0.0019
  T-bone-S                T-marrow                  0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                T-marrow                  0.000493
  T-marrow                Blood                     0.0076
  C-bone-S                C-marrow                  0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                C-marrow                  0.0000821
  C-marrow                Blood                     0.0076
  Kidneys1                UB-con                    0.099
  Kidneys2                Blood                     0.00139
  Testes                  Blood                     0.00038
  Ovaries                 Blood                     0.00038
  ST0                     Blood                     1.39
  ST1                     Blood                     0.00693
  ST2                     Blood                     0.000128

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

  {Ta,Hf}/Blood1          Blood                  1000
  {Ta,Hf}/Blood2          Blood                  1000
  {Lu}/Blood              Blood                     ---

  {Ta,Hf,Lu}/C-bone-S     C-bone-S                  ---
  {Ta,Hf,Lu}/T-bone-S     T-bone-S                  ---
  {Ta,Hf,Lu}/C-bone-V     C-bone-V                  ---
  {Ta,Hf,Lu}/T-bone-V     T-bone-V                  ---

  {Lu}/C-marrow           C-marrow                  ---
  {Lu}/T-marrow           T-marrow                  ---
  {Ta,Hf}/Liver0          Liver1                    ---
  {Ta,Hf}/Liver1          Liver2                    ---
  {Lu}/Liver1             Liver1                    ---
  {Lu}/Liver2             Liver2                    ---
  {Ta,Hf}/Kidneys         Blood                     1.39
  {Lu}/Kidneys1           Blood                     1.39
  {Lu}/Kidneys2           Blood                     1.39
  {Lu}/Testes             Testes                    ---
  {Lu}/Ovaries            Ovaries                   ---

  {Ta,Hf}/ST0             Blood                     1.39
  {Ta,Hf}/ST1             Blood                     1.39
  {Lu}/ST0                ST0                       ---
  {Lu}/ST1                ST1                       ---
  {Lu}/ST2                ST2                       ---
