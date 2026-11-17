[include:common_6-C_parameter.inc]

[parameter]
  ${C}/fA    = fA_MaxValueOfIng
  ${C}/fA_Re = fA_MaxValueOfIng


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.134 p.52 Table 3.2
# ICRP Publ.134 p.54 Para.88
  $Blood                   0.3%
  Environment             99.7%


[{C}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  exc   Environment           ---


[include:../share/common_HATM+UB.inc]

[include:element_6-C_Methane.inc]
