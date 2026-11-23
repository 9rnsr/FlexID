[include:common_50-Sn_parameter.inc]

[parameter]
  # ICRP Publ.151 p.260 Table 28.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Sn}/fA    = 0.004
  ${Sn}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_52-Te.inc]
[include:element_51-Sb.inc]
[include:element_50-Sn.inc]
[include:element_49-In.inc]
[include:element_48-Cd.inc]
