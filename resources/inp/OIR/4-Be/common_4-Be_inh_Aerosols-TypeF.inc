[include:common_4-Be_parameter.inc]

[parameter]
  # ICRP Publ.151 p.21 Table 2.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

  ${Be}/fA    = 0.005
  ${Be}/fA_Re = fA_MaxValueOfIng    # footnote§


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_4-Be.inc]
