[{C}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood0                Blood         # Generic model
  acc   Blood1                Blood         # CO2 model
  acc   Blood2                Blood         # CO2 model
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Sys-Tis(short)        Other
  acc   Sys-Tis(long)         Other
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other
  acc   ST3                   Other
  exc   Exhalation            ---
  exc   Excreta(skin)         ---


[{C}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.66 Table 3.5
  Blood1                  Exhalation               36.2
  Blood1                  Excreta(skin)             0.3
  Blood1                  UB-con                    0.65
  Blood1                  RC-con                    0.15
  Blood1                  ST0                      60
  Blood1                  ST1                       1.8
  Blood1                  ST2                       0.3
  Blood1                  ST3                       0.44
  Blood1                  T-bone-S                  0.09
  Blood1                  C-bone-S                  0.06
  Blood1                  T-bone-V                  0.006
  Blood1                  C-bone-V                  0.004
  ST0                     Blood1                   49.91
  ST1                     Blood1                    1.331
  ST2                     Blood1                    0.2218
  ST3                     Blood1                    0.01664
  ST1                     Blood2                    0.05545
  ST2                     Blood2                    0.009242
  ST3                     Blood2                    0.0006931
  Blood2                  UB-con                 1000
  T-bone-S                Blood1                    0.01733
  C-bone-S                Blood1                    0.01733
  T-bone-V                Blood1                    0.000493
  C-bone-V                Blood1                    0.0000821

# ICRP Publ.134 p.68 Table 3.6
  Blood0                  Sys-Tis(short)            1.2
  Blood0                  Sys-Tis(long)             0.3
  Blood0                  UB-con                    1.5
  Sys-Tis(short)          Blood1                    0.0693
  Sys-Tis(short)          Blood0                    0.0924
  Sys-Tis(short)          RC-con                    0.0693
  Sys-Tis(long)           Blood1                    0.0099
