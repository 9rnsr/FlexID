[include:common_42-Mo_parameter.inc]

[parameter]
  # ICRP Publ.134 p.305 Table 14.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Mo}/fA    = 0.9
  ${Mo}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_43-Tc.inc]
[include:element_42-Mo.inc]
[include:element_41-Nb.inc]
