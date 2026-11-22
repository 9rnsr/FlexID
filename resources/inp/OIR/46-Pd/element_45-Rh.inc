[{Rh}:compartment]
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


[{Rh}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.216 Table 23.3
  Blood1                  SI-conRe                  3.0
  Blood1                  UB-con                   17
  Blood1                  Liver1                   12
  Blood1                  Kidneys1                  7.76
  Blood1                  Kidneys2                  0.24
  Blood1                  Blood2                   27
  Blood1                  ST0                      15
  Blood1                  ST1                       5.0
  Blood1                  ST2                       5.0
  Blood1                  C-bone-S                  2.0
  Blood1                  T-bone-S                  6.0
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

  {Pd,Rh}/Blood1          Blood1                    ---
  {Pd,Rh}/Blood2          Blood2                    ---

  {Pd,Rh}/C-bone-S        C-bone-S                  ---
  {Pd,Rh}/T-bone-S        T-bone-S                  ---
  {Pd,Rh}/C-bone-V        C-bone-V                  ---
  {Pd,Rh}/T-bone-V        T-bone-V                  ---

  {Pd,Rh}/Liver1          Liver1                    ---
  {Pd,Rh}/Liver2          Liver2                    ---
  {Pd,Rh}/Kidneys1        Kidneys1                  ---
  {Pd,Rh}/Kidneys2        Kidneys2                  ---

  {Pd,Rh}/ST0             ST0                       ---
  {Pd,Rh}/ST1             ST1                       ---
  {Pd,Rh}/ST2             ST2                       ---
