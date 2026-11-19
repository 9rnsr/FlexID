[{Ca}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   Noch-T-bone-V         T-bone-V
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Ca}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.121 Table 6.3
  Blood                   UB-con                    0.60
  Blood                   RC-con                    0.45
  Blood                   T-bone-S                  2.08
  Blood                   C-bone-S                  1.67
  Blood                   ST0                       8.70
  Blood                   ST1                       1.50
  Blood                   ST2                       0.00075
  T-bone-S                Blood                     0.578
  T-bone-S                Exch-T-bone-V             0.116
  C-bone-S                Blood                     0.578
  C-bone-S                Exch-C-bone-V             0.116
  ST0                     Blood                     2.9
  ST1                     Blood                     0.1733
  ST2                     Blood                     0.00038
  Exch-T-bone-V           T-bone-S                  0.002773
  Exch-T-bone-V           Noch-T-bone-V             0.004159
  Exch-C-bone-V           C-bone-S                  0.002773
  Exch-C-bone-V           Noch-C-bone-V             0.004159
  Noch-C-bone-V           Blood                     0.0000821
  Noch-T-bone-V           Blood                     0.000493
