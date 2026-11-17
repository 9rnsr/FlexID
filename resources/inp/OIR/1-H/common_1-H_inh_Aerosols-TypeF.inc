[parameter]
  # ICRP Publ.134 p.21 Table 2.3 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 100
  $ss =   0
  $fb = 0
  $sb = 0

  ${H}/fA    = 0.99 # ≒1
  ${H}/fA_Re = 0

  $Blood = 'Blood'


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_1-H_HTO.inc]
