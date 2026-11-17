[include:common_6-C_parameter.inc]

[parameter]
  ${C}/fA    = fA_MaxValueOfIng
  ${C}/fA_Re = fA_MaxValueOfIng


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.134 p.52 Table 3.2
# ICRP Publ.134 p.53 Para.87
  $Blood                 100%
  Environment              0%


[{C}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  exc   Environment           ---


[include:../share/common_HATM+UB.inc]

[include:element_6-C_CO2.inc]
