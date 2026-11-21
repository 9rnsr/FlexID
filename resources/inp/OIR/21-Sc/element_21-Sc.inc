[{Sc}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Spleen                Spleen
  acc   ST1                   Other
  acc   ST2                   Other


[{Sc}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.92 Table 10.3
  Blood1                  Blood2                    0.45
  Blood1                  UB-con                    0.054
  Blood1                  Liver1                    0.60
  Blood1                  Kidneys                   0.09
  Blood1                  Spleen                    0.06
  Blood1                  T-marrow                  0.15
  Blood1                  C-marrow                  0.15
  Blood1                  T-bone-S                  0.15
  Blood1                  C-bone-S                  0.15
  Blood1                  ST1                       0.60
  Blood1                  ST2                       0.546
  Blood2                  Blood1                    0.462
  Liver1                  SI-conRe                  0.0578
  Liver1                  Liver2                    0.0578
  Liver1                  Blood1                    0.116
  Liver2                  Blood1                    0.00693
  Kidneys                 Blood1                    0.0347
  Spleen                  Blood1                    0.0019
  T-marrow                Blood1                    0.00693
  C-marrow                Blood1                    0.00693
  ST1                     Blood1                    0.231
  ST2                     Blood1                    0.00693
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821

$if progeny

# ICRP Publ.151 p.91 Para.161
# > (161) The only scandium chain addressed in this publication is the two-member
# > chain consisting of 44mSc and its progeny 44Sc. The biokinetics of the progeny are
# > assumed to be the same as those of the parent from its time of production in a
# > systemic compartment.
# > (161) 本書で取り上げるスカンジウムの系列は、44mScとその娘核種である44Scからなる
# > 2段階の系列のみです。娘核種の生体内動態は、生体内のコンパートメントにおいて
# > 生成された時点から、親核種と同一であると仮定されます。

  {Sc}/Blood1             Blood1                    ---
  {Sc}/Blood2             Blood2                    ---

  {Sc}/C-bone-S           C-bone-S                  ---
  {Sc}/T-bone-S           T-bone-S                  ---
  {Sc}/C-bone-V           C-bone-V                  ---
  {Sc}/T-bone-V           T-bone-V                  ---

  {Sc}/C-marrow           C-marrow                  ---
  {Sc}/T-marrow           T-marrow                  ---
  {Sc}/Liver1             Liver1                    ---
  {Sc}/Liver2             Liver2                    ---
  {Sc}/Kidneys            Kidneys                   ---
  {Sc}/Spleen             Spleen                    ---

  {Sc}/ST1                ST1                       ---
  {Sc}/ST2                ST2                       ---

$endif
