[include:common_9-F_parameter.inc]

[parameter]
  # ICRP Publ.151 p.30 Table 3.3 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

  ${F}/fA    = 0.99 # ≒1
  ${F}/fA_Re = fA_MaxValueOfIng


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_9-F.inc]
