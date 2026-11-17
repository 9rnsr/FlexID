[parameter]
  # ICRP Publ.134 p.20 Table 2.2 / Tritiated methane, Absorption type V

  ${H}/fA    = 0            # 小腸から血液への吸収は無い
  ${H}/fA_Re = 0            # 小腸から血液への吸収は無い

  ${H}/Blood = 'Blood'


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.134 p.20 Table 2.2
# ICRP Publ.134 p.22 Para.25
  $Blood                   0.3%
  Breath(discharge)       99.7%


[include:../share/common_HATM+UB.inc]

[include:element_1-H_HTO.inc]
