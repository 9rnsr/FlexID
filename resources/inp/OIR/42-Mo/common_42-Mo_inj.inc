[include:common_42-Mo_parameter.inc]

[parameter]
$if parent
  ${Mo}/fA    = fA_MaxValueOfIng
  ${Mo}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_43-Tc.inc]
[include:element_42-Mo.inc]
[include:element_41-Nb.inc]
