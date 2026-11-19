[{Tc}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   S-glands              S-glands
  acc   OralcavityRe          O-cavity
  acc   Oesophagus-sRe        Oesophag-s
  acc   St-conRe              St-cont
  acc   St-wall               St-wall
  acc   RC-wall               RC-wall
  acc   Thyroid1              Thyroid       # fast turnover
  acc   Thyroid2              Thyroid       # slow turnover
  acc   Liver1                Liver         # fast turnover
  acc   Liver2                Liver         # slow turnover
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissues
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Tc}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.130 p.76 Table 3.4
# ICRP Publ.130 p.77 Para.143
# ICRP Publ.134 p.334 Fig.15.1
  OralcavityRe            Oesophagus-sRe          720
  Oesophagus-sRe          St-conRe               2160
  St-conRe                SI-conRe                 20.57

# ICRP Publ.134 p.334 Table 15.3
  Blood                   Thyroid1                  7.0
  Blood                   ST0                      71.88
  Blood                   ST1                       3.0
  Blood                   ST2                       0.18
  Blood                   UB-con                    1.7
  Blood                   S-glands                  2.6
  Blood                   St-wall                   4.3
  Blood                   Kidneys1                  0.7
  Blood                   Kidneys2                  0.04
  Blood                   Liver1                    4.5
  Blood                   RC-wall                   3.4
  Blood                   T-bone-S                  0.35
  Blood                   C-bone-S                  0.35
  Thyroid1                Blood                   100
  Thyroid1                Thyroid2                  1.0
  Thyroid2                Blood                     1.0
  ST0                     Blood                    50
  ST1                     Blood                     0.462
  ST2                     Blood                     0.0347
  S-glands                OralcavityRe             50
  St-wall                 St-conRe                 50
  Kidneys1                UB-con                    8.32
  Kidneys2                Blood                     0.0347
  Liver1                  Blood                     8.234
  Liver1                  Liver2                    0.0832
  Liver2                  Blood                     0.0347
  RC-wall                 RC-con                    1.39
  T-bone-S                Blood                     0.457
  T-bone-S                T-bone-V                  0.00462
  C-bone-S                Blood                     0.457
  C-bone-S                C-bone-V                  0.00462
  T-bone-V                Blood                     0.000493
  C-bone-V                Blood                     0.0000821

$if progeny

  {Tc}/OralcavityRe       OralcavityRe              ---
  {Tc}/Oesophagus-sRe     Oesophagus-sRe            ---
  {Tc}/St-wall            St-wall                   ---
  {Tc}/St-conRe           St-conRe                  ---
  {Tc}/RC-wall            RC-wall                   ---

  {Tc}/Blood              Blood                     ---

  {Tc}/C-bone-S           C-bone-S                  ---
  {Tc}/T-bone-S           T-bone-S                  ---
  {Tc}/C-bone-V           C-bone-V                  ---
  {Tc}/T-bone-V           T-bone-V                  ---

  {Tc}/S-glands           S-glands                  ---
  {Tc}/Thyroid1           Thyroid1                  ---
  {Tc}/Thyroid2           Thyroid2                  ---
  {Tc}/Liver1             Liver1                    ---
  {Tc}/Liver2             Liver2                    ---
  {Tc}/Kidneys1           Kidneys1                  ---
  {Tc}/Kidneys2           Kidneys2                  ---

  {Tc}/ST0                ST0                       ---
  {Tc}/ST1                ST1                       ---
  {Tc}/ST2                ST2                       ---

$endif
