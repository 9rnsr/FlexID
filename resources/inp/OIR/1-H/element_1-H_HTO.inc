[{H}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   ExtravasHTO           Other
  acc   OBT1                  Other
  acc   OBT2                  Other
  exc   Breath(discharge)     ---
  exc   Skin(discharge)       ---


[{H}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.35 Table 2.5 & footnote†
  Blood                   ExtravasHTO             400
  ExtravasHTO             OBT1                      0.0006
  ExtravasHTO             OBT2                      0.00008
  Blood                   UB-con                    0.385       # = 55% of 0.7/d
  Blood                   RC-con                    0.028       # =  4% of 0.7/d
  Blood                   Breath(discharge)         0.084       # = 12% of 0.7/d
  Blood                   Skin(discharge)           0.203       # = 29% of 0.7/d
  ExtravasHTO             Blood                    44
  OBT1                    ExtravasHTO               0.01733
  OBT2                    ExtravasHTO               0.0019
