[{V}:compartment]
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
  acc   Other1                Other
  acc   Other2                Other


[{V}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.106 Table 12.3
  Blood1                  Blood2                    2.8
  Blood1                  Liver1                    0.24
  Blood1                  Kidneys                   0.4
  Blood1                  Other1                    2.44
  Blood1                  Other2                    0.24
  Blood1                  UB-con                    1.52
  Blood1                  SI-conRe                  0.12
  Blood1                  T-bone-S                  0.12
  Blood1                  C-bone-S                  0.12
  Blood2                  Blood1                    0.5
  Liver1                  SI-conRe                  0.09
  Liver1                  Blood1                    0.375
  Liver1                  Liver2                    0.035
  Liver2                  Blood1                    0.01
  Kidneys                 UB-con                    1.8
  Other1                  Blood1                    0.14
  Other2                  Blood1                    0.01
  T-bone-S                Blood1                    0.01
  C-bone-S                Blood1                    0.01

# ICRP Publ.151 p.113 Para.210
# > (210) Progeny of chromium addressed in this publication are radioisotopes of
# > vanadium. The model for vanadium as a progeny of chromium is an expanded
# > version of the characteristic model for vanadium with added compartments and
# > associated transfer coefficients needed to solve the linked biokinetic models for
# > chains headed by chromium. If vanadium is produced in a blood compartment of the
# > chromium model not contained in the characteristic model for vanadium, it is
# > assumed to transfer to its central blood compartment at a rate of 1000 d1, and to
# > follow its characteristic model thereafter. If produced in a tissue compartment not
# > contained in the characteristic model for vanadium, vanadium is assumed to transfer
# > to its central blood compartment at a rate of 0.14 d1, and to follow its characteristic
# > model thereafter.
# > (210) 本書で扱うクロムの娘核種は、バナジウムの放射性同位体です。クロムの娘核種としての
# > バナジウムに関するモデルは、バナジウム固有のモデルを拡張したものです。この拡張には、
# > クロムを起点とする崩壊系列について、連鎖的な生物学的動態モデルを解くために必要な
# > コンパートメントおよびそれに関連する移行係数が追加されています。クロムのモデルにおける
# > 血液コンパートメント（バナジウム固有のモデルには含まれないもの）内でバナジウムが
# > 生成される場合、そのバナジウムは1000 d⁻¹の速度でバナジウムの中心的血液コンパートメントへ
# > 移行し、その後はバナジウム固有のモデルに従うものと仮定されます。一方、バナジウム固有の
# > モデルに含まれない組織コンパートメント内でバナジウムが生成される場合、そのバナジウムは
# > 0.14 d⁻¹の速度でバナジウムの中心的血液コンパートメントへ移行し、その後はバナジウム固有の
# > モデルに従うものと仮定されます。

  {Cr}/Plasma1               Blood1              1000
  {Cr}/Plasma2               Blood1              1000

  {Cr}/C-bone-S              C-bone-S               ---
  {Cr}/T-bone-S              T-bone-S               ---

  {Cr}/Liver                 Blood1                 0.14
  {Cr}/Kidneys               Kidneys                ---

  {Cr}/Other1                Blood1                 0.14
  {Cr}/Other2                Blood1                 0.14
