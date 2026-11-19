[{Au}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other1                Other
  acc   Other2                Other


[{Au}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.321 Table 35.3
  Blood1                  Blood2                    0.1
  Blood1                  Kidneys                   0.1
  Blood1                  Liver                     0.1
  Blood1                  Other1                    0.18
  Blood1                  Other2                    0.1
  Blood1                  UB-con                    0.3
  Blood1                  RC-con                    0.1
  Blood1                  T-bone-S                  0.01
  Blood1                  C-bone-S                  0.01
  Blood2                  Blood1                    0.139
  Kidneys                 UB-con                    0.0693
  Liver                   Blood1                    0.0693
  Other1                  Blood1                    0.0693
  Other2                  Blood1                    0.0139
  T-bone-S                Blood1                    0.0693
  C-bone-S                Blood1                    0.0693

$if progeny

# ICRP Publ.151 p.320 Para.651
# > (651) Progeny of gold addressed in this publication are radioisotopes of gold,
# > rhenium, osmium, iridium, and platinum. The model for gold as a parent is applied
# > to gold as a progeny of gold. ...
# > (651) 本刊行物で扱う金の子孫元素は、金、レニウム、オスミウム、イリジウム、および
# > 白金の放射性同位元素である。親元素としての金のモデルは、金の子孫元素としての金に
# > 適用される。 ...

  {Au}/Blood1            Blood1                    ---
  {Au}/Blood2            Blood2                    ---

  {Au}/C-bone-S          C-bone-S                  ---
  {Au}/T-bone-S          T-bone-S                  ---

  {Au}/Liver             Liver                     ---
  {Au}/Kidneys           Kidneys                   ---

  {Au}/Other1            Other1                    ---
  {Au}/Other2            Other2                    ---

$endif
