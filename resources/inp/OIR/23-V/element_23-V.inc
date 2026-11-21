[{V}:compartment]
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


[{V}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.106 Table 12.3
  Blood1                  Blood2                    2.8
  Blood1                  Liver1                    0.24
  Blood1                  Kidneys                   0.4
  Blood1                  Other1                    2.44
  Blood1                  Other2                    0.24
  Blood1                  UB-con                    1.52
  Blood1                  SI-conRe                  0.12
  Blood1                  T-bone-S                  0.12
  Blood1                  C-bone-S                  0.12
  Blood2                  Blood1                    0.5
  Liver1                  SI-conRe                  0.09
  Liver1                  Blood1                    0.375
  Liver1                  Liver2                    0.035
  Liver2                  Blood1                    0.01
  Kidneys                 UB-con                    1.8
  Other1                  Blood1                    0.14
  Other2                  Blood1                    0.01
  T-bone-S                Blood1                    0.01
  C-bone-S                Blood1                    0.01
