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

  {Hf}/Blood1             Blood                     ---
  {Hf}/Blood2             Blood                  1000
  {Lu}/Blood              Blood                     ---

  {Hf,Lu}/C-bone-S        C-bone-S                  ---
  {Hf,Lu}/T-bone-S        T-bone-S                  ---
  {Hf,Lu}/C-bone-V        C-bone-V                  ---
  {Hf,Lu}/T-bone-V        T-bone-V                  ---

  {Lu}/C-marrow           C-marrow                  ---
  {Lu}/T-marrow           T-marrow                  ---
  {Hf}/Liver0             Liver1                    ---     # 確認書による
  {Hf}/Liver1             Liver2                    ---     # 確認書による
  {Lu}/Liver1             Liver1                    ---
  {Lu}/Liver2             Liver2                    ---
  {Hf}/Kidneys            Blood                     1.39
  {Lu}/Kidneys1           Kidneys1                  ---
  {Lu}/Kidneys2           Kidneys2                  ---
  {Lu}/Testes             Testes                    ---
  {Lu}/Ovaries            Ovaries                   ---

  {Hf}/ST0                Blood                     1.39
  {Hf}/ST1                Blood                     1.39
  {Lu}/ST0                ST0                       ---
  {Lu}/ST1                ST1                       ---
  {Lu}/ST2                ST2                       ---
