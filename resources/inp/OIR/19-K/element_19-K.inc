[{K}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   RedMarrow             R-marrow
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Muscle                Muscle
  acc   Other                 Other
  exc   Excreta               ---


[{K}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.82 Table 9.3
  Plasma                  RBC                       6
  Plasma                  Kidneys                 257
  Plasma                  Liver                   230
  Plasma                  Muscle                  255
  Plasma                  T-bone-S                 16.8
  Plasma                  C-bone-S                 11.2
  Plasma                  RedMarrow                28
  Plasma                  Other                   470
  Plasma                  UB-con                    5.5
  Plasma                  RC-con                    0.83
  Plasma                  Excreta                   0.2
  RBC                     Plasma                    0.38
  Kidneys                 Plasma                  214
  Liver                   Plasma                   24.5
  Muscle                  Plasma                    1.345
  T-bone-S                Plasma                    2.67
  C-bone-S                Plasma                    2.67
  RedMarrow               Plasma                    2.67
  Other                   Plasma                   12
