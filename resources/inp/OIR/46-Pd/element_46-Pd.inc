[{Pd}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissue
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Pd}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.223 Table 24.3
  Blood1                  SI-conRe                  4.0
  Blood1                  UB-con                   20
  Blood1                  Liver1                   12
  Blood1                  Kidneys1                  8.0
  Blood1                  Kidneys2                  4.0
  Blood1                  Blood2                   27
  Blood1                  ST0                      10
  Blood1                  ST1                      10
  Blood1                  ST2                       1.0
  Blood1                  C-bone-S                  1.0
  Blood1                  T-bone-S                  3.0
  Blood2                  Blood1                    2.773
  Liver1                  Blood1                    0.04621
  Liver1                  SI-conRe                  0.09242
  Liver1                  Liver2                    0.1386
  Liver2                  Blood1                    0.01386
  Kidneys1                UB-con                    0.2773
  Kidneys2                Blood1                    0.01386
  ST0                     Blood1                    0.1386
  ST1                     Blood1                    0.01386
  ST2                     Blood1                    0.0019
  C-bone-S                Blood1                    0.03697
  T-bone-S                Blood1                    0.03697
  C-bone-S                C-bone-V                  0.009242
  T-bone-S                T-bone-V                  0.009242
  C-bone-V                Blood1                    0.0000821
  T-bone-V                Blood1                    0.000493
