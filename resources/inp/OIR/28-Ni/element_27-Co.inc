[{Co}:compartment]
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
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Co}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.169 Table 8.3
  Blood1                  Liver1                   70
  Blood1                  UB-con                   60
  Blood1                  RC-con                    4.0
  Blood1                  ST0                      18
  Blood1                  ST1                      10
  Blood1                  ST2                       4.0
  Blood1                  C-bone-S                  6.0
  Blood1                  T-bone-S                  6.0
  Blood1                  Kidneys1                  9.0
  Blood1                  Kidneys2                  1.0
  Blood1                  Blood2                   12
  Blood2                  Blood1                    0.693
  Liver1                  SI-conRe                  0.0924
  Liver1                  Blood1                    0.347
  Liver1                  Liver2                    0.0231
  Liver2                  Blood1                    0.0019
  ST0                     Blood1                    0.099
  ST1                     Blood1                    0.0139
  ST2                     Blood1                    0.00095
  C-bone-S                Blood1                    0.0842
  C-bone-S                C-bone-V                  0.0149
  T-bone-S                Blood1                    0.0842
  T-bone-S                T-bone-V                  0.0149
  C-bone-V                Blood1                    0.0000821
  T-bone-V                Blood1                    0.000493
  Kidneys1                UB-con                    0.462
  Kidneys2                Blood1                    0.0019

# ICRP Publ.151 p.146 Para.301
# > (301) Progeny of nickel addressed in this publication are 56Co (half-time 77.2 d),
# > 57Co (272 d), and 66Cu (5.12 min). 66Cu produced in a systemic compartment is
# > assumed to decay at its site of production. The model for cobalt as a progeny of
# > nickel is an expansion of the characteristic model for cobalt with added compartments
# > and associated transfer coefficients needed to solve the linked biokinetic
# > models for nickel and cobalt (see Annex B). If produced in a compartment not
# > explicitly identified in its characteristic model, cobalt is assumed to transfer to its
# > central blood compartment at 1000 d⁻¹ if produced in a blood compartment, and at
# > 0.099 d⁻¹ if produced in a tissue compartment, and to follow its characteristic model
# > thereafter.
# > (301) 本書で扱うニッケルの娘核種は、56Co（半減期77.2日）、57Co（272日）、および66Cu
# > （5.12分）です。生体内のコンパートメントで生成された66Cuは、その生成部位で崩壊するものと
# > 仮定されます。ニッケルの娘核種としてのコバルトに関するモデルは、コバルト固有のモデルを
# > 拡張したものです。これには、ニッケルとコバルトの連動した生物学的動態モデルを解く
# > ために必要なコンパートメントおよびそれに関連する移行係数が追加されています（付録Bを参照）。
# > コバルトが、その固有モデルにおいて明示的に定義されていないコンパートメントで生成された場合、
# > 血液コンパートメントで生成されたときは1000 d⁻¹の速度で、組織コンパートメントで
# > 生成されたときは0.099 d⁻¹の速度で、それぞれ中心血液コンパートメントへ移行し、その後は
# > コバルト固有のモデルに従うものと仮定されます。

  {Ni}/Plasma             Blood1                    ---
  {Ni}/RBC                Blood1                 1000

  {Ni}/C-bone-S           C-bone-S                  --
  {Ni}/T-bone-S           T-bone-S                  --
  {Ni}/C-bone-V           C-bone-V                  --
  {Ni}/T-bone-V           T-bone-V                  --

  {Ni}/Liver1             Liver1                    ---
  {Ni}/Liver2             Liver2                    ---
  {Ni}/Kidneys1           Kidneys1                  ---
  {Ni}/Kidneys2           Kidneys2                  ---

  {Ni}/ST0                Blood1                    0.099
  {Ni}/ST1                Blood1                    0.099
