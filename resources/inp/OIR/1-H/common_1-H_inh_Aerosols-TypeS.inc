[include:common_1-H_parameter.inc]

[parameter]
  # ICRP Publ.134 p.21 Table 2.3 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0           # footnote†
  $sb = 0           # footnote†

  ${H}/fA    = 0.01
  ${H}/fA_Re = fA_MaxValueOfIng


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_1-H_HTO.inc]
