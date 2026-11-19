[include:common_4-Be_parameter.inc]

[parameter]
  # ICRP Publ.151 p.21 Table 2.2 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

  ${Be}/fA    = 5E-5
  ${Be}/fA_Re = fA_MaxValueOfIng    # footnote§


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_4-Be.inc]
