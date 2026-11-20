[{Ge}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other                 Other


[{Ge}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.173 Table 18.3
  Blood                   Other                     0.89
  Blood                   Kidneys                   0.2
  Blood                   Liver                     0.4
  Blood                   UB-con                    8.3
  Blood                   RC-con                    0.01
  Blood                   T-bone-S                  0.1
  Blood                   C-bone-S                  0.1
  Other                   Blood                     0.3
  Kidneys                 UB-con                    1.2
  Liver                   Blood                     0.9
  T-bone-S                Blood                     0.3
  C-bone-S                Blood                     0.3
  T-bone-S                T-bone-V                  0.0015
  C-bone-S                C-bone-V                  0.0015
  T-bone-V                Blood                     0.000493
  C-bone-V                Blood                     0.0000821
