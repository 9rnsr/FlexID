[include:common_53-I_parameter.inc]

[parameter]
  # ICRP Publ.137 p.97 Table 5.3 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${I}/fA    = 0.01
  ${I}/fA_Re = fA_MaxValueOfIng     # footnote¶
$else
  ${*}/fA    = fr * fA_MaxValueOfIng
  ${*}/fA_Re =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_54-Xe.inc]
[include:element_53-I.inc]
[include:element_52-Te.inc]
[include:element_51-Sb.inc]
