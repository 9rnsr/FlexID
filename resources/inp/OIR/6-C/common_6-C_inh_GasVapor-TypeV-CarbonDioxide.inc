[parameter]
  # ICRP Publ.134 p.53 Table 3.3 / Carbon dioxide, Absorption type V

  ${C}/fA    = 0            # 小腸から血液への吸収は無い
  ${C}/fA_Re = 0            # 小腸から血液への吸収は無い

  ${C}/Blood = 'Blood1'     # CO2 model


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
