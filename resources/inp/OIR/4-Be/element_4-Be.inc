[{Be}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Other1                Other
  acc   Other2                Other


[{Be}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.68 Table 3.6
  Blood1                  UB-con                   20
  Blood1                  RC-con                    5.0
  Blood1                  T-bone-S                 15
  Blood1                  C-bone-S                 15
  Blood1                  Liver1                    5.0
  Blood1                  Kidneys                   3.0
  Blood1                  Other1                   30
  Blood1                  Other2                    5.0
  Blood1                  Blood2                    2.0
  Blood2                  Blood1                    0.014
  T-bone-S                Blood2                    0.0025
  C-bone-S                Blood2                    0.0025
  Liver1                  Blood1                    0.2
  Liver1                  Liver2                    0.05
  Liver2                  Blood2                    0.0019
  Kidneys                 Blood1                    0.15
  Other1                  Blood1                    0.07
  Other2                  Blood2                    0.00025
