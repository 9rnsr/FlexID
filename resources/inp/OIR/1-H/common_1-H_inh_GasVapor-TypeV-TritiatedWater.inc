[include:common_1-H_parameter.inc]

[parameter]
  ${H}/fA    = fA_MaxValueOfIng
  ${H}/fA_Re = fA_MaxValueOfIng


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.134 p.20 Table 2.2
# ICRP Publ.134 p.19 Para.21
  $Blood                  100.0%
  Environment               0.0%


[{H}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  exc   Environment           ---


[include:../share/common_HATM+UB.inc]

[include:element_1-H_HTO.inc]
