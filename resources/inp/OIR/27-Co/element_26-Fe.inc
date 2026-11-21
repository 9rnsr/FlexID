[{Fe}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   OtherPlasma           Blood
  acc   PlasmaTrans           Blood
  acc   RBC                   Blood
  acc   MarrowSynthe          R-marrow
  acc   MarrowTransit         R-marrow
  acc   MarrowStorage         R-marrow
  acc   Liver1(Hepato)        Liver
  acc   Liver2(RE)            Liver
  acc   Spleen                Spleen
  acc   Other1(Trans)         Other
  acc   Other2(Parenc)        Other
  acc   Other3(RE)            Other
  exc   Excreta               ---


[{Fe}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.143 Table 7.3
  OtherPlasma             PlasmaTrans              70
  OtherPlasma             UB-con                    0.01
  OtherPlasma             RC-con                    0.1
  PlasmaTrans             MarrowSynthe              9.43
  PlasmaTrans             Liver1(Hepato)            0.555
  PlasmaTrans             Other1(Trans)             1.11
  RBC                     OtherPlasma               0.000833
  RBC                     MarrowTransit             0.00729
  RBC                     RC-con                    0.0002
  RBC                     UB-con                    0.000015
  MarrowSynthe            RBC                       0.243
  MarrowSynthe            MarrowTransit             0.104
  MarrowTransit           OtherPlasma               1.39
  MarrowTransit           MarrowStorage             0.0635
  MarrowTransit           Liver2(RE)                0.0106
  MarrowTransit           Spleen                    0.017
  MarrowTransit           Other3(RE)                0.0635
  MarrowStorage           MarrowTransit             0.0038
  Liver2(RE)              MarrowTransit             0.0038
  Spleen                  MarrowTransit             0.0038
  Other3(RE)              MarrowTransit             0.0038
  Liver1(Hepato)          PlasmaTrans               0.00364
  Liver1(Hepato)          SI-conRe                  0.00037
  Other1(Trans)           PlasmaTrans               0.888
  Other1(Trans)           Other2(Parenc)            0.222
  Other2(Parenc)          Other1(Trans)             0.00127
  Other2(Parenc)          Excreta                   0.00057
  Other2(Parenc)          UB-con                    0.00003

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

  {Co}/Blood1             OtherPlasma               ---
  {Co}/Blood2             OtherPlasma            1000

  {Co}/C-bone-S           OtherPlasma               1.39
  {Co}/T-bone-S           OtherPlasma               1.39
  {Co}/C-bone-V           OtherPlasma           $(0.03 / 365)
  {Co}/T-bone-V           OtherPlasma           $(0.18 / 365)

  {Co}/Liver1             OtherPlasma               1.39
  {Co}/Liver2             OtherPlasma               1.39
  {Co}/Kidneys1           OtherPlasma               1.39
  {Co}/Kidneys2           OtherPlasma               1.39

  {Co}/ST0                OtherPlasma               1.39
  {Co}/ST1                OtherPlasma               1.39
  {Co}/ST2                OtherPlasma               1.39
