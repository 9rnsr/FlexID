[{Cu}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma1               Blood
  acc   Plasma2               Blood
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Other                 Other


[{Cu}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.157 Table 16.3
  Plasma1                 Liver1                   25
  Plasma1                 UB-con                    0.00014
  Liver1                  SI-conRe                 19
  Liver1                  Liver2                  200
  Liver2                  Plasma2                   1.3
  Plasma2                 Other                    15
  Other                   Plasma1                   0.3
