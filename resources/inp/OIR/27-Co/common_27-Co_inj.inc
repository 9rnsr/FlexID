[include:common_27-Co_parameter.inc]

[parameter]
$if parent
  ${Co}/fA    = fA_MaxValueOfIng
  ${Co}/fA_Re = fA_MaxValueOfIng	# footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_27-Co.inc]
[include:element_26-Fe.inc]
