[include:common_52-Te_parameter.inc]

[parameter]
  # ICRP Publ.137 p.73 Table 4.3 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Te}/fA    = 0.003
  ${Te}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  $fA    = fr * fA_MaxValueOfIng
  $fA_Re =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_54-Xe.inc]
[include:element_53-I.inc]
[include:element_52-Te.inc]
[include:element_51-Sb.inc]
