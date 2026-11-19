[{F}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   Kidneys               Kidneys
  acc   T-bone-S1             T-bone-S
  acc   T-bone-S2             T-bone-S
  acc   C-bone-S1             C-bone-S
  acc   C-bone-S2             C-bone-S
  acc   Other                 Other


[{F}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.33 Table 3.4
  Blood                   T-bone-S1               197
  Blood                   C-bone-S1               158
  Blood                   Other                  1720
  Blood                   Kidneys                  34.6
  T-bone-S1               Blood                  1310
  T-bone-S1               T-bone-S2               867
  C-bone-S1               Blood                  1310
  C-bone-S1               C-bone-S2               867
  T-bone-S2               T-bone-S1                28.8
  C-bone-S2               C-bone-S1                28.8
  Other                   Blood                   817
  Kidneys                 Blood                   559
  Kidneys                 UB-con                  881
