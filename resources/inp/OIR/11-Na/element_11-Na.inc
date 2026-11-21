[{Na}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Other                 Other
  exc   Excreta(sweat)        ---


[{Na}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.38 Table 4.3
  Blood                   UB-con                    0.4418
  Blood                   RC-con                    0.0047
  Blood                   Excreta(sweat)            0.0235
  Blood                   Other                    95
  Blood                   T-bone-S                  1.0
  Blood                   C-bone-S                  4.0
  Other                   Blood                    25
  T-bone-S                Blood                     2.0
  T-bone-S                T-bone-V                  0.00055
  C-bone-S                Blood                     2.0
  C-bone-S                C-bone-V                  0.00055
  T-bone-V                Blood                     0.002
  C-bone-V                Blood                     0.002
