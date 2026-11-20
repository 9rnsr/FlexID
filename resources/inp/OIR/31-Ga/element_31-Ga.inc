[{Ga}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   RedMarrow             R-marrow
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   Spleen                Spleen
  acc   Muscle                Muscle
  acc   ST0                   Other
  acc   ST1                   Other


[{Ga}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.164 Table 17.3
  Blood                   RC-con                    0.15
  Blood                   Liver                     0.3
  Blood                   Kidneys                   0.4
  Blood                   Spleen                    0.05
  Blood                   T-bone-S                  0.5
  Blood                   C-bone-S                  0.5
  Blood                   RedMarrow                 0.25
  Blood                   Muscle                    0.2
  Blood                   Pancreas                  0.005
  Blood                   ST0                       2.145
  Blood                   ST1                       0.5
  Liver                   Blood                     0.139
  Kidneys                 UB-con                    1.39
  Spleen                  Blood                     0.139
  T-bone-S                Blood                     0.347
  C-bone-S                Blood                     0.347
  RedMarrow               Blood                     0.347
  Muscle                  Blood                     0.139
  Pancreas                Blood                     0.139
  ST0                     Blood                     1.39
  ST1                     Blood                     0.0019
