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

  {Sb}/Plasma             Plasma                    ---
  {Sb}/RBC                RBC                       ---

  {Sb}/C-bone-S           C-bone-S                  ---
  {Sb}/T-bone-S           T-bone-S                  ---
  {Sb}/C-bone-V           C-bone-V                  ---
  {Sb}/T-bone-V           T-bone-V                  ---

  {Sb}/Thyroid            Thyroid                   ---
  {Sb}/Liver1             Liver1                    ---
  {Sb}/Liver2             Liver2                    ---
  {Sb}/Kidneys            Kidneys                   ---

  {Sb}/ST0                ST0                       ---
  {Sb}/ST1                ST1                       ---
  {Sb}/ST2                ST2                       ---

$endif
