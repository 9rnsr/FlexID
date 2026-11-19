[{S}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------

# ICRP Publ.134 p.109 Fig.5.2 organic sulpher
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   AllTissues            Other
  exc   Excreta(hair)         ---


[{S}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.109 Table 5.5
  Blood1                  Blood2                    8.3
  Blood1                  UB-con                    4.0
  Blood2                  UB-con                    0.0011
  Blood2                  Excreta(hair)             0.0009
  Blood2                  SI-conRe                  0.0002
  Blood2                  AllTissues                0.0170
  AllTissues              Blood2                    0.0042
