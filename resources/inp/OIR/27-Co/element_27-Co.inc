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

$if progeny

# ICRP Publ.134 p.171 Para.366
# > (366) Radioactive progeny considered in the derivation of dose coefficients for
# > cobalt isotopes are isotopes of cobalt or iron. Cobalt produced in vivo by radioactive
# > decay is assigned the characteristic model for cobalt, i.e., the systemic model for
# > cobalt as a parent. The model for iron produced in vivo is a modification of the
# > characteristic model for iron described in this report. The compartment in the characteristic
# > model for iron called Other plasma is identified with the compartment in
# > the cobalt model called Blood 1. Iron produced in any compartment of the cobalt
# > model other than Blood 1 is assigned a transfer rate to Other plasma and assumed to
# > follow the characteristic model for iron after reaching Other plasma. The following
# > transfer coefficients from compartments of the cobalt model to Other plasma are
# > assigned: Blood 2, 1000 d1; soft tissue and bone surface compartments, 1.39 d1
# > (the highest transfer rate from a tissue compartment to Other plasma in the characteristic
# > model for iron); bone volume compartments, the rate of bone turnover.
# > (366) コバルト同位体の線量係数を導出する際に考慮される放射性娘核種は、コバルトまたは鉄の
# > 同位体である。放射性崩壊によって生体内で生成されるコバルトには、コバルト固有のモデル、
# > すなわち親核種としてのコバルトに対する全身モデルが適用される。生体内で生成される鉄に対しては、
# > 本報告書で記述されている鉄固有のモデルを修正したものを用いる。鉄固有のモデルにおける
# > 「その他の血漿（Other plasma）」コンパートメントは、コバルトモデルにおける「血液1（Blood 1）」
# > コンパートメントと同一のものとして扱う。コバルトモデルにおいて「血液1」以外のコンパートメントで
# > 生成された鉄については、「その他の血漿」への移行速度が設定され、「その他の血漿」に到達した後は
# > 鉄固有のモデルに従うものと仮定する。コバルトモデルの各コンパートメントから「その他の血漿」への
# > 移行係数は、以下のように設定される。「血液2（Blood 2）」からは1000 d⁻¹、軟組織および骨表面の
# > コンパートメントからは1.39 d⁻¹（鉄固有のモデルにおいて、組織コンパートメントから
# > 「その他の血漿」への移行速度として最大の値）、そして骨体積コンパートメントからは
# > 骨代謝回転速度が適用される。

  {Co}/Blood1             Blood1                    ---
  {Co}/Blood2             Blood2                    ---

  {Co}/C-bone-S           C-bone-S                  ---
  {Co}/T-bone-S           T-bone-S                  ---
  {Co}/C-bone-V           C-bone-V                  ---
  {Co}/T-bone-V           T-bone-V                  ---

  {Co}/Liver1             Liver1                    ---
  {Co}/Liver2             Liver2                    ---
  {Co}/Kidneys1           Kidneys1                  ---
  {Co}/Kidneys2           Kidneys2                  ---

  {Co}/ST0                ST0                       ---
  {Co}/ST1                ST1                       ---
  {Co}/ST2                ST2                       ---

$endif
