[{Mn}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Brain                 Brain
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Mn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.123 Table 14.3
  Plasma                  Liver1                  300
  Plasma                  Kidneys                  50
  Plasma                  Pancreas                 50
  Plasma                  UB-con                    2
  Plasma                  RC-con                   10
  Plasma                  ST0                     391.8
  Plasma                  ST1                     150
  Plasma                  ST2                      40
  Plasma                  C-bone-S                  2.5
  Plasma                  T-bone-S                  2.5
  Plasma                  Brain                     1.0
  Plasma                  RBC                       0.2
  Liver1                  SI-conRe                  0.139
  Liver1                  Liver2                    0.555
  Liver2                  Plasma                    0.347
  Kidneys                 Plasma                    0.347
  Pancreas                Plasma                    0.347
  Pancreas                SI-conRe                  0.347
  ST0                     Plasma                   33.3
  ST1                     Plasma                    0.347
  ST2                     Plasma                    0.0173
  C-bone-S                Plasma                    0.01716
  C-bone-S                C-bone-V                  0.0001733
  T-bone-S                Plasma                    0.01716
  T-bone-S                T-bone-V                  0.0001733
  C-bone-V                Plasma                    0.0000821
  T-bone-V                Plasma                    0.000493
  Brain                   Plasma                    0.00462
  RBC                     Plasma                    0.00833

$if progeny

  {Mn}/Plasma             Plasma                    ---
  {Mn}/RBC                RBC                       ---

  {Mn}/C-bone-S           C-bone-S                  ---
  {Mn}/T-bone-S           T-bone-S                  ---
  {Mn}/C-bone-V           C-bone-V                  ---
  {Mn}/T-bone-V           T-bone-V                  ---

  {Mn}/Brain              Brain                     ---
  {Mn}/Liver1             Liver1                    ---
  {Mn}/Liver2             Liver2                    ---
  {Mn}/Kidneys            Kidneys                   ---
  {Mn}/Pancreas           Pancreas                  ---

  {Mn}/ST0                ST0                       ---
  {Mn}/ST1                ST1                       ---
  {Mn}/ST2                ST2                       ---

$endif
