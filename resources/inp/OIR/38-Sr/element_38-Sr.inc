[{Sr}:compartment]
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


[{Sr}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.220 Table 10.3
  Blood                   UB-con                    1.73
  Blood                   RC-con                    0.525
  Blood                   T-bone-S                  2.08
  Blood                   C-bone-S                  1.67
  Blood                   ST0                       7.5
  Blood                   ST1                       1.5
  Blood                   ST2                       0.003
  T-bone-S                Blood                     0.578
  T-bone-S                Exch-T-bone-V             0.116
  C-bone-S                Blood                     0.578
  C-bone-S                Exch-C-bone-V             0.116
  ST0                     Blood                     2.50
  ST1                     Blood                     0.116
  ST2                     Blood                     0.00038
  Exch-T-bone-V           T-bone-S                  0.0043
  Exch-T-bone-V           Noch-T-bone-V             0.0043
  Exch-C-bone-V           C-bone-S                  0.0043
  Exch-C-bone-V           Noch-C-bone-V             0.0043
  Noch-C-bone-V           Blood                     0.0000821
  Noch-T-bone-V           Blood                     0.000493

$if progeny

  {Sr}/Blood              Blood                     ---

  {Sr}/C-bone-S           C-bone-S                  ---
  {Sr}/T-bone-S           T-bone-S                  ---
  {Sr}/Exch-C-bone-V      Exch-C-bone-V             ---
  {Sr}/Exch-T-bone-V      Exch-T-bone-V             ---
  {Sr}/Noch-C-bone-V      Noch-C-bone-V             ---
  {Sr}/Noch-T-bone-V      Noch-T-bone-V             ---

  {Sr}/ST0                ST0                       ---
  {Sr}/ST1                ST1                       ---
  {Sr}/ST2                ST2                       ---

$endif
