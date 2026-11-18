[include:common_1-H_parameter.inc]

[parameter]
  ${H}/fA    = fA_MaxValueOfIng
  ${H}/fA_Re = fA_MaxValueOfIng


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.134 p.20 Table 2.2
# ICRP Publ.134 p.22 Para.25
  $Blood                   0.3%
  Environment             99.7%


[{H}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  exc   Environment           ---


[include:../share/common_HATM+UB.inc]

[include:element_1-H_HTO.inc]
