[{Mg}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Other0                Other
  acc   Other1                Other
  acc   Other2                Other


[{Mg}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.49 Table 5.3
  Plasma                  RBC                       0.05
  Plasma                  UB-con                    1
  Plasma                  SI-conRe                  0.2
  Plasma                  T-bone-S                  4
  Plasma                  C-bone-S                  4
  Plasma                  Other0                   70
  Plasma                  Other1                   19.75
  Plasma                  Other2                    1
  RBC                     Plasma                    0.03
  T-bone-S                Plasma                    0.18
  T-bone-S                T-bone-V                  0.02
  C-bone-S                Plasma                    0.18
  C-bone-S                C-bone-V                  0.02
  Other0                  Plasma                   60
  Other1                  Plasma                    3
  Other2                  Plasma                    0.023
  T-bone-V                Plasma                    0.023
  C-bone-V                Plasma                    0.023
