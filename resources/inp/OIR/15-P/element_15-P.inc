[{P}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   Noch-T-bone-V         T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissues
  acc   ST0                   Other         # Rapid turnover
  acc   ST1                   Other         # Intermediate turnover
  acc   ST2                   Other         # Slow turnover


[{P}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.92 Table 4.3
  Plasma                  UB-con                    4.4
  Plasma                  RC-con                    0.2
  Plasma                  T-bone-S                  4.44
  Plasma                  C-bone-S                  3.56
  Plasma                  ST0                      10.18
  Plasma                  ST1                      10.18
  Plasma                  ST2                       0.1
  Plasma                  RBC                       2.4
  Plasma                  Kidneys1                  0.4
  Plasma                  Kidneys2                  0.14
  Plasma                  Liver1                    4.0
  RBC                     Plasma                    0.6931
  ST0                     Plasma                    0.3466
  ST1                     Plasma                    0.03466
  ST2                     Plasma                    0.00038
  Kidneys1                UB-con                    0.6931
  Kidneys2                Plasma                    0.03466
  Liver1                  Liver2                    0.3466
  Liver1                  Plasma                    1.04
  Liver2                  Plasma                    0.03466
  C-bone-S                Plasma                    0.578
  C-bone-S                Exch-C-bone-V             0.116
  T-bone-S                Plasma                    0.578
  T-bone-S                Exch-T-bone-V             0.116
  Exch-C-bone-V           C-bone-S                  0.002773
  Exch-C-bone-V           Noch-C-bone-V             0.004159
  Exch-T-bone-V           T-bone-S                  0.002773
  Exch-T-bone-V           Noch-T-bone-V             0.004159
  Noch-C-bone-V           Plasma                    0.0000821
  Noch-T-bone-V           Plasma                    0.000493
