[{Cu}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
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
  exc   Excreta               ---


[{Cu}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

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

  {Ni}/Plasma             Plasma                    ---
  {Ni}/RBC                RBC                       ---

  {Ni}/C-bone-S           C-bone-S                  ---
  {Ni}/T-bone-S           T-bone-S                  ---
  {Ni}/C-bone-V           C-bone-V                  ---
  {Ni}/T-bone-V           T-bone-V                  ---

  {Ni}/Liver1             Liver1                    ---
  {Ni}/Liver2             Liver2                    ---
  {Ni}/Kidneys1           Kidneys1                  ---
  {Ni}/Kidneys2           Kidneys2                  ---

  {Ni}/ST0                ST0                       ---
  {Ni}/ST1                ST1                       ---

  {Ni}/Excreta            Excreta                   ---
