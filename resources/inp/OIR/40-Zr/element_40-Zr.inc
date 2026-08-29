[{Zr}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver0                Liver
  acc   Liver1                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other


[{Zr}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.274 Table 12.3
  Blood1                  Blood2                    2.0
  Blood1                  Liver0                    0.075
  Blood1                  Kidneys                   0.0125
  Blood1                  ST0                       2.0
  Blood1                  ST1                       0.0375
  Blood1                  UB-con                    0.1
  Blood1                  SI-conRe                  0.025
  Blood1                  T-bone-S                  0.375
  Blood1                  C-bone-S                  0.375
  Blood2                  Blood1                    0.462
  Liver0                  SI-conRe                  0.116
  Liver0                  Blood1                    0.116
  Liver0                  Liver1                    0.462
  Liver1                  Blood1                    0.01
  Kidneys                 Blood1                    0.01
  ST0                     Blood1                    0.462
  ST1                     Blood1                    0.02
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821
