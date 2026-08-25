[{Zn}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   Muscle                Muscle
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other
  exc   Excreta               ---


[{Zn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.199 Table 9.4
  Plasma                  Liver1                   60
  Plasma                  Kidneys                   4
  Plasma                  Pancreas                  3
  Plasma                  Muscle                    2
  Plasma                  RBC                       1.5
  Plasma                  ST0                      40
  Plasma                  ST1                      30
  Plasma                  ST2                       0.4
  Plasma                  UB-con                    0.13
  Plasma                  Excreta                   0.13
  Plasma                  SI-conRe                  0.2
  Plasma                  T-bone-S                  0.15
  Plasma                  C-bone-S                  0.3
  Liver1                  Plasma                   10
  Liver1                  SI-conRe                  0.067
  Liver1                  Liver2                   10
  Liver2                  Plasma                    0.6
  Kidneys                 Plasma                    0.7
  Pancreas                Plasma                    1.5
  Pancreas                SI-conRe                  1.0
  Muscle                  Plasma                    0.005
  RBC                     Plasma                    0.14
  ST0                     Plasma                   10
  ST1                     Plasma                    3
  ST2                     Plasma                    0.01
  T-bone-S                Plasma                    0.01
  C-bone-S                Plasma                    0.01
  T-bone-S                T-bone-V                  0.00053
  C-bone-S                C-bone-V                  0.00053
  T-bone-V                Plasma                    0.000493
  C-bone-V                Plasma                    0.0000821

$if progeny

  {Zn}/Plasma             Plasma                    ---
  {Zn}/RBC                RBC                       ---

  {Zn}/C-bone-S           C-bone-S                  ---
  {Zn}/T-bone-S           T-bone-S                  ---
  {Zn}/C-bone-V           C-bone-V                  ---
  {Zn}/T-bone-V           T-bone-V                  ---

  {Zn}/Liver1             Liver1                    ---
  {Zn}/Liver2             Liver2                    ---
  {Zn}/Kidneys            Kidneys                   ---
  {Zn}/Pancreas           Pancreas                  ---
  {Zn}/Muscle             Muscle                    ---

  {Zn}/ST0                ST0                       ---
  {Zn}/ST1                ST1                       ---
  {Zn}/ST2                ST2                       ---

  {Zn}/Excreta            Excreta                   ---

$endif
