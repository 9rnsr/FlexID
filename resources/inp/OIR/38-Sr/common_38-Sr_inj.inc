[include:common_38-Sr_parameter.inc]

[parameter]
$if parent
  ${Sr}/fA    = fA_MaxValueOfIng
  ${Sr}/fA_Re = 0
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_39-Y.inc]
[include:element_38-Sr.inc]
[include:element_37-Rb.inc]
[include:element_36-Kr.inc]
