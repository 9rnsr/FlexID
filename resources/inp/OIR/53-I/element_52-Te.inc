[{Te}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood         # plasma
  acc   Blood2                Blood         # red blood cells
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Thyroid               Thyroid
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   ST                    Other


[{Te}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.73 Table 4.7
  Blood1                  UB-con                    0.751
  Blood1                  Kidneys                   0.0404
  Blood1                  Liver                     0.1213
  Blood1                  Blood2                    0.1011
  Blood1                  ST                        0.0768
  Blood1                  C-bone-S                  0.0202
  Blood1                  T-bone-S                  0.0404
  Blood1                  Thyroid                   0.0040
  Blood2                  Blood1                    0.0693
  Liver                   SI-conRe                  0.0693
  Thyroid                 Blood1                    0.0693
  Kidneys                 Blood1                    0.0693
  ST                      Blood1                    0.0693
  C-bone-S                Blood1                    0.0116
  T-bone-S                Blood1                    0.0116
  C-bone-S                C-bone-V                  0.000693
  T-bone-S                T-bone-V                  0.000693
  C-bone-V                Blood1                    0.000082
  T-bone-V                Blood1                    0.000493

# ICRP Publ.137 p.113 Para.250
# > (250) Tellurium atoms produced in the blood iodide compartment of the iodine
# > model are assigned to the plasma compartment (Blood 1) of the tellurium model.
# > Tellurium atoms produced in the blood organic iodine compartment of the iodine
# > model are assumed to transfer to Blood 1 at the rate 1000 d⁻¹. Tellurium atoms
# > produced at soft tissue sites in the iodine model are assumed to transfer to Blood
# > 1 at the rate 0.0693 d⁻¹ (t½=10 d), which is the rate of removal from all soft tissue
# > compartments in the characteristic model for tellurium.
# > (250) ヨウ素モデルの血液ヨウ化物コンパートメントで生成されたテルル原子は、
# > テルルモデルの血漿コンパートメント（血液1）に割り当てられます。ヨウ素モデルの
# > 血液有機ヨウ素コンパートメントで生成されたテルル原子は、1000 d⁻¹の速度で血液1に
# > 移動するものとします。ヨウ素モデルの軟部組織部位で生成されたテルル原子は、
# > 0.0693 d⁻¹（t½=10 d）の速度で血液1に移動するものとします。これは、テルルの
# > 特性モデルにおけるすべての軟部組織コンパートメントからの除去速度です。

  {I}/Blood1              Blood1                    ---
  {I}/Blood2              Blood1                 1000
  {Te}/Blood1             Blood1                    ---
  {Te}/Blood2             Blood2                    ---

  {Te}/C-bone-S           C-bone-S                  ---
  {Te}/T-bone-S           T-bone-S                  ---
  {Te}/C-bone-V           C-bone-V                  ---
  {Te}/T-bone-V           T-bone-V                  ---

  {I}/S-glands            Blood1                    0.0693
  {I}/OralcavityRe        Oralcavity                ---
  {I}/Oesophagus-sRe      Oesophagus-s              ---
  {I}/St-conRe            St-con                    ---
  {I}/St-wall             Blood1                    0.0693
  {I}/Thyroid1            Blood1                    0.0693
  {I}/Thyroid2            Blood1                    0.0693
  {Te}/Thyroid            Thyroid                   ---
  {I}/Liver1              Blood1                    0.0693
  {I}/Liver2              Blood1                    0.0693
  {Te}/Liver              Liver                     ---
  {I}/Kidneys1            Blood1                    0.0693
  {I}/Kidneys2            Blood1                    0.0693
  {Te}/Kidneys            Kidneys                   ---

  {I}/Other1              Blood1                    0.0693
  {I}/Other2              Blood1                    0.0693
  {I}/Other3              Blood1                    0.0693
  {I}/Other4              Blood1                    0.0693
  {Te}/ST                 ST                        ---
