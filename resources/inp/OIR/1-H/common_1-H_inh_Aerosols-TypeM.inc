[parameter]
  # ICRP Publ.134 p.21 Table 2.3 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0
  $sb = 0

  ${H}/fA    = 0.2
  ${H}/fA_Re = 0

  $Blood = 'Blood'


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_1-H_HTO.inc]
