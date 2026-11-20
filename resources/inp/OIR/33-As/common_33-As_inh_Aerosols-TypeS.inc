[include:common_33-As_parameter.inc]

[parameter]
  # ICRP Publ.151 p.179 Table 19.2 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${As}/fA    = 0.01
  ${As}/fA_Re = fA_MaxValueOfIng        # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_33-As.inc]
[include:element_32-Ge.inc]
