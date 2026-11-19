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

# ICRP Publ.137 p.113 Para.251
# > (251) Antimony produced in the blood iodide compartment of the model for
# > iodine or the plasma compartment of the model for tellurium (Blood 1) is assigned
# > to plasma in the model for antimony. Antimony produced in the blood organic
# > iodine compartment of the iodine model is assumed to transfer to plasma at the
# > rate 1000 d⁻¹. Antimony produced at any soft tissue site in the iodine or tellurium
# > model is assumed to transfer to plasma at the rate 0.693 d⁻¹ (t½=1 d), which is the
# > highest rate of removal from soft tissue compartments in the characteristic model for
# > antimony. Antimony produced in a bone compartment of the tellurium model is
# > assumed to behave as if entering that site as a parent radionuclide.
# > (251) ヨウ素モデルの血液ヨウ化物コンパートメント、またはテルルモデルの
# > 血漿コンパートメント（血液1）で生成されたアンチモンは、アンチモンモデルでは
# > 血漿に割り当てられます。ヨウ素モデルの血液有機ヨウ素コンパートメントで生成された
# > アンチモンは、1000 d⁻¹の速度で血漿に移行すると仮定されます。ヨウ素モデルまたは
# > テルルモデルの任意の軟組織部位で生成されたアンチモンは、アンチモンの特性モデルにおける
# > 軟組織コンパートメントからの除去速度の最大値である0.693 d⁻¹（t½=1 d）で血漿に移行すると
# > 仮定されます。テルルモデルの骨コンパートメントで生成されたアンチモンは、
# > 親放射性核種としてその部位に入るかのように振舞うと仮定されます。

  {I}/Blood1              Plasma                    ---
  {I}/Blood2              Plasma                 1000
  {Te}/Blood1             Plasma                    ---
  {Te}/Blood2             Plasma                 1000

  {Te}/C-bone-S           C-bone-S                  ---
  {Te}/T-bone-S           T-bone-S                  ---
  {Te}/C-bone-V           C-bone-V                  ---
  {Te}/T-bone-V           T-bone-V                  ---

  {I}/S-glands            Plasma                    0.693
  {I}/OralcavityRe        Oralcavity                ---
  {I}/Oesophagus-sRe      Oesophagus-s              ---
  {I}/St-conRe            St-con                    ---
  {I}/St-wall             Plasma                    0.693
  {I}/Thyroid1            Plasma                    0.693
  {I}/Thyroid2            Plasma                    0.693
  {Te}/Thyroid            Plasma                    0.693
  {I}/Liver1              Plasma                    0.693
  {I}/Liver2              Plasma                    0.693
  {Te}/Liver              Plasma                    0.693
  {I}/Kidneys1            Plasma                    0.693
  {I}/Kidneys2            Plasma                    0.693
  {Te}/Kidneys            Plasma                    0.693

  {I}/Other1              Plasma                    0.693
  {I}/Other2              Plasma                    0.693
  {I}/Other3              Plasma                    0.693
  {I}/Other4              Plasma                    0.693
  {Te}/ST                 Plasma                    0.693
