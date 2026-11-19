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

  {Te}/Blood1             Blood1                    ---
  {Te}/Blood2             Blood2                    ---

  {Te}/C-bone-S           C-bone-S                  ---
  {Te}/T-bone-S           T-bone-S                  ---
  {Te}/C-bone-V           C-bone-V                  ---
  {Te}/T-bone-V           T-bone-V                  ---

  {Te}/Thyroid            Thyroid                   ---
  {Te}/Liver              Liver                     ---
  {Te}/Kidneys            Kidneys                   ---

  {Te}/ST                 ST                        ---

$endif
