[include:common_9-F_parameter.inc]

[parameter]
  # ICRP Publ.151 p.30 Table 3.3 / Inhaled particulate materials, Absorption type M

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

  ${F}/fA    = 0.01
  ${F}/fA_Re = fA_MaxValueOfIng


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_9-F.inc]
