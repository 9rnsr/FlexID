[parameter]
  # ICRP Publ.134 p.84 Table 4.2 / Inhaled particulate materials, Absorption type F
  $fr = 1
  $sr = 1
  $ss = 0
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

  ${P}/fA    = 0.8          # ICRP Publ.134 p.84 Table 4.2 / Inhaled particulate materials, Absorption type F
  ${P}/fA_Re = 0.8          # footnote¶

  ${P}/Blood = 'Plasma'


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_15-P.inc]
