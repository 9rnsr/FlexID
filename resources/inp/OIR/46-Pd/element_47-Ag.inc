[{Ag}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other


[{Ag}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.233 Table 25.3
  Blood1                  Blood2                    0.72
  Blood1                  Kidneys                   1.2
  Blood1                  Liver1                   30
  Blood1                  T-bone-S                  1.2
  Blood1                  C-bone-S                  1.2
  Blood1                  ST0                      12.8
  Blood1                  ST1                      12.8
  Blood1                  UB-con                    0.13
  Blood2                  Blood1                    0.231
  Kidneys                 Blood1                    0.8
  Liver1                  SI-conRe                  1.0
  Liver1                  Liver2                    9.0
  Liver2                  Blood1                    0.2
  ST0                     Blood1                    8.0
  ST1                     Blood1                    0.4
  T-bone-S                Blood1                    0.07
  C-bone-S                Blood1                    0.07

# ICRP Publ.151 p.223 Para.434
# > (434) Progeny of palladium addressed in this publication are isotopes of rhodium
# > and silver. The characteristic model for rhodium used in this publication is applied to
# > rhodium as a progeny of palladium. The model for silver as a progeny of osmium is
# > an expansion of the characteristic model for silver with added compartments and
# > associated transfer coefficients needed to solve the linked biokinetic models for
# > chains headed by palladium (see Annex B). For a silver isotope produced in a compartment
# > not contained in the characteristic model for silver, the isotope is assumed
# > to transfer to the central blood compartment of the model for silver at a rate of
# > 1000 d⁻¹ if produced in a blood compartment and 8.0 d⁻¹ if produced in a tissue
# > compartment, and to follow the characteristic model for silver thereafter.
# > (434) 本書で扱うパラジウムの娘核種は、ロジウムおよび銀の同位体です。本書で用いる
# > ロジウムの特性モデルは、パラジウムの娘核種としてのロジウムに適用されます。オスミウムの
# > 娘核種としての銀に関するモデルは、銀の特性モデルを拡張したものであり、パラジウムを
# > 起点とする崩壊系列について、連鎖した生物動態モデルを解くために必要なコンパートメント
# > およびそれに関連する移行係数が追加されています（付録Bを参照）。銀の特性モデルに含まれない
# > コンパートメントで生成される銀同位体については、それが血液コンパートメントで生成される
# > 場合は1000 d⁻¹の速度で、組織コンパートメントで生成される場合は8.0 d⁻¹の速度で、銀モデルの
# > 中心的な血液コンパートメントへ移行し、その後は銀の特性モデルに従うものと仮定されます。

  {Pd}/Blood1             Blood1                    ---
  {Pd}/Blood2             Blood1                 1000

  {Pd}/C-bone-S           C-bone-S                  ---
  {Pd}/T-bone-S           T-bone-S                  ---
  {Pd}/C-bone-V           Blood1                    8.0
  {Pd}/T-bone-V           Blood1                    8.0

  {Pd}/Liver1             Liver1                    ---
  {Pd}/Liver2             Liver2                    ---
  {Pd}/Kidneys1           Blood1                    8.0
  {Pd}/Kidneys2           Blood1                    8.0

  {Pd}/ST0                Blood1                    8.0
  {Pd}/ST1                Blood1                    8.0
  {Pd}/ST2                Blood1                    8.0
