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

$if progeny

# ICRP Publ.137 p.61 Para.134
# > (134) Tellurium atoms produced at soft tissue sites in the antimony model that are
# > ambiguous with regard to the characteristic model for tellurium (ST0, ST1, ST2,
# > Liver 0, and Liver 1) are assumed to be transferred to the central blood compartment
# > of that model (plasma, called ‘Blood 1’ in Fig. 4.1) at the rate 0.0693 d⁻¹ (t½=10 d).
# > This is the rate of removal from all soft tissue compartments in the characteristic
# > model for tellurium.
# > (134) アンチモンモデルの軟組織部位で生成されたテルル原子のうち、テルルの特性モデル
# > （ST0、ST1、ST2、肝臓0、肝臓1）に関して曖昧な部位は、そのモデルの中央の血液コンパートメント
# > （血漿、図4.1では「血液1」と呼ばれる）に0.0693 d⁻¹（t½=10 d）の速度で移行すると仮定されます。
# > これは、テルルの特性モデルにおけるすべての軟組織コンパートメントからの除去速度です。

  {Sb}/Plasma             Blood1                    ---
  {Sb}/RBC                Blood2                    ---
  {Te}/Blood1             Blood1                    ---     # plasma
  {Te}/Blood2             Blood2                    ---     # red blood cell

  {Sb,Te}/C-bone-S        C-bone-S                  ---
  {Sb,Te}/T-bone-S        T-bone-S                  ---
  {Sb,Te}/C-bone-V        C-bone-V                  ---
  {Sb,Te}/T-bone-V        T-bone-V                  ---

  {Sb,Te}/Thyroid         Thyroid                   ---
  {Sb}/Liver1             Blood1                    0.0693
  {Sb}/Liver2             Blood1                    0.0693
  {Te}/Liver              Liver                     ---
  {Sb,Te}/Kidneys         Kidneys                   ---

  {Sb}/ST0                Blood1                    0.0693
  {Sb}/ST1                Blood1                    0.0693
  {Sb}/ST2                Blood1                    0.0693
  {Te}/ST                 ST                        ---

$endif
