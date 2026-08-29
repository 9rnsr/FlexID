[include:common_43-Tc_parameter.inc]

[parameter]
$if parent
  ${Tc}/fA    = fA_MaxValueOfIng
  ${Tc}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_43-Tc.inc]
