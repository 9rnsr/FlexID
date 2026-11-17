[parameter]
  # ICRP Publ.134 p.53 Table 3.3 / Carbon monoxide, Absorption type V

  ${C}/fA    = 0            # 小腸から血液への吸収は無い
  ${C}/fA_Re = 0            # 小腸から血液への吸収は無い

  ${C}/Blood = 'Blood'      # CO model


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.134 p.52 Table 3.2
# ICRP Publ.134 p.52 Para.86
  $Blood                  40%
  Environment             60%


[{C}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  exc   Environment           ---


[include:../share/common_HATM+UB.inc]

[include:element_6-C_CO.inc]
