[parameter]
  # ICRP Publ.134 p.20 Table 2.2 / Tritiated water, Absorption type V

  ${H}/fA    = 0            # 小腸から血液への吸収は無い
  ${H}/fA_Re = 0            # 小腸から血液への吸収は無い

  ${H}/Blood = 'Blood'


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.134 p.20 Table 2.2
# ICRP Publ.134 p.19 Para.21
  $Blood                  100.0%
  Breath(discharge)         0.0%


[include:../share/common_HATM+UB.inc]

[include:element_1-H_HTO.inc]
