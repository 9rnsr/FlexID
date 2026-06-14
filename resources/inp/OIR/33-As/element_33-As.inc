[{As}:compartment]
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
  acc   Other1                Other
  acc   Other2                Other
  acc   Other3                Other


[{As}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.183 Table 19.3
  Plasma                  RBC                       2.0
  Plasma                  Other1                   20
  Plasma                  Other2                    1.52
  Plasma                  Other3                    0.28
  Plasma                  Liver1                    2.4
  Plasma                  Kidneys1                  2.52
  Plasma                  Kidneys2                  0.28
  Plasma                  C-bone-S                  1.0
  Plasma                  T-bone-S                  1.0
  Plasma                  UB-con                    8.4
  Plasma                  RC-con                    0.6
  RBC                     Plasma                    0.3
  Other1                  Plasma                    0.6
  Other2                  Plasma                    0.08
  Other3                  Plasma                    0.018
  Liver1                  Plasma                    0.95
  Liver1                  Liver2                    0.05
  Liver2                  Plasma                    0.07
  Kidneys1                UB-con                    5.0
  Kidneys2                Plasma                    0.7
  C-bone-S                Plasma                    0.6
  T-bone-S                Plasma                    0.6
  C-bone-S                C-bone-V                  0.003
  T-bone-S                T-bone-V                  0.006
  C-bone-V                Plasma                    0.0000821
  T-bone-V                Plasma                    0.000493
