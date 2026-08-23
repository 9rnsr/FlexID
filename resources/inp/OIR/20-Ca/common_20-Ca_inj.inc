[include:common_20-Ca_parameter.inc]

[parameter]
$if parent
  ${Ca}/fA    = fA_MaxValueOfIng
  ${Ca}/fA_Re = fA_MaxValueOfIng
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_20-Ca.inc]
[include:element_21-Sc.inc]
