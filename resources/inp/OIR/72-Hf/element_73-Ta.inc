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

# ICRP Publ.151 p.271 Para.544
# > (544) Progeny of hafnium addressed in this publication are radioisotopes of hafnium,
# > tantalum, and lutetium. The model for hafnium as a parent is applied to
# > hafnium produced by decay of another hafnium isotope. The models for tantalum
# > and lutetium as progeny of hafnium are expansions of the characteristic models for
# > these elements with added compartments and associated transfer coefficients needed
# > to solve the linked biokinetic models for chains headed by hafnium (see Annex B).
# > For lutetium as a progeny of hafnium, the issue arises that the progeny is produced
# > in some compartments not contained in the characteristic model for lutetium. If
# > produced in a blood compartment not contained in the model for lutetium, the
# > progeny is assumed to transfer to the central blood compartment of its characteristic
# > biokinetic model at a rate of 1000 d⁻¹, and to follow that model thereafter. If
# > produced in a tissue compartment not in the lutetium model, the progeny is assumed
# > to transfer to the central blood compartment of the characteristic model for lutetium
# > at a rate of 1.39 d⁻¹, and to follow that model thereafter.
# > (544) 本書で扱うハフニウムの娘核種は、ハフニウム、タンタル、およびルテチウムの
# > 放射性同位体です。親核種としてのハフニウムに関するモデルは、別のハフニウム同位体の
# > 崩壊によって生成されるハフニウムにも適用されます。ハフニウムの娘核種であるタンタルおよび
# > ルテチウムのモデルは、それぞれの元素固有のモデルを拡張したものです。これには、
# > ハフニウムを起点とする崩壊系列に対して、連鎖的な生物学的動態モデルを解くために必要な
# > コンパートメントと、それに関連する移行係数が追加されています（付録Bを参照）。ハフニウムの
# > 娘核種としてのルテチウムに関しては、ルテチウム固有のモデルには含まれていない
# > コンパートメント内で生成されるという問題が生じます。ルテチウムのモデルに含まれていない
# > 血液コンパートメント内で生成された場合、その娘核種は、ルテチウム固有の生物学的動態モデル
# > における中心血液コンパートメントへ1000 d⁻¹の速度で移行し、その後はそのモデルに従うものと
# > 仮定されます。一方、ルテチウムのモデルに含まれていない組織コンパートメント内で
# > 生成された場合、その娘核種は、ルテチウム固有のモデルにおける中心血液コンパートメントへ
# > 1.39 d⁻¹の速度で移行し、その後はそのモデルに従うものと仮定されます。

  {Hf,Ta}/Blood1          Blood1                    ---
  {Hf,Ta}/Blood2          Blood2                    ---

  {Hf,Ta}/C-bone-S        C-bone-S                  ---
  {Hf,Ta}/T-bone-S        T-bone-S                  ---
  {Hf,Ta}/C-bone-V        C-bone-V                  ---
  {Hf,Ta}/T-bone-V        T-bone-V                  ---

  {Hf,Ta}/Liver0          Liver0                    ---
  {Hf,Ta}/Liver1          Liver1                    ---
  {Hf,Ta}/Kidneys         Kidneys                   ---

  {Hf,Ta}/ST0             ST0                       ---
  {Hf,Ta}/ST1             ST1                       ---
