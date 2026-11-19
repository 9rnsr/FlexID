[{Sb}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Thyroid               Thyroid
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Sb}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.60 Table 3.3
  Plasma                  Thyroid                   0.3
  Plasma                  UB-con                    9.0
  Plasma                  Liver1                    4.0
  Plasma                  Kidneys                   0.25
  Plasma                  RBC                       3.0
  Plasma                  ST0                      74.91
  Plasma                  ST1                       4.5
  Plasma                  ST2                       0.04
  Plasma                  C-bone-S                  2.0
  Plasma                  T-bone-S                  2.0
  Thyroid                 Plasma                    0.139
  RBC                     Plasma                    0.693
  Liver1                  Plasma                    0.437
  Liver1                  SI-conRe                  0.243
  Liver1                  Liver2                    0.0139
  Liver2                  Plasma                    0.000693
  Kidneys                 Plasma                    0.231
  ST0                     Plasma                    1.39
  ST1                     Plasma                    0.0139
  ST2                     Plasma                    0.000693
  C-bone-S                Plasma                    0.343
  T-bone-S                Plasma                    0.343
  C-bone-S                C-bone-V                  0.00347
  T-bone-S                T-bone-V                  0.00347
  C-bone-V                Plasma                    0.0000821
  T-bone-V                Plasma                    0.000493

$if progeny

# ICRP Publ.137 p.83 Para.177
# > (177) Antimony atoms produced in soft tissue compartments in the tellurium
# > model that are ambiguous with regard to the characteristic model for antimony
# > (compartments representing liver and other soft tissues are assumed to transfer to
# > the central blood compartment of that model (plasma) at the rate 0.693 d⁻¹
# > (t½=1 d). This is the highest rate of removal from all soft tissue compartments in
# > the characteristic model for antimony.
# > (177) テルルモデルにおける軟部組織コンパートメントで生成されるアンチモン原子のうち、
# > アンチモンの特性モデルとは明確に区別できないもの(肝臓やその他の軟部組織を表す
# > コンパートメント)は、そのモデルの中央血液コンパートメント(血漿)へ0.693 d⁻¹ (t½=1 d)
# > の速度で移動すると仮定されます。これは、アンチモンの特性モデルにおけるすべての
# > 軟部組織コンパートメントからの除去速度の中で最も高い値です。

  {Te}/Blood1             Plasma                    ---
  {Te}/Blood2             RBC                       ---

  {Te}/C-bone-S           C-bone-S                  ---
  {Te}/T-bone-S           T-bone-S                  ---
  {Te}/C-bone-V           C-bone-V                  ---
  {Te}/T-bone-V           T-bone-V                  ---

  {Te}/Thyroid            Thyroid                   ---
  {Te}/Liver              Plasma                    0.693
  {Te}/Kidneys            Kidneys                   ---

  {Te}/ST                 Plasma                    0.693

$endif
