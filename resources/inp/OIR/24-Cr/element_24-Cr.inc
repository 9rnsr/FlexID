[{Cr}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma1               Blood
  acc   Plasma2               Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other1                Other
  acc   Other2                Other


[{Cr}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.114 Table 13.3
  Plasma1                 Plasma2                 220
  Plasma1                 UB-con                    4.8
  Plasma2                 Plasma1                  10
  Plasma2                 RC-con                    0.1
  Plasma2                 Other1                    0.7
  Plasma2                 Other2                    0.027
  Plasma2                 Kidneys                   0.015
  Plasma2                 Liver                     0.15
  Plasma2                 T-bone-S                  0.01
  Plasma2                 C-bone-S                  0.01
  Other1                  Plasma1                   0.25
  Other2                  Plasma1                   0.00005
  Liver                   Plasma1                   0.01
  Kidneys                 Plasma1                   0.007
  T-bone-S                Plasma1                   0.000493
  C-bone-S                Plasma1                   0.0000821
