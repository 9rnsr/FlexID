[{S}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------

# ICRP Publ.134 p.108 Fig.5.1 inorganic sulpher
  acc   Blood                 Blood
  acc   Cartilage             Cartilage
  acc   RedMarrow             R-marrow
  acc   Other                 Other


[{S}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.108 Table 5.4
  Blood                   RedMarrow                 0.075
  Blood                   Cartilage                 0.25
  Blood                   Other                     0.175
  Blood                   UB-con                    1.8
  Blood                   RC-con                    0.2
  RedMarrow               Blood                     0.3
  Cartilage               Blood                     0.1
  Other                   Blood                     3.5
