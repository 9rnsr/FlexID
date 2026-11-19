[include:common_15-P_parameter.inc]

[parameter]
  # ICRP Publ.134 p.84 Table 4.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 1
  $ss = 0.005
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

  ${P}/fA    = 0.16
  ${P}/fA_Re = fA_MaxValueOfIng     # footnote¶


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_15-P.inc]
