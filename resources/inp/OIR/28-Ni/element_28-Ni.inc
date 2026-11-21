[{Ni}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
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
  exc   Excreta               ---


[{Ni}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.146 Table 15.4
  Plasma                  Kidneys1                 12.7
  Plasma                  SI-conRe                  0.18
  Plasma                  Liver1                    0.45
  Plasma                  C-bone-S                  0.675
  Plasma                  T-bone-S                  0.675
  Plasma                  ST0                       7.2
  Plasma                  ST1                       1.2
  Plasma                  RBC                       0.075
  Plasma                  Excreta                   0.34
  RBC                     Plasma                    0.231
  Kidneys1                Plasma                   35
  Kidneys1                UB-con                   15
  Kidneys1                Kidneys2                  0.0013
  Kidneys2                Plasma                    0.00173
  Liver1                  Plasma                    1.9
  Liver1                  Liver2                    0.29
  Liver1                  SI-conRe                  1.46
  Liver2                  Plasma                    0.00173
  ST0                     Plasma                    1.9
  ST1                     Plasma                    0.00173
  C-bone-S                Plasma                    1.9
  C-bone-S                C-bone-V                  0.0192
  T-bone-S                Plasma                    1.9
  T-bone-S                T-bone-V                  0.0192
  C-bone-V                Plasma                    0.0000821
  T-bone-V                Plasma                    0.000493
