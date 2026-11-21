[{Al}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Other0                Other
  acc   Other1                Other


[{Al}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.64 Table 6.3
  Blood1                  UB-con                    9.98
  Blood1                  RC-con                    0.166
  Blood1                  T-bone-S                  0.0832
  Blood1                  C-bone-S                  0.0832
  Blood1                  Other0                    5.74
  Blood1                  Other1                    0.582
  Blood2                  Blood1                    0.035
  Other0                  Blood1                    0.5
  Other1                  Blood2                    0.0005
  T-bone-S                Blood2                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood2                    0.000493
  C-bone-S                Blood2                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood2                    0.0000821
