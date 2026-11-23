[{W}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-T-bone-V         T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Spleen                Spleen
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Other0                Other         # Rapid turnover
  acc   Other1                Other         # Intermediate turnover
  acc   Other2                Other         # Slow turnover
  exc   Excreta               ---


[{W}:transfer]
#-----------------------+---------------------+--------------
# From                  | To                  | Coefficient[/d] or [%]
#-----------------------+---------------------+--------------

# ICRP Publ.151 p.284 Table 31.3
  Plasma                  Other0                    4.99
  Plasma                  RBC                       0.0582
  Plasma                  UB-con                    8.74
  Plasma                  Kidneys1                  0.524
  Plasma                  Kidneys2                  0.0582
  Plasma                  RC-con                    0.582
  Plasma                  Spleen                    0.00582
  Plasma                  Liver1                    0.466
  Plasma                  Other1                    0.262
  Plasma                  Other2                    0.0233
  Plasma                  T-bone-S                  0.518
  Plasma                  C-bone-S                  0.414
  Other0                  Plasma                    8.32
  RBC                     Plasma                    0.347
  Kidneys1                UB-con                    1.39
  Kidneys2                Plasma                    0.0019
  Liver1                  Plasma                    0.312
  Liver1                  Liver2                    0.0347
  Other1                  Plasma                    0.0693
  Other2                  Excreta                   0.0019
  Spleen                  Plasma                    0.0019
  T-bone-S                Plasma                    0.578
  T-bone-S                Exch-T-bone-V             0.116
  C-bone-S                Plasma                    0.578
  C-bone-S                Exch-C-bone-V             0.116
  Liver2                  Plasma                    0.0019
  Noch-T-bone-V           Plasma                    0.000493
  Noch-C-bone-V           Plasma                    0.0000821
  Exch-T-bone-V           T-bone-S                  0.00277
  Exch-T-bone-V           Noch-T-bone-V             0.00416
  Exch-C-bone-V           C-bone-S                  0.00277
  Exch-C-bone-V           Noch-C-bone-V             0.00416
