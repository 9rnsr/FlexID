[{Hg}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
 #acc   Plasma0               Blood     # Gas,Vapor only
  acc   Plasma1               Blood
  acc   Plasma2               Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Kidneys               Kidneys
  acc   Liver                 Liver
  acc   Brain1                Brain
  acc   Brain2                Brain
  acc   Other1                Other
  acc   Other2                Other
  exc   OtherExcreta          ---


[{Hg}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.339 Table 36.5
  Plasma1                 RBC                       0.48
  Plasma1                 Plasma2                   2.4
  Plasma1                 Kidneys                   7.2
  Plasma1                 Liver                     4.8
  Plasma1                 Brain1                    0.048
  Plasma1                 T-bone-S                  0.024
  Plasma1                 C-bone-S                  0.024
  Plasma1                 Other1                    5.184
  Plasma1                 Other2                    0.72
  Plasma1                 SI-conRe                  1.92
  Plasma1                 OtherExcreta              1.2
  RBC                     Plasma1                   0.33
  Plasma2                 Plasma1                   0.6
  Kidneys                 UB-con                    0.0198
  Liver                   SI-conRe                  0.1733
  Brain1                  Plasma1                   0.0329
  Brain1                  Brain2                    0.00173
  Brain2                  Plasma1                   0.00038
  T-bone-S                Plasma1                   0.0347
  C-bone-S                Plasma1                   0.0347
  Other1                  Plasma1                   0.0347
  Other2                  Plasma1                   0.00693
