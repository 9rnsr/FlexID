[{Ge}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other                 Other


[{Ge}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.173 Table 18.3
  Blood                   Other                     0.89
  Blood                   Kidneys                   0.2
  Blood                   Liver                     0.4
  Blood                   UB-con                    8.3
  Blood                   RC-con                    0.01
  Blood                   T-bone-S                  0.1
  Blood                   C-bone-S                  0.1
  Other                   Blood                     0.3
  Kidneys                 UB-con                    1.2
  Liver                   Blood                     0.9
  T-bone-S                Blood                     0.3
  C-bone-S                Blood                     0.3
  T-bone-S                T-bone-V                  0.0015
  C-bone-S                C-bone-V                  0.0015
  T-bone-V                Blood                     0.000493
  C-bone-V                Blood                     0.0000821

# ICRP Publ.151 p.182 Para.363
# > (363) Progeny of arsenic addressed in this publication are radioisotopes of germanium.
# > The model for germanium produced in systemic compartments by decay of
# > arsenic is an expanded version of the characteristic model for germanium, with added
# > compartments and associated transfer coefficients needed to solve the linked biokinetic
# > models for arsenic and germanium. Germanium produced in compartments of the
# > model for arsenic that are not addressed in the characteristic model for germanium is
# > assumed to transfer to the central blood compartment of the germanium model at a rate
# > of 1000 d⁻¹ and to follow the characteristic model for germanium thereafter.
# > (363) 本書で扱うヒ素の生成核種は、ゲルマニウムの放射性同位体です。ヒ素の崩壊によって
# > 生体内の各コンパートメントで生成されるゲルマニウムのモデルは、ゲルマニウムに関する
# > 標準的なモデルを拡張したものです。この拡張では、ヒ素とゲルマニウムの連動した生物動態モデルを
# > 解くために必要なコンパートメントおよびそれに関連する移行係数が追加されています。ヒ素の
# > モデルにおけるコンパートメントのうち、ゲルマニウムの標準的なモデルには含まれていない
# > コンパートメントで生成されたゲルマニウムは、1000 d⁻¹の速度でゲルマニウムモデルの中心血液
# > コンパートメントへ移行し、その後はゲルマニウムの標準的なモデルに従うものと仮定されます。

  {As}/Plasma             Blood                  1000
  {As}/RBC                Blood                  1000

  {As}/C-bone-S           C-bone-S                  ---
  {As}/T-bone-S           T-bone-S                  ---
  {As}/C-bone-V           C-bone-V                  ---
  {As}/T-bone-V           T-bone-V                  ---

  {As}/Liver1             Blood                  1000
  {As}/Liver2             Blood                  1000
  {As}/Kidneys1           Blood                  1000
  {As}/Kidneys2           Blood                  1000

  {As}/Other1             Blood                  1000
  {As}/Other2             Blood                  1000
  {As}/Other3             Blood                  1000
