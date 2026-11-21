[{Cl}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   Other                 Other


[{Cl}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.75 Table 8.4
  Blood                   Other                   200
  Blood                   UB-con                    0.83
  Blood                   RC-con                    0.0083
  Other                   Blood                    15

$if progeny

  {Cl}/Blood              Blood                     ---
  {Cl}/Other              Other                     ---

$endif
