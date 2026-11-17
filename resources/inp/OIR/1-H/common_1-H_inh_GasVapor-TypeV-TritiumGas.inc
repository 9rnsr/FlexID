[parameter]
  # ICRP Publ.134 p.20 Table 2.2 / Tritium gas, Absorption type V

  ${H}/fA    = 0            # 小腸から血液への吸収は無い
  ${H}/fA_Re = 0            # 小腸から血液への吸収は無い

  ${H}/Blood = 'Blood'


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.134 p.20 Table 2.2
# ICRP Publ.134 p.21 Para.23-34
  $Blood                   0.01%
  Environment             99.99%


[{H}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  exc   Environment           ---


[include:../share/common_HATM+UB.inc]

[include:element_1-H_HTO.inc]
