[{Al}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Other0                Other
  acc   Other1                Other


[{Al}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.64 Table 6.3
  Blood1                  UB-con                    9.98
  Blood1                  RC-con                    0.166
  Blood1                  T-bone-S                  0.0832
  Blood1                  C-bone-S                  0.0832
  Blood1                  Other0                    5.74
  Blood1                  Other1                    0.582
  Blood2                  Blood1                    0.035
  Other0                  Blood1                    0.5
  Other1                  Blood2                    0.0005
  T-bone-S                Blood2                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood2                    0.000493
  C-bone-S                Blood2                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood2                    0.0000821

# ICRP Publ.151 p.49 Par.70
# > (70) The only progeny of magnesium addressed in this publication is 28Al produced
# > by decay of 28Mg. The model for aluminium as a progeny of magnesium is an
# > expansion of the characteristic model for aluminium with added compartments and
# > associated transfer coefficients needed to solve the linked biokinetic models for magnesium
# > and aluminium (see Annex B). For 28Al produced in a compartment not
# > contained in the characteristic model for aluminium, 28Al is assumed to transfer to
# > the central blood compartment of that model at a rate of 1000 d1 if produced in a
# > blood compartment, and 0.5 d1 if produced in a tissue compartment, and to follow
# > the characteristic model for aluminium thereafter.
# > (70) 本書で扱うマグネシウムの娘核種は、28Mgの崩壊によって生成される28Alのみです。
# > マグネシウムの娘核種としてのアルミニウムに関するモデルは、アルミニウムの標準的な
# > モデルを拡張したものであり、マグネシウムとアルミニウムの連動した生物学的動態モデルを
# > 解くために必要なコンパートメントおよびそれに関連する移行係数が追加されています（付録Bを参照）。
# > アルミニウムの標準的なモデルに含まれないコンパートメント内で28Alが生成される場合、
# > その28Alは、血液コンパートメント内で生成された場合は1000 d⁻¹の速度で、組織コンパートメント内で
# > 生成された場合は0.5 d⁻¹の速度で、当該モデルの中心的な血液コンパートメントへ移行し、
# > その後はアルミニウムの標準的なモデルに従うものと仮定されます。

  {Mg}/Plasma             Blood1                 1000
  {Mg}/RBC                Blood1                 1000

  {Mg}/C-bone-S           C-bone-S                  ---
  {Mg}/T-bone-S           T-bone-S                  ---
  {Mg}/C-bone-V           C-bone-V                  ---
  {Mg}/T-bone-V           T-bone-V                  ---

  {Mg}/Other0             Blood1                    0.5
  {Mg}/Other1             Blood1                    0.5
  {Mg}/Other2             Blood1                    0.5
