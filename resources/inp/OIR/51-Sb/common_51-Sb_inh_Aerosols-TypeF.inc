[include:common_51-Sb_parameter.inc]

[parameter]
  # ICRP Publ.137 p.48 Table 3.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss = 0
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Sb}/fA    = 0.05
  ${Sb}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_53-I.inc]
[include:element_52-Te.inc]
[include:element_51-Sb.inc]
