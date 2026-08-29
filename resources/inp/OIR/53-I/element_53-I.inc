[{I}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood         # inorganic iodine in blood
  acc   Blood2                Blood         #   organic iodine in blood
  acc   S-glands              S-glands
  acc   OralcavityRe          O-cavity
  acc   Oesophagus-sRe        Oesophag-s
  acc   St-conRe              St-cont
  acc   St-wall               St-wall
  acc   Thyroid1              Thyroid
  acc   Thyroid2              Thyroid
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Other1                Other
  acc   Other2                Other
  acc   Other3                Other
  acc   Other4                Other


[{I}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.130 p.76 Table 3.4
# ICRP Publ.130 p.77 Para.143
# ICRP Publ.137 p.106 Fig.5.2
  OralcavityRe            Oesophagus-sRe          720
  Oesophagus-sRe          St-conRe               2160
  St-conRe                SI-conRe                 20.57

# ICRP Publ.137 p.108 Table 5.4
  Blood1                  Thyroid1                  7.26
  Blood1                  UB-con                   11.84
  Blood1                  S-glands                  5.16
  Blood1                  St-wall                   8.60
  Blood1                  Other1                  600
  Blood1                  Kidneys1                 25
  Blood1                  Liver1                   15
  S-glands                OralcavityRe             50
  St-wall                 St-conRe                 50
  Thyroid1                Thyroid2                 95
  Thyroid1                Blood1                   36
  Thyroid2                Blood2                    0.0077
  Thyroid2                Blood1                    0
  Other1                  Blood1                  330
  Other1                  Other2                   35
  Other2                  Other1                   56
  Kidneys1                Blood1                  100
  Liver1                  Blood1                  100
  Blood2                  Other3                   15
  Other3                  Blood2                   21
  Other3                  Other4                    1.2
  Other4                  Other3                    0.62
  Other4                  Blood1                    0.14
  Blood2                  Kidneys2                  3.6
  Kidneys2                Blood2                   21
  Kidneys2                Blood1                    0.14
  Blood2                  Liver2                   21
  Liver2                  Blood2                   21
  Liver2                  Blood1                    0.14
  Liver2                  RC-con                    0.08

$if progeny

  {I}/Blood1              Blood1                    ---
  {I}/Blood2              Blood2                    ---

  {I}/S-glands            S-glands                  ---
  {I}/OralcavityRe        OralcavityRe              ---
  {I}/Oesophagus-sRe      Oesophagus-sRe            ---
  {I}/St-conRe            St-conRe                  ---
  {I}/St-wall             St-wall                   ---
  {I}/Thyroid1            Thyroid1                  ---
  {I}/Thyroid2            Thyroid2                  ---
  {I}/Liver1              Liver1                    ---
  {I}/Liver2              Liver2                    ---
  {I}/Kidneys1            Kidneys1                  ---
  {I}/Kidneys2            Kidneys2                  ---

  {I}/Other1              Other1                    ---
  {I}/Other2              Other2                    ---
  {I}/Other3              Other3                    ---
  {I}/Other4              Other4                    ---

$endif
